using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waitMeshesCanvas;
    [SerializeField] private GameObject tapToPlaceCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject powerUpCanvas;

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
                powerUpCanvas.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case PowerUpType.Immudity:
                break;
            case PowerUpType.Grass:
                break;
            default:
                break;
        }
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
        //pointsLifeCanvas.SetActive(true);
    }

    public void HidePowerUpApple()
    {
        powerUpCanvas.transform.GetChild(0).gameObject.SetActive(false);
        //powerUpCanvas.transform.GetChild(1).gameObject.SetActive(false);
        powerUpCanvas.SetActive(false);
    }
}
