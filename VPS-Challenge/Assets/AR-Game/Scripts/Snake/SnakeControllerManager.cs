using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerManager : MonoBehaviour
{
    public static SnakeControllerManager Instance;

    [Header("Control Parameters")]
    [SerializeField] private float speed = 2;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] float disteanceBetween = 0.05f;
    

    [Header("Prefabs")]
    [SerializeField] private List<GameObject> bodyPartPrefabs = new List<GameObject>();
    public List<SnakeBodyPartManager> snakeBody = new List<SnakeBodyPartManager>();

    private bool turnLeft;
    private bool turnRight;
    private bool shouldStop;

    private int maxBodyParts = 50;
    float contUp = 0;

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
        CreateBodyParts();
        //SnakeManager.Instance.OnDownSnake += ActivateReset;
    }

    private void FixedUpdate()
    {
        if (snakeBody.Count < maxBodyParts)
        {
            InitalizeBody();
        }
        SnakeMovement();
    }

    private void SnakeMovement()
    {
        snakeBody[0].transform.position += snakeBody[0].transform.forward * speed * Time.deltaTime;

        if (turnRight)
        {
            snakeBody[0].transform.rotation = Quaternion.Euler(0, snakeBody[0].transform.rotation.eulerAngles.y, 0);
            snakeBody[0].transform.Rotate(new Vector3(0, 1, 0) * turnSpeed * Time.deltaTime);
        }
        else if (turnLeft)
        {
            snakeBody[0].transform.rotation = Quaternion.Euler(0, snakeBody[0].transform.rotation.eulerAngles.y, 0);
            snakeBody[0].transform.Rotate(new Vector3(0, -1, 0) * turnSpeed * Time.deltaTime);
        }

        if (snakeBody.Count > 1)
        {

            for (int i = 1; i < snakeBody.Count; i++)
            {
                SnakeBodyPartManager snakeManager = snakeBody[i - 1];
                snakeBody[i].transform.position = snakeManager.snakes[0].position;
                snakeBody[i].transform.rotation = snakeManager.snakes[0].rotation;
                snakeManager.snakes.RemoveAt(0);
            }
        }
    }

    private void InitalizeBody()
    {
        CreateBodyParts();
    }

    private void CreateBodyParts()
    {

        if (snakeBody.Count == 0)
        {
            GameObject snakeHead = Instantiate(bodyPartPrefabs[0],
            transform.position + Vector3.up * 0.045f, Quaternion.Euler(0, 180, 0), transform);
            SnakeBodyPartManager Controller = snakeHead.AddComponent<SnakeBodyPartManager>();
            snakeBody.Add(Controller);
            GameManager.Instance.SnakeSpawen();
        }

        SnakeBodyPartManager bodySnake = snakeBody[snakeBody.Count - 1];

        if (contUp == 0)
        {
            bodySnake.ClearSnake();
        }

        contUp += Time.deltaTime;

        if (contUp >= disteanceBetween)
        {
            int parts = snakeBody.Count;
            GameObject part = (parts % 2 == 0) ? bodyPartPrefabs[1] : bodyPartPrefabs[2];
            GameObject newPart = Instantiate(part, transform.position, transform.rotation, transform);

            newPart.AddComponent<SnakeBodyPartManager>();
            newPart.AddComponent<Rigidbody>();

            snakeBody.Add(newPart.GetComponent<SnakeBodyPartManager>());


            if (snakeBody.Count > SnakeManager.Instance.visibleBody)
            {
                newPart.transform.GetChild(0).gameObject.SetActive(false);
                newPart.GetComponent<Rigidbody>().isKinematic = true;
            }

            newPart.GetComponent<SnakeBodyPartManager>().ClearSnake();
            contUp = 0;
        }
    }

    public void TurnRight(bool shouldTurn)
    {
        turnRight = shouldTurn;
    }
    public void TurnLeft(bool shouldTurn)
    {
        turnLeft = shouldTurn;
    }
}
