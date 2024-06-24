using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ARText : MonoBehaviour
{
    private GameObject SnakeHead;
    private bool shouldTrackSnake;
    [SerializeField] private GameObject ARCamera;
    [SerializeField] private float distanceSnake = 0.15f;
    [SerializeField] public List<string> Words = new List<string>();

    public static ARText Instance;
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
        GameManager.Instance.OnSnakeSpawen += FollowSanke;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldTrackSnake)
            return;
        FollowSanke();

    }

    private void FollowSanke()
    {
        SnakeHead = SnakeControllerManager.Instance.snakeBody[0].gameObject;
        transform.localPosition = new Vector3(SnakeHead.transform.position.x, SnakeHead.transform.position.y + distanceSnake, SnakeHead.transform.position.z);
        transform.LookAt(ARCamera.transform);
        shouldTrackSnake = true;

    }

    public void ShowSentence(bool isPowerUp)
    {
        //transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        if (isPowerUp)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Words[Random.Range(0, Words.Count - 1)];
        }
        else
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Words[Words.Count-1];
        }

        StartCoroutine(TurnOffSentence());
    }

    private IEnumerator TurnOffSentence() 
    {
        yield return new WaitForSeconds(1.5f);
        //transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
