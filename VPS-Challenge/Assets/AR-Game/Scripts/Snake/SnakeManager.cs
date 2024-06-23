using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{

    public static SnakeManager Instance;

    public Mesh CurrentMesh;
    public GameObject currenMeshContainer;
    public bool justSpawned = true;
    public int visibleBody = 1;

    private const string appleTag = "Apple";
    private const string powerUpTag = "PowerUp";
    private const string enemyTag = "Enemy";
    private const string powerUpApple = "GemPUApple";
    private const string powerUpDestroy = "GemPUDestroy";
    private const string powerUpGrass = "GemPUGrass";

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case appleTag:
                CollisionWithApple(other.gameObject);
                break;
            default:
                break;
        }
    }

    private const int LifeInApple = 3;

    private void CollisionWithApple(GameObject apple)
    {
        if (visibleBody < SnakeControllerManager.Instance.snakeBody.Count)
        {
            GamePlayManager.Instance.AppleCount++;
            GamePlayManager.Instance.PointCounts();

            if (GamePlayManager.Instance.AppleCount % LifeInApple == 0)
            {
                GamePlayManager.Instance.LifeCount++;
            }

            //RadarManager.instance.needDetroyHelper = true;
            //RadarManager.instance.DeleteHelper(other.gameObject);

            visibleBody++;

            GameObject bodyPart = SnakeControllerManager.Instance.snakeBody[visibleBody - 1].gameObject;
            bodyPart.transform.GetChild(0).gameObject.SetActive(true);

            bodyPart.gameObject.GetComponent<Rigidbody>().isKinematic = false; //?

            Debug.Log(apple.name);
            int indexApple = GamePlayManager.Instance.ActiveApples.IndexOf(apple);
            Debug.Log(indexApple);
            GamePlayManager.Instance.ActiveApples.RemoveAt(indexApple);

            //Grass
            GrassController grass = apple.transform.GetChild(1).GetComponent<GrassController>();
            apple.transform.GetChild(1).transform.parent = null;
            grass.transform.parent = MeshingSpawner.Instance.Containers[1].transform;


            //Explosion
            GameObject explosion = apple.transform.GetChild(2).gameObject;
            apple.transform.GetChild(2).transform.parent = null;
            explosion.transform.localScale = Vector3.one;
            explosion.SetActive(true);
            StartCoroutine(DestroyExplosion(explosion));

            //Grass
            grass.gameObject.transform.localScale = Vector3.one * 0.5f;
            grass.gameObject.transform.rotation = Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z);
            grass.enabled = true;
            grass.GrowGrass();

            if (GamePlayManager.Instance.ActiveApples.Count == 0)
            {
                GamePlayManager.Instance.ShouldSpawnApples = true;
            }
            else
            {
                GamePlayManager.Instance.ShouldSpawnApples = false;
            }


            foreach (var item in GamePlayManager.Instance.PowerUpIn)
            {

                if (GamePlayManager.Instance.AppleCount == item)
                {
                    Debug.Log("Spanw Power Up");

                    //ObjectProviderManager.Instance.SpawnPowerUp();
                }
            }

            Destroy(apple);
            GameManager.Instance.AppleEaten();

        }
    }
    private void OnCollisionStay(Collision collision)
    {
        var meshFilter = collision.gameObject.GetComponent<MeshFilter>();

        if (meshFilter)
        {
            CurrentMesh = meshFilter.sharedMesh;
            currenMeshContainer = collision.gameObject;
            if (justSpawned)
            {
                GameManager.Instance.SnakePlaced();
                justSpawned = false;
            }
        }
    }

    public IEnumerator DestroyExplosion(GameObject Explosion)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(Explosion);
    }
}
