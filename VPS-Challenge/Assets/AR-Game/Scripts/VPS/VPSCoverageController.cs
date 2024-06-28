using Niantic.Lightship.AR.VpsCoverage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VPSCoverageController : MonoBehaviour
{
    public enum MapApp
    {
        GoogleMaps,
        AppleMaps
    }

    [SerializeField]
    private MapApp mapApp = MapApp.GoogleMaps;
    [SerializeField]
    private CoverageClientManager coverageClientManager;
    [SerializeField] private int maxResult = 10;
    [SerializeField] private VpsCoverageItem itemPrefab;
    [SerializeField] private TextMeshProUGUI status;
    [Header("UI")]
    [SerializeField] private GameObject scrollContent;

    public event Action<LocalizationTarget> OnWayspotSelected;
    public event Action OnButtonsCreated;
    public List<VpsCoverageItem> CoverageItems = new List<VpsCoverageItem>();
    // Start is called before the first frame update
    void Start()
    {
        //Invoke(nameof(DoRequest), 5.0f);
#if !UNITY_EDITOR && UNITY_ANDROID
        mapApp = MapApp.GoogleMaps;
#elif !UNITY_EDITOR && UNITY_IOS
        mapApp = MapApp.AppleMaps;
#endif

    }
    public void DoRequest()
    {
        status.text = "Requesting coverage from server...";
        coverageClientManager.TryGetCoverage(OnCoverageResult);
    }

    void Update()
    {

    }

    public void DisableCoverage()
    {
        coverageClientManager.enabled = false;
        this.enabled = false;
    }

    private void OnCoverageResult(AreaTargetsResult result)
    {
        var responseText = string.Empty;

        if (result.Status == ResponseStatus.Success)
        {

            responseText = "Response : " + result.AreaTargets.Count +
                 " targets(s) found within a radius of " + result.QueryRadius;
            Debug.Log(responseText);    
            result.AreaTargets.Sort((a, b) =>
            a.Area.Centroid.Distance(result.QueryLocation).
            CompareTo(b.Area.Centroid.Distance(result.QueryLocation)));

            var maxCount = maxResult == 0 ? result.AreaTargets.Count : Math.Min(maxResult, result.AreaTargets.Count);

            foreach (var areaResult in result.AreaTargets)
            {
                if (areaResult.Area.LocalizabilityQuality != CoverageArea.Localizability.PRODUCTION)
                {
                    //Debug.Log("But Quality...");
                    //continue;
                }

                //Debug.Log($"Got a localization target: {areaResult.Target.Name}, anchor payload: {areaResult.Target.DefaultAnchor}");

                VpsCoverageItem targetListItemInstance = Instantiate(itemPrefab, scrollContent.transform, false);
                FillTargetItem(targetListItemInstance, result.QueryLocation, areaResult.Area, areaResult.Target);


                var layout = scrollContent.GetComponent<VerticalLayoutGroup>();
                var contentTransform = scrollContent.GetComponent<RectTransform>();
                float itemHeight = itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
                contentTransform.sizeDelta = new Vector2
                (
                    contentTransform.sizeDelta.x,
                    layout.padding.top + scrollContent.transform.childCount * (layout.spacing + itemHeight)
                );

                contentTransform.anchoredPosition = new Vector2(0, int.MinValue);
                CoverageItems.Add(targetListItemInstance);
                maxCount--;
                if (maxCount == 0)
                {
                    break;
                }
            }
        }
        else
        {
            responseText = "Response : " + result.Status;
        }

        OnButtonsCreated?.Invoke();
        status.text = responseText;
    }

    private void FillTargetItem(VpsCoverageItem vpsCoverageItem, LatLng queryLocation, CoverageArea area, LocalizationTarget target)
    {
        vpsCoverageItem.transform.name = target.Name;

        if (area.LocalizabilityQuality == CoverageArea.Localizability.EXPERIMENTAL)
        {
            vpsCoverageItem.transform.Find("WayspotImage").Find("Quality").GetComponent<RawImage>().color = Color.yellow;
        }

        coverageClientManager.TryGetImageFromUrl(target.ImageURL, 
            downLoadedImage => vpsCoverageItem.WayspotImageTexture = downLoadedImage);

        vpsCoverageItem.TitleLabelText = target.Name;

        if (target.Center.Latitude == 0.0 && target.Center.Longitude == 0.0)
        {
            // For private scans without a GPS value, show the distance as unknown.
            vpsCoverageItem.DistanceLabelText += "Distance: ?";
        }
        else
        {
            double distanceInM = target.Center.Distance(queryLocation);
            vpsCoverageItem.DistanceLabelText += "Distance: " + distanceInM.ToString("N0") + " m";
        }

        vpsCoverageItem.SubscribeToNavigateButton(() => { OpenRouteInMapApp(queryLocation, target.Center); });

        vpsCoverageItem.SubscribeToCopyButton(() =>
        {
            GameManager.Instance.LocalizationSelected();
            OnWayspotSelected?.Invoke(target);
            GUIUtility.systemCopyBuffer = target.DefaultAnchor;
        });
    }

    private void OpenRouteInMapApp(LatLng from, LatLng to)
    {
        var sb = new StringBuilder();

        if (mapApp == MapApp.GoogleMaps)
        {
            sb.Append("https://www.google.com/maps/dir/?api=1&origin=");
            sb.Append(from.Latitude);
            sb.Append("+");
            sb.Append(from.Longitude);
            sb.Append("&destination=");
            sb.Append(to.Latitude);
            sb.Append("+");
            sb.Append(to.Longitude);
            sb.Append("&travelmode=walking");
        }
        else if (mapApp == MapApp.AppleMaps)
        {
            sb.Append("http://maps.apple.com/?saddr=");
            sb.Append(from.Latitude);
            sb.Append("+");
            sb.Append(from.Longitude);
            sb.Append("&daddr=");
            sb.Append(to.Latitude);
            sb.Append("+");
            sb.Append(to.Longitude);
            sb.Append("&dirflg=w");
        }

        Application.OpenURL(sb.ToString());
    }
}