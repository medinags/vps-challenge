using Niantic.Lightship.AR.LocationAR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.PersistentAnchors;
using Niantic.Lightship.AR.VpsCoverage;
public class VPSHandler : MonoBehaviour
{
    [SerializeField] private ARLocationManager arLocationManager;
    [SerializeField] private VPSCoverageController vpsCoverage;
    [SerializeField] private GameObject defaulAnchorPrefab;
    private GameObject arLocationHolder;
    private GameObject anchorObj;

    private Vector3 desiredAnchorPosition;
    private bool hasBeenDiscovered;
    void Start()
    {
        vpsCoverage.OnWayspotSelected += OnWayspot;
        arLocationManager.locationTrackingStateChanged += LocationTrackingStateChanged;

        arLocationHolder = new GameObject("ARLocation");
        // The ARLocation will be enabled once the anchor starts tracking.
        arLocationHolder.SetActive(false);
    }

    private void LocationTrackingStateChanged(ARLocationTrackedEventArgs obj)
    {
/*        if (!hasBeenDiscovered && obj.Tracking)
        {
            Vector3 pos = arLocationHolder.transform.InverseTransformPoint(desiredAnchorPosition);
            anchorObj.transform.localPosition = pos;
            hasBeenDiscovered = true;
        }*/
    }
    private void OnWayspot(LocalizationTarget target)
    {
       
        Debug.Log("New Location Added");
        if (String.IsNullOrEmpty(target.DefaultAnchor))
        {
            Debug.LogWarning("The selected location does not have a default anchor");
            return;
        }

        var arLocation = arLocationHolder.AddComponent<ARLocation>();
        arLocationHolder.transform.SetParent(arLocationManager.transform);
        
        //anchorObj = SpawnDefaultAnchor(false);

        arLocation.Payload = new ARPersistentAnchorPayload(target.DefaultAnchor);
        arLocationManager.SetARLocations(arLocation);
        arLocationManager.StartTracking();
        arLocationHolder.name = target.Name;
        vpsCoverage.DisableCoverage();

    }

    private GameObject SpawnDefaultAnchor(bool spawnInOrigin)
    {
        Vector3 pos = Vector3.zero;
        if (!spawnInOrigin)
        {
           pos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        }
        var anchordObj = Instantiate(defaulAnchorPrefab);
        anchordObj.transform.position = pos;
        anchordObj.transform.SetParent(arLocationHolder.transform);
        desiredAnchorPosition = pos;
        return anchordObj;
    }
}
