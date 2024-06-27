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
    public static MeshingSpawner Instance;

    [Header("AR References")]
    [SerializeField] private XROrigin origin;
    [SerializeField] private ARMeshManager meshManager;
    [SerializeField] private GameObject meshRoot;
    [SerializeField] GameObject SnakeManagerGO;

    [Header("Prefabs")]
    [SerializeField] private GameObject apple;
    [SerializeField] private GameObject grass;
    public List<GameObject> Containers = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyBlocks = new List<GameObject>();
    [SerializeField] private List<GameObject> powersUp = new List<GameObject>();

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
    [HideInInspector]
    public List<GameObject> currentEnemies = new List<GameObject>();
    private int grassQuantity = 0;
    private int grassPowerCount = 0;
    public bool IsReady = false;
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
        meshManager.meshesChanged += MeshesChanged;
        meshRoot = origin.TrackablesParent.gameObject;
        if (GameManager.Instance.UseVPS)
        {
            GameManager.Instance.OnLocationSelected += LocationSelected;
        }
  
        GameManager.Instance.OnSnakePlaced += SpawnFirstApple;
        GameManager.Instance.OnSnakePlaced += FindFloor;
        GameManager.Instance.OnappleEaten += AppleEaten;
        GamePlayManager.Instance.OnNewPowerUp += NewPowerUp;
        
    }

    private void LocationSelected()
    {
        meshManager.gameObject.SetActive(true);
    }

    public void EnableMshing()
    {
       
        meshManager.gameObject.SetActive(!GameManager.Instance.UseVPS);
    }

    private void NewPowerUp(PowerUpType obj)
    {
        if (obj.Equals(PowerUpType.Grass))
        {
            grassQuantity = Random.Range(4, 10);
            grassPowerCount = 0;
            GrowGrass();
            UIManager.Instance.ShowPowerUpGrass(grassQuantity);
        }
    }

    private void AppleEaten()
    {
        FindFloor();
        GrowApples();
        SpawnEnemy();
    }


    public void GrowApples()
    {
        if (!GamePlayManager.Instance.ShouldSpawnApples)
            return;

        float offset = appleOffset + apple.transform.localScale.y / 2;
        var newApple = GenerateItem(offset, apple);

        if (newApple != null)
        {

            if (currentEnemies.Count == 0)
            {
                GamePlayManager.Instance.ActiveApples.Add(newApple);
                //RadarManager.Instance.GenerateHelper(newApple);

                UIRadar.Instance.GeneratePointer(newApple, Color.red);

            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, newApple.transform.position);
                    if (dis < minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        apple.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                GamePlayManager.Instance.ActiveApples.Add(newApple);
                //RadarManager.Instance.GenerateHelper(newApple);
                UIRadar.Instance.GeneratePointer(newApple, Color.red);
            }

            Debug.Log("New Apple");
        }
        else
        {
            Invoke(nameof(GrowApples), 1.0f);
        }
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
            IsReady = true;
            meshTimer.Stop();
            GameManager.Instance.MinimumMeshesFound();
        }

        foreach (Transform child in meshRoot.transform)
        {
            SaveBlocks(child.gameObject);
        }
    }

    public void SpawnPowerUp()
    {
        Debug.Log("Power up: " + powersUp.Count);
        GameObject powerUpPrefab = powersUp[Random.Range(0, powersUp.Count)];
        GameObject powerUpGO = GenerateItem(0, powerUpPrefab);

        Debug.Log(powerUpPrefab.name);

        if (powerUpGO != null)
        {
            powerUpGO.name = powerUpPrefab.name;

            if (currentEnemies.Count == 0)
            {
                //RadarManager.Instance.GenerateHelper(powerUpGO);

                UIRadar.Instance.GeneratePointer(powerUpGO, Color.blue);
            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, powerUpGO.transform.position);
                    if (dis < minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        apple.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                Debug.Log("New Power Up");
                //RadarManager.Instance.GenerateHelper(powerUpGO);
                UIRadar.Instance.GeneratePointer(powerUpGO, Color.blue);
            }
        }
        else
        {
            Debug.Log("Try again Power Up");
            Invoke(nameof(SpawnPowerUp), 0.5f);
        }
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

    private void SpawnFirstApple()
    {
        FirstApple(SnakeManager.Instance.CurrentMesh);
    }

    public void FirstApple(Mesh mesh)
    {
        int vertex = Random.Range(0, mesh.vertexCount);

        Vector3 normal = mesh.normals[vertex];
        bool floorPart = Mathf.Abs(normal.y) >= 1.0f - groundTolerance;
        if (floorPart)
        {
            Vector3 position = mesh.vertices[vertex];

            Vector3 globalPos = transform.TransformPoint(position);
            position = globalPos + Vector3.up * origin.CameraYOffset;

            position = new Vector3(position.x, position.y + appleOffset + apple.transform.localScale.y / 2, position.z);
            var newApple = Instantiate(apple, position, Quaternion.identity);
            newApple.name = "1";
            //RadarManager.Instance.GenerateHelper(newApple);
            UIRadar.Instance.GeneratePointer(newApple, Color.red);
            GamePlayManager.Instance.ActiveApples.Add(newApple);
            Debug.Log("New apple");
        }
        else
        {
            FirstApple(mesh);
            Debug.Log("No Valit Vertex, Recalculating...");
        }
    }

    private GameObject GenerateItem(float offsetFloor, GameObject spawnObject)
    {
        var randomPosition = Random.Range(0, FloorMeshes.Count);
        GameObject floorBlock = FloorMeshes[randomPosition];

        if (floorBlock != null)
        {
            var mesh = floorBlock.GetComponent<MeshFilter>().sharedMesh;
            int vertex = Random.Range(0, mesh.vertexCount);

            Vector3 normal = mesh.normals[vertex];
            bool floorPart = Mathf.Abs(normal.y) >= 1.0f - groundTolerance;


            if (floorPart)
            {
                Vector3 position = mesh.vertices[vertex];

                Vector3 globalPos = transform.TransformPoint(position);
                position = globalPos + Vector3.up * origin.CameraYOffset;

                //bool isSnakeLevel = SnakeManagerGO.transform.position.y;

                position = new Vector3(position.x, position.y + offsetFloor, position.z);
                var newApple = Instantiate(spawnObject, position, Quaternion.identity);

                return newApple;
            }
            else
            {
                Debug.Log("No Valit Vertex, Recalculating...");
                return null;

            }
        }
        else
        {
            return null;
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemyPrefab = enemyBlocks[Random.Range(0, enemyBlocks.Count)];
        GameObject enemyBlock = GenerateItem(0, enemyPrefab);

        if (enemyBlock != null)
        {

            if (currentEnemies.Count == 0)
            {
                enemyBlock.name = enemyPrefab.name + currentEnemies.Count;
                currentEnemies.Add(enemyBlock);
                enemyBlock.transform.parent = Containers[0].gameObject.transform;
            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, enemyBlock.transform.position);
                    if (dis < minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        enemyBlock.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                enemyBlock.transform.parent = Containers[0].gameObject.transform;
                enemyBlock.name = enemyPrefab.name + currentEnemies.Count;
                currentEnemies.Add(enemyBlock);
                Debug.Log("New Block Enemy");
            }

        }
        else
        {
            Invoke(nameof(SpawnEnemy), 1.0f);
        }
    }

    public void GrowGrass()
    {
        GameObject powerUpGrass = GenerateItem(0, grass);

        if (powerUpGrass != null)
        {
            powerUpGrass.name = grass.name;
            GrassController startGrass = powerUpGrass.transform.GetChild(0).GetComponent<GrassController>();
            startGrass.GrowGrass();
            GameObject explosion = powerUpGrass.transform.GetChild(1).gameObject;
            powerUpGrass.transform.parent = null;
            explosion.transform.localScale = Vector3.one;
            explosion.SetActive(true);
            powerUpGrass.transform.rotation = Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z);
            powerUpGrass.transform.parent = Containers[1].transform;
            StartCoroutine(SnakeManager.Instance.DestroyExplosion(explosion));

            if (grassPowerCount < grassQuantity)
            {
                Invoke(nameof(GrowGrass), 0.5f);
                grassPowerCount++;
            }
            else
            {
                GamePlayManager.Instance.IsPowerUpGrassActivated = false;
            }


        }
        else
        {
            Invoke(nameof(GrowGrass), 1);
        }
    }
}
