using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using System;
using Niantic.Lightship.AR.Semantics;

public class ScreenTouchSpawner : MonoBehaviour
{
    [SerializeField] private string[] allowTapOverCanvas = { "TapToPlaceCanvas", "PowerUpCanvas", };

    [SerializeField] private GameObject snakeEggPrefab;
    [SerializeField] private ARSemanticSegmentationManager semanticManager;

    private bool hasToSpawnEgg = true;
    private bool ReadTouchs;
    void Start()
    {
        GameManager.Instance.OnMinimumMeshesFound += EnableTouchReader;
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
                if (hasToSpawnEgg)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit) && IsTouchOverGround(touch))
                    {
                        hasToSpawnEgg = false;
                        GameManager.Instance.EggLaid();
                        GameObject egg = Instantiate(snakeEggPrefab, hit.point, Quaternion.identity);
                        egg.GetComponent<EggController>().Snake = GameManager.Instance.SnakeManager;
                    }
                }
      
            }

         }
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
