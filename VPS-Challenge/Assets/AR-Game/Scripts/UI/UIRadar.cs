using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIRadar : MonoBehaviour
{
    public static UIRadar Instance;
    [SerializeField] private GameObject SnakeHead;
    [SerializeField] private Transform BaseContainer;
    [SerializeField] private Transform PointersContainer;
    [SerializeField] private bool shouldTrackSnake;
    [SerializeField] private GameObject pointerPrefab;
    [SerializeField] private float minDistance = 1.0f;
    [SerializeField] private Transform headTracker;
    [SerializeField] private RectTransform radarArea;
    [SerializeField] private float radarSize;
    public List<Pointer> pointers = new List<Pointer>();

   

    [Serializable]
    public class Pointer
    {
        public GameObject PointerGO;
        public GameObject TargetGO;
        public bool isVisible;
    }

    public bool needDetroyHelper;

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
        radarSize = radarArea.rect.width/2;
        GameManager.Instance.OnSnakeSpawen += StartRadar;
    }

    private void StartRadar()
    {
        SnakeHead = SnakeControllerManager.Instance.snakeBody[0].gameObject;
        shouldTrackSnake = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldTrackSnake)
            return;
        RotateWithCamera();
        UpdatePointers();
    }

    private void RotateWithCamera()
    {
        PointersContainer.transform.localRotation = Quaternion.Euler(0, 0,
             SnakeHead.transform.rotation.eulerAngles.y);
        headTracker.transform.position = SnakeHead.transform.position;

    }

    public void GeneratePointer(GameObject target, Color color)
    {
        if (pointers.Count == 0)
        {
            GameObject pointerGO = Instantiate(pointerPrefab, PointersContainer);
            pointerGO.SetActive(true);
            pointerGO.GetComponent<Image>().color = color;
            Pointer pointer = new Pointer();

            pointer.TargetGO = target;
            pointer.PointerGO = pointerGO;
            pointer.isVisible = true;
            pointers.Add(pointer);
        }
        else
        {
            if (CheckNull())
            {
                for (int i = 0; i < pointers.Count; i++)
                {
                    if (pointers[i].TargetGO == null)
                    {
                        pointers[i].TargetGO = target;
                        pointers[i].isVisible = true;
                        pointers[i].PointerGO.SetActive(true);
                        pointers[i].PointerGO.GetComponent<Image>().color = color;
                    }
                }
            }
            else
            {
                GameObject pointerGO = Instantiate(pointerPrefab, PointersContainer);
                pointerGO.SetActive(true);
                pointerGO.GetComponent<Image>().color = color;
                Pointer pointer = new Pointer();
                pointer.TargetGO = target;
                pointer.PointerGO = pointerGO;
                pointer.isVisible = true;
                pointers.Add(pointer);
            }
        }
 
    }

    public void DeletePointer(GameObject target)
    {
        needDetroyHelper = true;
        int count = pointers.Count;
        for (int i = 0; i < count; i++)
        {
            if (pointers[i].TargetGO == target)
            {
                pointers[i].TargetGO = null;
                pointers[i].isVisible = false;
                pointers[i].PointerGO.SetActive(false);
            }
        }

        Destroy(target);
        needDetroyHelper = false;
    }

    private void UpdatePointers()
    {
        if (pointers.Count == 0)
        {
            return;
        }

        foreach (var pointer in pointers)
        {
            if (!pointer.isVisible)
            {
                continue;
            }

            if (needDetroyHelper)
                return;

            float distance = Vector3.Distance(SnakeHead.transform.position, pointer.TargetGO.transform.position);
            float tLine;
            float scaleFactor;
            if (distance > minDistance)
            {
                tLine = minDistance / distance;
                scaleFactor = 0.75f;
            }
            else
            {
                tLine = distance;
                scaleFactor = 0.25f;
            }
            Vector3 pos = Calculate(SnakeHead.transform.position, pointer.TargetGO.transform.position, tLine);
            Vector3 tPos = headTracker.transform.InverseTransformPoint(pos);
            pointer.PointerGO.transform.localPosition = new Vector3(tPos.x*radarSize, tPos.z *radarSize, 0);
            pointer.PointerGO.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    private bool CheckNull()
    {
        int count = pointers.Count;
        for (int i = 0; i < count; i++)
        {
            if (pointers[i].TargetGO == null)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 Calculate(Vector3 pOne, Vector3 pTwo, float t)
    {
       
        var mx = pTwo.x - pOne.x;
        var my = pTwo.y - pOne.y;
        var mz = pTwo.z - pOne.z;

        var x = pOne.x + mx * t;
        var y = pOne.y + my * t;
        var z = pOne.z + mz * t;

        return new Vector3(x, y, z);
    }  
}
