using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShopComplete : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtComplete;
    [SerializeField] GameObject effects;

    public void CompleteShop()
    {
        gameObject.SetActive(true);
        effects.SetActive(true);
        txtComplete.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), .2f);
    }

    public void Next()
    {
        gameObject.SetActive(false);
        effects.SetActive(false);
    }
}