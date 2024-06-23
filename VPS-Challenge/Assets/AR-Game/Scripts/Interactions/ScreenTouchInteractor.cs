using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class ScreenTouchInteractor : MonoBehaviour
{
    private bool hasToSpawnEgg = true;
    private string[] allowTapOverCanvas = { "TapToPlaceCanvas", "PowerUpCanvas", };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

                    if (Physics.Raycast(ray, out hit))
                    {
                        string objectName = hit.collider.gameObject.name;
                        Debug.Log("Touched object: " + objectName);
                        hasToSpawnEgg = false;
                        GameManager.Instance.EggLaid();
                        //TODO Implement egg
                        GameManager.Instance.SnakeBorn();
                        GameManager.Instance.SnakeManager.transform.position = hit.point;
                        GameManager.Instance.SnakeManager.SetActive(true);

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
}
