using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPassengerController : MonoBehaviour
{
    [SerializeField] DayManager dayManager;
    [SerializeField] PassengerManager passengerManager;
    [SerializeField] CurrentPassengerController currentPassengerController;
    [SerializeField] Animator m_animator;
    [SerializeField] GameObject nextPassengerObject;
    [SerializeField] PassengerObjectKey nextPassengerKey;
    [SerializeField] PassengerStage passengerStage = PassengerStage.None;

    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform waitForTurnPoint;

    Coroutine coroutine;

    bool firstLoad = true;
    public void SetPassengerPrefab(GameObject passengerPref, int gender, int id)
    {
        //ResetPassenger();
        nextPassengerKey = new PassengerObjectKey(gender, id);
        nextPassengerObject = dayManager.FoodObjectPool.GetPassenger(gender, id, passengerPref, this.transform);

        m_animator = nextPassengerObject.GetComponent<Animator>();
        ChangeState(PassengerStage.OnWalkingIn);
    }

    public void ChangeState(PassengerStage newState)
    {
        if (newState == passengerStage) return;

        passengerStage = newState;
        EnterNewState();
    }

    private void EnterNewState()
    {
        switch (passengerStage)
        {
            case PassengerStage.OnWaitingForTurn:
                OnWaitingForTurn();
                break;
            case PassengerStage.OnWalkingIn:
                OnWalkingIn();
                break;
            default:

                break;
        }
    }

    void OnWaitingForTurn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = null;

        if (firstLoad)
        {
            firstLoad = false;
        }
        else
        {
            RandomIdleAnim();
        }
        

        StartCoroutine(WaitToTransport());
      
    }

    void OnWalkingIn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }


        transform.position = spawnPoint.position;
        m_animator.SetTrigger("Walk");
        coroutine = StartCoroutine(MoveAndRotate(waitForTurnPoint.position, 1f, 90));
    }


    private IEnumerator MoveAndRotate(Vector3 destination, float moveTime, float targetRotationY)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, targetRotationY, 0) * startRotation;
        transform.rotation = endRotation;

        float moveElapsedTime = 0;

        while (moveElapsedTime < moveTime)
        {
            if (moveElapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, destination, moveElapsedTime / moveTime);
                moveElapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        transform.position = destination;
      

        switch (passengerStage)
        {
            case PassengerStage.OnWalkingIn:
                ChangeState(PassengerStage.OnWaitingForTurn);
                break;
            default:
                break;
        }
    }

    IEnumerator WaitToTransport()
    {
        yield return new WaitUntil(() => currentPassengerController.nextPassenger);

        currentPassengerController.SetPassenger(nextPassengerObject, nextPassengerKey);
        ResetPassenger();
        if (dayManager.IsLastWave) yield break;
        passengerManager.GetRandomPassengerObject();
    }

    private void ResetPassenger()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        coroutine = null;
        m_animator = null;
        nextPassengerObject = null;
        passengerStage = PassengerStage.None;
    }

    public void Reset(bool isRetry)
    {
        if (!firstLoad)
        {
            if (nextPassengerObject != null)
            {
                dayManager.FoodObjectPool.RemovePassenger(nextPassengerObject, nextPassengerKey.Gender, nextPassengerKey.Id);
            }
            
            if(!isRetry) firstLoad = true;
            currentPassengerController.SetFirstLoad(true);
        }
       
        //ResetPassenger();
    }
    void RandomIdleAnim()
    {
        m_animator.SetTrigger("Idle");
        int choice = UnityEngine.Random.Range(0, 4);
        switch (choice)
        {
            case 0:

                break;
            case 1:
                m_animator.SetTrigger("Idle1");
                break;
            case 2:
                m_animator.SetTrigger("Idle2");
                break;
        }
    }
}
