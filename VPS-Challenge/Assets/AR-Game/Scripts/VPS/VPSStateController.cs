using Niantic.Lightship.AR.LocationAR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.PersistentAnchors;
public class VPSStateController : MonoBehaviour
{
    [SerializeField] private ARLocationManager locationManager;
    [SerializeField] private bool firstTrackingUpdateReceived;
    // Start is called before the first frame update
    void Start()
    {
        locationManager.locationTrackingStateChanged += TrackingStateChanged;
        locationManager.arPersistentAnchorStateChanged += PersistentAnchorStateChanged;
    }

    private void PersistentAnchorStateChanged(ARPersistentAnchorStateChangedEventArgs obj)
    {
        //Debug.Log(obj.arPersistentAnchor.gameObject.name);
        //Debug.Log(obj.arPersistentAnchor.trackingState);

    }

    private void TrackingStateChanged(ARLocationTrackedEventArgs args)
    {
        ARLocation trackedLocation = args.ARLocation;
        bool isTracking = args.Tracking;

        if (!firstTrackingUpdateReceived && isTracking)
        {
            Debug.Log("First tracking update received");
            firstTrackingUpdateReceived = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
