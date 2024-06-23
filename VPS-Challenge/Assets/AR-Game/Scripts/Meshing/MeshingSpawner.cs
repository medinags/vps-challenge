using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Diagnostics;
using System;

public class MeshingSpawner : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private XROrigin origin;
    [SerializeField] private ARMeshManager meshManager;
    [SerializeField] private GameObject meshRoot;
    [SerializeField] GameObject SnakeManagerGO;

    [Header("Mesh Scan Parameters")]
    [SerializeField] private int minimumMeshBlocks = 30;
    [SerializeField] private int miniumTimeMesh = 5000;

    [Header("Object Generation Parameters")]
    [SerializeField] private float groundTolerance = 0.05f;
    [SerializeField] private float centerOffset = 0.15f;
    [SerializeField] private float appleOffset = 0.015f;
    [SerializeField] private float minDisEnemies = 0.4f;

    [SerializeField] private float snakeY;
    [SerializeField] private float yOffset = 0.05f;

    public List<GameObject> FloorMeshes = new List<GameObject>();
    [SerializeField] private float lowerLimit;
    [SerializeField] private float upperLimit;


    private Stopwatch meshTimer = new Stopwatch();
    private bool isTimerComplete;
    private bool isMinimulBlocks;
    private List<GameObject> meshBloks = new List<GameObject>();
    private Dictionary<GameObject, Vector3> BlockCenterDic = new Dictionary<GameObject, Vector3>();
    private List<float> meshCenters;

    // Start is called before the first frame update
    void Start()
    {
        meshManager.meshesChanged += MeshesChanged;
        meshRoot = origin.TrackablesParent.gameObject;

        GameManager.Instance.OnSnakePlaced += FindFloor;
    }

    private void MeshesChanged(ARMeshesChangedEventArgs obj)
    {
        if (meshRoot.transform.childCount >= minimumMeshBlocks && !isMinimulBlocks)
        {
            meshTimer.Start();
            isMinimulBlocks = true;
        }

        if (meshTimer.ElapsedMilliseconds >= miniumTimeMesh && !isTimerComplete)
        {
            isTimerComplete = true;
            GameManager.Instance.MinimumMeshesFound();
            meshTimer.Stop();
        }

        foreach (Transform child in meshRoot.transform)
        {
            SaveBlocks(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SaveBlocks(GameObject child)
    {
        if (child == null)
        {
            return;
        }

        var mesh = child.GetComponent<MeshCollider>();
        if (mesh == null || mesh.sharedMesh == null)
        {
            return;
        }

        var meshCenter = mesh.sharedMesh.bounds.center;
        Vector3 globalCenter = mesh.transform.TransformPoint(meshCenter);
        meshCenter = globalCenter;

        if (!meshBloks.Contains(child))
        {
            meshBloks.Add(child);
            BlockCenterDic.Add(child, meshCenter);
        }
        else
        {
            BlockCenterDic[child] = meshCenter;
        }
    }



    private void FindFloor()
    {
        meshCenters = new List<float>();
        snakeY = SnakeManagerGO.transform.position.y + yOffset;
        lowerLimit = snakeY - centerOffset;
        upperLimit = snakeY + centerOffset;

        //upper.position = new Vector3(SnakeManagerGO.transform.position.x, upperLimit, SnakeManagerGO.transform.position.y);
        //lower.position = new Vector3(SnakeManagerGO.transform.position.x, lowerLimit, SnakeManagerGO.transform.position.y);

        foreach (var pair in BlockCenterDic)
        {
            var key = pair.Key;

            if (key == null)
            {
                continue;
            }

            Vector3 centerPos = pair.Value;

            if (lowerLimit <= centerPos.y && centerPos.y <= upperLimit)
            {
                if (!FloorMeshes.Contains(key))
                {
                    meshCenters.Add(centerPos.y);
                    FloorMeshes.Add(key);
                }
            }
            else
            {
                if (FloorMeshes.Contains(key))
                {
                    FloorMeshes.Remove(key);
                }
            }
        }
    }
}
