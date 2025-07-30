using System;
using System.Collections;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    FoodSpot foodSpot;
    [SerializeField] float moveTime = 0.5f;
    [SerializeField] FoodStage foodStage = FoodStage.None;
    Transform despawnFoodPoint;
    public FoodControllerKey Key { get; set; } 

    public void SetFoodSpot(FoodSpot foodSpot, Transform transform, int id, int quantity)
    {
        Key = new FoodControllerKey(id, quantity);
        despawnFoodPoint = transform;
        this.foodSpot = foodSpot;
        this.transform.parent = foodSpot.transform;

        ChangeState(FoodStage.OnWaitingToBill);
    }

    private IEnumerator LerpPosition(Vector3 target, float duration, bool needToRemoveObjectPool = false)
    {
        Vector3 startPosition = transform.position;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        transform.position = target;
        yield return null;
        if (needToRemoveObjectPool)
        {
            DayManager.Instance.FoodObjectPool.RemoveFood(this);
        }
    }

    public void ChangeState(FoodStage newState)
    {
        if (newState == foodStage) return;
        ExitCurrentState();
        foodStage = newState;
        EnterNewState();
    }

    private void EnterNewState()
    {
        switch (foodStage)
        {
            case FoodStage.None:

                break;
            case FoodStage.OnWaitingToBill:
                StartCoroutine(LerpPosition(foodSpot.transform.position, moveTime));
                break;
            case FoodStage.OnBilled:
                transform.parent = despawnFoodPoint;
                foodSpot.IsHadFood = false;
                foodSpot.FlyingCoinIcon();
                StartCoroutine(LerpPosition(despawnFoodPoint.position, moveTime, true));
                break;
            default:
                break;
        }
    }

    private void ExitCurrentState()
    {
        switch (foodStage)
        {
            case FoodStage.None:

                break;
            case FoodStage.OnWaitingToBill:

                break;
            case FoodStage.OnBilled:

            default:
                break;
        }
    }

    public void Reset()
    {
        foodStage = FoodStage.None;
    }
}

[Serializable]
public enum FoodStage
{
    None, OnWaitingToBill, OnBilled
}
