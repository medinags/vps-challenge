using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using System;
using Niantic.Lightship.AR.Semantics;
using Random = UnityEngine.Random;

public class ScreenTouchSpawner : MonoBehaviour
{
    [SerializeField] private string[] allowTapOverCanvas = { "TapToPlaceCanvas", "PowerUpCanvas", };

    [SerializeField] private GameObject snakeEggPrefab;
    [SerializeField] private GameObject applePrefab;

    [SerializeField] private ARSemanticSegmentationManager semanticManager;

    private bool hasToSpawnEgg = true;
    public bool ReadTouchs;
    public float launchForce = 200;
    void Start()
    {
        //GameManager.Instance.OnMinimumMeshesFound += EnableTouchReader;
        UIManager.Instance.OnTapToPlace += EnableTouchReader;
        GameManager.Instance.OnEggLaid += DisableToucReader;
        GamePlayManager.Instance.OnNewPowerUp += NewPowerUp;
    }

    private void NewPowerUp(PowerUpType obj)
    {
        if (ReadTouchs && GamePlayManager.Instance.IsPowerUpAppleActivated)
        {
            return;
        }
        ReadTouchs = obj.Equals(PowerUpType.Apples);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ReadTouchs)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 touch = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

            bool isTouchOverUI = IsTouchOverNotAllowUI(touch);

            if (!isTouchOverUI)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    bool isTouchOverGround = IsTouchOverGround(touch);

                    if (hasToSpawnEgg && isTouchOverGround)
                    {
                        hasToSpawnEgg = false;
                        GameManager.Instance.EggLaid();
                        GameObject egg = Instantiate(snakeEggPrefab, hit.point, Quaternion.identity);
                        egg.GetComponent<EggController>().Snake = GameManager.Instance.SnakeManager;

                    }
                    else if (GamePlayManager.Instance.IsPowerUpAppleActivated)
                    {
                        ReadTouchs = false;
                        GameObject container = SpawnPowerUpApples();

                        Vector3 entrancePoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, touch.y, Camera.main.nearClipPlane));
                        Rigidbody rb = container.GetComponent<Rigidbody>();
                        rb.velocity = new Vector3(0f, 0f, 0f);
                        rb.angularVelocity = new Vector3(0f, 0f, 0f);

                        container.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                        container.transform.position = entrancePoint;

                        float force = 200.0f;
                        rb.AddForce(Camera.main.transform.forward * force);
                        GamePlayManager.Instance.IsPowerUpAppleActivated = false;
                        UIManager.Instance.HidePowerUpApple();

                    }

                }

            }

        }
    }

    private GameObject SpawnPowerUpApples()
    {
        int quantity = Random.Range(2, 5);
        UIManager.Instance.SetAppleQuantity(quantity);
        GameObject appleContainer = new GameObject();
        appleContainer.transform.position = Camera.main.transform.position;
        
        appleContainer.AddComponent<Rigidbody>();
        for (int i = 0; i < quantity; i++)
        {
            GameObject apple = Instantiate(applePrefab, appleContainer.transform);
            float randomValueX = Random.Range(0.1f, 0.5f);
            float randomValueZ = Random.Range(0.1f, 0.5f);
            apple.transform.localPosition = new Vector3(randomValueX,0,randomValueZ);
            apple.GetComponent<MeshCollider>().enabled = true;
            apple.AddComponent<AppleFromPowerUp>();
        }

        return appleContainer;

    }

    private bool IsTouchOverNotAllowUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);

        if (result.Count > 0)
        {
            for (int i = 0; i < allowTapOverCanvas.Length; i++)
            {
                if (result[0].gameObject.transform.parent.gameObject.name == allowTapOverCanvas[i])
                {
                    return false;
                }
            }

        }

        return result.Count > 0;
    }

    private void EnableTouchReader()
    {
        ReadTouchs = true;
    }

    private void DisableToucReader()
    {
        ReadTouchs = false;
    }

    private bool IsTouchOverGround(Vector2 touchPos)
    {
        var list = semanticManager.GetChannelNamesAt((int)touchPos.x, (int)touchPos.y);

        if (list.Count > 0)
        {
            var channel = list[0];
            if (channel.Equals("ground"))
            {
                Debug.Log(channel);
                return true;
            }

        }
        return false;
    }
}
