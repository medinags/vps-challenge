using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject waitMeshesCanvas;
    [SerializeField] private GameObject tapToPlaceCanvas;
    [SerializeField] private GameObject controlsCanvas;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnMinimumMeshesFound += ShowTapToPlaceUI;
        GameManager.Instance.OnEggLaid += HideTapToPlaceUI;
        GameManager.Instance.OnSnakeBorn += ShowPrincipalMenu;
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
}
