using Niantic.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject arcadeCanvas;
    [SerializeField] private GameObject waitMeshesCanvas;
    [SerializeField] private GameObject tapToPlaceCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject powerUpCanvas;
    [SerializeField] private GameObject pointsLifeCanvas;
    [SerializeField] private GameObject radarCanvas;

    public event Action OnTapToPlace;
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
        GameManager.Instance.OnLocationFound += LocationFound;
        GameManager.Instance.OnMinimumMeshesFound += ShowTapToPlaceUI;
        GameManager.Instance.OnEggLaid += HideTapToPlaceUI;
        GameManager.Instance.OnSnakeBorn += ShowPrincipalMenu;
        GamePlayManager.Instance.OnNewPowerUp += NewPowerUp;
        startCanvas.SetActive(true);

    }

    private void LocationFound()
    {
        arcadeCanvas.SetActive(false);

        if (MeshingSpawner.Instance.IsReady)
        {
            Debug.Log("Meshing Ready....");
            ShowTapToPlaceUI();
        }
        else
        {
            waitMeshesCanvas.SetActive(true);
        }
    }

    private void NewPowerUp(PowerUpType obj)
    {
        powerUpCanvas.SetActive(true);

        switch (obj)
        {
            case PowerUpType.Apples:
                ShowPowerUpApple();
                break;
            case PowerUpType.Immudity:
                ShowPowerUpHelmet();
                break;
            case PowerUpType.Grass:
                break;
            default:
                break;
        }
    }

    public void HandleFristCanvas()
    {
        startCanvas.SetActive(false );
        if (GameManager.Instance.UseVPS)
        {
            arcadeCanvas.SetActive(true);
        }
        else
        {
            waitMeshesCanvas.SetActive(true);
        }
    }
    public void UpdatePoints(string points, string lifes)
    {
        pointsLifeCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = points;
        pointsLifeCanvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = lifes;
    }

    public void UpdateGameTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        string textTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        pointsLifeCanvas.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    private void ShowTapToPlaceUI()
    {
        if (!GameManager.Instance.UseVPS)
        {
            waitMeshesCanvas.SetActive(false);
            tapToPlaceCanvas.SetActive(true);
            OnTapToPlace?.Invoke();
        }
        else
        {
            Debug.Log("Use VPS...");
            if (VPSStateController.Instance.FirstTrackingUpdateReceived)
            {
                Debug.Log("VPS Ready....");
                waitMeshesCanvas.SetActive(false);
                tapToPlaceCanvas.SetActive(true);
                OnTapToPlace?.Invoke();
            }
            else
            {
                Debug.Log("VPS No Ready....");
            }
        }


    }
    private void HideTapToPlaceUI()
    {
        tapToPlaceCanvas.SetActive(false);
    }
    private void ShowPrincipalMenu()
    {
        tapToPlaceCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        pointsLifeCanvas.SetActive(true);
        radarCanvas.SetActive(true);
    }

    public void HidePowerUpApple()
    {
        powerUpCanvas.transform.GetChild(0).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(1).gameObject.SetActive(false);
        powerUpCanvas.SetActive(false);
    }
    public void ShowPowerUpApple()
    {

        powerUpCanvas.transform.GetChild(2).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(3).gameObject.SetActive(false);
        powerUpCanvas.SetActive(true);
        powerUpCanvas.transform.GetChild(0).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(1).gameObject.SetActive(true);

    }
    public void ShowPowerUpGrass(int grassQuantity)
    {
        powerUpCanvas.transform.GetChild(2).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(3).gameObject.SetActive(false);
        powerUpCanvas.SetActive(true);
        powerUpCanvas.transform.GetChild(4).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(5).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = "X" + grassQuantity;

        StartCoroutine(HideGrassHelmentUI(4, 5));
    }

    public void ShowPowerUpHelmet()
    {
        powerUpCanvas.SetActive(true);
        powerUpCanvas.transform.GetChild(2).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(3).gameObject.SetActive(true);
        StartCoroutine(HideGrassHelmentUI(2, 3));
    }

    IEnumerator HideGrassHelmentUI(int GOone, int GOtwo)
    {
        yield return new WaitForSeconds(2.0f);
        powerUpCanvas.transform.GetChild(GOone).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(GOtwo).gameObject.SetActive(false);
        powerUpCanvas.SetActive(false);

    }
}
