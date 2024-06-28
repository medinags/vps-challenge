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
    [SerializeField] private GameObject Helmet;

    private const string appleTag = "Apple";
    private const string powerUpTag = "PowerUp";
    private const string enemyTag = "Enemy";
    private const string powerUpApple = "GemPUApple";
    private const string powerUpDestroy = "GemPUDestroy";
    private const string powerUpGrass = "GemPUGrass";

    public bool hasImmudity;

    private const int LifeInApple = 3;

    private bool deathDueToFall;
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
        GamePlayManager.Instance.OnNewPowerUp += NewPowerUp;
    }



    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(this.transform.position.y) > 5 && !deathDueToFall)
        {
            deathDueToFall = true;
            GameManager.Instance.GameOver();
            SnakeControllerManager.Instance.snakeBody[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            SnakeControllerManager.Instance.gameObject.SetActive(false);
            Debug.Log("Down....");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case appleTag:
                CollisionWithApple(other.gameObject);
                break;
            case powerUpTag:
                CollisionWithPowerUp(other.gameObject);
                ARText.Instance.ShowSentence(true);
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case enemyTag:
                CollisionWithEnemy(collision.gameObject);
                break;
            default:
                break;
        }
    }


    private void CollisionWithApple(GameObject apple)
    {
        if (visibleBody < SnakeControllerManager.Instance.snakeBody.Count)
        {
            int indexApple = GamePlayManager.Instance.ActiveApples.IndexOf(apple);
            if (indexApple == -1)
            {
                Debug.Log("Bug -1");
                return;
            }

            GamePlayManager.Instance.ApplesCount++;
            GamePlayManager.Instance.CalculateScore();

            if (GamePlayManager.Instance.ApplesCount % LifeInApple == 0)
            {
                GamePlayManager.Instance.LifeCount++;
            }

    

            //RadarManager.Instance.needDetroyHelper = true;
            //RadarManager.Instance.DeleteHelper(apple);

            visibleBody++;

            GameObject bodyPart = SnakeControllerManager.Instance.snakeBody[visibleBody - 1].gameObject;
            bodyPart.transform.GetChild(0).gameObject.SetActive(true);

            bodyPart.gameObject.GetComponent<Rigidbody>().isKinematic = false; //?

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

                if (GamePlayManager.Instance.ApplesCount == item)
                {
                    Debug.Log("Spawn Power Up");

                    MeshingSpawner.Instance.SpawnPowerUp();
                }
            }

            //Destroy(apple);
            UIRadar.Instance.DeletePointer(apple);
            GameManager.Instance.AppleEaten();

        }
    }



    private void CollisionWithPowerUp(GameObject powerUp)
    {
        Debug.Log("Go Power Up: " + powerUp.name);

        //RadarManager.Instance.DeleteHelper(powerUp);


        PowerUpType powerUpType = powerUp.GetComponent<PowerUpHelper>().PowerUpType;
        GamePlayManager.Instance.NewPowerUp(powerUpType);
        if (powerUpType == PowerUpType.Immudity)
        {
            hasImmudity = true;
            Helmet.SetActive(hasImmudity);
        }

        //RadarManager.Instance.needDetroyHelper = true;
        UIRadar.Instance.DeletePointer(powerUp);
    }

    private void NewPowerUp(PowerUpType obj)
    {
        //hasImmudity = obj == PowerUpType.Immudity;
        //Helmet.SetActive(hasImmudity);
    }
    private void CollisionWithEnemy(GameObject enemy)
    {
        GameObject rootEnemy = enemy.transform.parent.gameObject;
        if (hasImmudity)
        {
       
            int indexEnemy = MeshingSpawner.Instance.currentEnemies.IndexOf(rootEnemy);
            MeshingSpawner.Instance.currentEnemies.RemoveAt(indexEnemy);
            Destroy(rootEnemy);
            GamePlayManager.Instance.PowerUpDestroyCount++;
            GamePlayManager.Instance.CalculateScore();

            hasImmudity = false;
            Helmet.SetActive(false);
            ARText.Instance.ShowSentence(true);
        }
        else
        {
            var dead = rootEnemy.GetComponent<DeathHandling>();
            dead.StartDeadTimer();
            ARText.Instance.ShowSentence(false);
        }

        GamePlayManager.Instance.CalculateScore();
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

public enum PowerUpType
{
    Apples,
    Immudity,
    Grass,
}