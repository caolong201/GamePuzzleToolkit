using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderPanel : MonoBehaviour
{
    [SerializeField] private Slider sdProgress;
    [SerializeField] private TextMeshProUGUI txtProgress;

    
    
    //Coin
    [SerializeField] private TextMeshProUGUI txtCoin;
    private int totalCoinOwner = 5599;

    private void Start()
    {
        sdProgress.fillRect.transform.SetActive(false);
        txtCoin.text = totalCoinOwner.ToString("N0");
    }

    public void UpdateProgress(float currentProgress, float totalProgress)
    {
        sdProgress.fillRect.transform.SetActive(true);
        txtProgress.transform.DOKill();
        txtProgress.transform.DOPunchScale(new Vector3(0,2f,0f), 0.2f);
        txtProgress.text = $"{currentProgress}/{totalProgress}";
        sdProgress.DOValue((currentProgress / totalProgress), 0.2f)
            .SetEase(Ease.InOutQuad);
        
    }

    public void Buy(int amount)
    {
        int cacheCurrAmount = totalCoinOwner;
        int targetAmount = totalCoinOwner - amount;
        DOTween.To(() => totalCoinOwner, x => totalCoinOwner = x, targetAmount, 1f) //
            .OnUpdate(() =>
            {
                txtCoin.text = totalCoinOwner.ToString("N0");
            })
            .OnComplete(() =>
            {
                Color targetColor = targetAmount > cacheCurrAmount ? Color.green : Color.red;
                txtCoin.DOColor(targetColor, 0.5f).OnComplete(() =>
                {
                    txtCoin.DOColor(new Color(118 / 255f, 53 / 255f, 0), 0.5f);
                });
            });
    }
}