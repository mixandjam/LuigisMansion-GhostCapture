using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    public bool active;
    public TextMeshProUGUI energyText;

    public static CanvasManager instance;
    public Vector3 offset;

    private void Awake()
    {
        instance = this;
        energyText.color = Color.clear;
    }

    public void UpdateText(Vector3 position, string amount)
    {
        energyText.transform.position = Camera.main.WorldToScreenPoint(position) + offset;
        energyText.text = amount;
    }

    public void ShowText(bool state)
    {
        energyText.DOColor(state ? Color.white : Color.clear, .2f);
    }



}
