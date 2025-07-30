using System;
using System.Collections;
using UnityEngine;

public class CurrentPassengerController : MonoBehaviour
{
    [SerializeField] DayManager dayManager;
    [SerializeField] NextPassengerController nextPassengerController;
    [SerializeField] Animator m_animator;
    [SerializeField] GameObject currentPassengerObject;
    [SerializeField] PassengerObjectKey currentPassengerKey;
    [SerializeField] PassengerStage passengerStage = PassengerStage.None;

    [SerializeField] Transform waitForTurnPoint;
    [SerializeField] Transform waitForBillPoint;
    [SerializeField] Transform despawnPoint;

    public bool nextPassenger = true;
    Coroutine coroutine;

    bool firstLoad = true;
    public void SetPassenger(GameObject passenger, PassengerObjectKey key)
    {
        currentPassengerKey = key;
        currentPassengerObject = passenger;
      
        passenger.gameObject.transform.parent = transform;
        currentPassengerObject.transform.localPosition = Vector3.zero;
        currentPassengerObject.transform.localRotation = Quaternion.Euler(0, 90, 0);

        m_animator = currentPassengerObject.GetComponent<Animator>();

        nextPassenger = false;
        ChangeState(PassengerStage.OnWaitingForTurn);
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
            case PassengerStage.None:

                break;
            case PassengerStage.OnWalkingOut:
                OnWalkingOut();
                break;
            case PassengerStage.OnWaitingForTurn:
                OnWalkingToWaitForBillPoint();
                break;
            case PassengerStage.OnWaitingForBill:
                OnWaitingForBill();
                break;
            case PassengerStage.OnWalkingIn:
                OnWalkingToWaitForBillPoint();
                break;
            default:

                break;
        }
    }

    void OnWaitingForBill()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }        
        
        RandomIdleAnim();

        coroutine = StartCoroutine(MoveAndRotate(waitForBillPoint.position, 0f, 80f, 0.25f));
    }

    void OnWalkingToWaitForBillPoint()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        transform.position = waitForTurnPoint.position;
        if (firstLoad)
        {
            firstLoad = false;
        }
        else
        {
            m_animator.SetTrigger("Walk");
        }
        coroutine = StartCoroutine(MoveAndRotate(waitForBillPoint.position, .75f, 0f, 0f));
    }

    void OnWalkingOut()
    {
        VFXAnimationManager.Instance.PlayHappyEmoji();
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
       
        m_animator.SetTrigger("Walk");
     
        coroutine = StartCoroutine(MoveAndRotate(despawnPoint.position, 1.5f, -90, 0.1f));
    }

    private IEnumerator MoveAndRotate(Vector3 destination, float moveTime, float targetRotationY, float rotationTime)
    {
        Vector3 startPosition = transform.position; 
        Quaternion startRotation = transform.rotation; 
        Quaternion endRotation = Quaternion.Euler(0, targetRotationY, 0) * startRotation; 

        float moveElapsedTime = 0;
        float rotationElapsedTime = 0;

        while (moveElapsedTime < moveTime || rotationElapsedTime < rotationTime)
        {
            if (moveElapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, destination, moveElapsedTime / moveTime);
                moveElapsedTime += Time.deltaTime;
            }

            if (rotationElapsedTime < rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationElapsedTime / rotationTime);
                rotationElapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        transform.position = destination;
        transform.rotation = endRotation;

        switch (passengerStage)
        {
            case PassengerStage.OnWalkingOut:
                dayManager.WaveFinished = true;
                Reset();
                break;
            case PassengerStage.OnWaitingForTurn:
                ChangeState(PassengerStage.OnWaitingForBill);
                break;
            default:
                break;
        }
    }


    public void Reset()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        coroutine = null;
        m_animator = null;
        dayManager.FoodObjectPool.RemovePassenger(currentPassengerObject, currentPassengerKey.Gender, currentPassengerKey.Id);

        passengerStage = PassengerStage.None;
    }

    public void SetFirstLoad(bool firstLoad)
    {
        this.firstLoad = firstLoad;
    }

    void RandomIdleAnim()
    {
        m_animator.SetTrigger("Idle");
        int choice = UnityEngine.Random.Range(0, 3);
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

[Serializable]
public enum PassengerStage
{
    None, OnWalkingIn, OnWaitingForTurn, OnWaitingForBill, OnWalkingOut
}
