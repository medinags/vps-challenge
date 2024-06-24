using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waitMeshesCanvas;
    [SerializeField] private GameObject tapToPlaceCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject powerUpCanvas;
    [SerializeField] private GameObject pointsLifeCanvas;
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
        GameManager.Instance.OnMinimumMeshesFound += ShowTapToPlaceUI;
        GameManager.Instance.OnEggLaid += HideTapToPlaceUI;
        GameManager.Instance.OnSnakeBorn += ShowPrincipalMenu;
        GamePlayManager.Instance.OnNewPowerUp += NewPowerUp;

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

    public void UpdatePoints(string points, string lifes)
    {
        pointsLifeCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = points;
        pointsLifeCanvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = lifes;
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void ShowTapToPlaceUI()
    {
        waitMeshesCanvas.SetActive(false);
        tapToPlaceCanvas.SetActive(true);
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
