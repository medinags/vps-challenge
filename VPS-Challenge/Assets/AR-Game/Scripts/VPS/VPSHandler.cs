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
    [SerializeField] private VPSCoverageControler vpsCoverage;
    private GameObject arLocationHolder;
    // Start is called before the first frame update
    void Start()
    {
        vpsCoverage.OnWayspotDefaultAnchorButtonPressed += OnWayspot;
        arLocationManager.locationTrackingStateChanged += LocationTrackingStateChanged;

        arLocationHolder = new GameObject("ARLocation");
        // The ARLocation will be enabled once the anchor starts tracking.
        arLocationHolder.SetActive(false);
    }

    private void LocationTrackingStateChanged(ARLocationTrackedEventArgs obj)
    {
        Debug.Log(obj.Tracking);
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

        arLocation.Payload = new ARPersistentAnchorPayload(target.DefaultAnchor);
        arLocationManager.SetARLocations(arLocation);
        arLocationManager.StartTracking();
        arLocationHolder.name = target.Name;
        vpsCoverage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
