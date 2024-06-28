using Niantic.Lightship.AR.LocationAR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.PersistentAnchors;
public class VPSStateController : MonoBehaviour
{
    public static VPSStateController Instance;

    [SerializeField] private ARLocationManager locationManager;
    public bool FirstTrackingUpdateReceived;
    public string CurrentVPSLocationName;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
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

        if (!FirstTrackingUpdateReceived && isTracking)
        {
            Debug.Log("First VPS tracking update received");
            FirstTrackingUpdateReceived = true;
            CurrentVPSLocationName = args.ARLocation.name;
            GameManager.Instance.LocationFound();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
