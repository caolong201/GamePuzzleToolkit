using System.Collections.Generic;
using UnityEngine;

public class FoodSpot : MonoBehaviour
{
    public bool IsHadFood {  get; set; }
    [SerializeField] GameObject coinIcon;
    [SerializeField] Transform topSpot;

    public void FlyingCoinIcon()
    {
        Vector3 targetPosition = topSpot.position;
        //Debug.Log(targetPosition);
        VFXAnimationManager.Instance.MovingToTarget(coinIcon, targetPosition, 3f);
    }
}
