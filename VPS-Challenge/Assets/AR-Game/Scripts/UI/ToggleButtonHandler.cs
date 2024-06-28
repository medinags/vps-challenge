using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class ToggleButtonHandler : MonoBehaviour
{
    [SerializeField] private bool isOn = false;

    [SerializeField] RectTransform uiHandleRectTransform;
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handleActiveColor;
    [SerializeField] TextMeshProUGUI textState;
    Image backgroundImage, handleImage;

    Color backgroundDefaultColor, handleDefaultColor;

    Vector2 handlePosition;

    public UnityEvent<bool> OnToggleChanged;

    // Start is called before the first frame update
    void Start()
    {
        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
        handleImage = uiHandleRectTransform.GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;
        if (isOn)
        {
            OnSwitch(isOn);
            OnOffText(isOn);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Toggle()
    {
        isOn = !isOn;
        OnToggleChanged?.Invoke(isOn);
    }

    public void OnSwitch(bool on)
    {
        uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);
        backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);
        handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .4f);
    }

    public void OnOffText(bool on)
    {
        if (isOn)
        {
            textState.text = "VPS ON";
        }
        else
        {
            textState.text = "VPS OFF";
        }
    }
}
