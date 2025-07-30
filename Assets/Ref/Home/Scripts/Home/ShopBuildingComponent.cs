using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopBuildingComponent : MonoBehaviour
{
    [SerializeField] ShopBuildingType shopBuildingType;
    public ShopBuildingType ShopBuildingType => shopBuildingType;

    [SerializeField] private Animator animator;
    [SerializeField] private string clipAnimationName;
    [SerializeField] private GameObject fx;

    private void PlayAnimation(string clipName)
    {
        if (animator != null) animator.transform.gameObject.SetActive(true);
        if (animator != null && animator.HasState(0, Animator.StringToHash(clipName)))
        {
            animator.transform.DOScale(Vector3.one, 0.3f);
            if (string.IsNullOrEmpty(clipAnimationName))
            {
                animator.Play(clipName);
            }
            else
            {
                animator.Play(clipAnimationName);
            }

            Debug.Log($"Playing animation: {clipName}");
        }
        else
        {
            Debug.LogWarning($"Animation clip '{clipName}' not found.");
        }
    }

    public void Show()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.transform.localScale = Vector3.zero;

        gameObject.SetActive(true);
        gameObject.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.5f);
        PlayAnimation("Win");
        if (fx != null) fx.SetActive(true);
    }
}