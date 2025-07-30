using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using Data;

public class GameManager : Singleton<GameManager>
{
    public UserData UserData
    {
        get; private set;
    }
    protected override void Awake()
    {
        base.Awake();
        Game.Launch();
        UserData = Game.Data.Load<UserData>();
        
    }
   

    private void Start()
    {
        //GameUI.Instance.Get<UIStart>().Show();
        DayManager.Instance.OnStart();
        ChangeState(GameStates.Start);
    }

   
    [SerializeField] private GameStates _state = GameStates.Retry;
    public void ChangeState(GameStates newState)
    {
        if (newState == _state) return;
        ExitCurrentState();
        _state = newState;
        EnterNewState();
    }

    private void EnterNewState()
    {

        switch (_state)
        {
            case GameStates.Tutorial:
                GameUI.Instance.Get<UITutorial>().Show();
                break;
            case GameStates.Home:
                break;
            case GameStates.Start:
                DayManager.Instance.LoadDay();
                break;
            case GameStates.Play:
              
                break;
            case GameStates.Retry:
                DayManager.Instance.LoadDay(true);
                break;
            case GameStates.Win:
                if (DayManager.Instance.UseDayInUserData)
                {
                    UserData.day = DayManager.Instance.DayIndex + 1;
                    if(UserData.day >= DayManager.Instance.MaxDay)
                    {
                        DayManager.Instance.GenerateRandomDay();
                    }
                }
              
                GameUI.Instance.Get<UIInGame>().Hide();
                GameUI.Instance.Get<UIWin>().Show();
                GameUI.Instance.Get<UIWin>().SetCoinText(DayManager.Instance.TotalDayCoin);
                GameUI.Instance.Get<UIWin>().SetDayText(DayManager.Instance.DayIndex + 1);
                GameUI.Instance.Get<UIWin>().SetVisitorText(DayManager.Instance.TotalDayPassenger);
                break;
            case GameStates.Lose:
                GameUI.Instance.Get<UIInGame>().Hide();
                GameUI.Instance.Get<UILose>().Show();
                break;
            case GameStates.NextLevel:
                DayManager.Instance.LoadDay();
                break;
            default:
                break;
        }
    }

    private void ExitCurrentState()
    {
        switch (_state)
        {
            case GameStates.Tutorial:
                break;
            case GameStates.Home:
                break;
            case GameStates.Start:
                break;
            case GameStates.Play:
   
                break;
            case GameStates.Retry:
                break;
            case GameStates.Win:
                break;
            case GameStates.Lose:
                break;
            case GameStates.NextLevel:
          
                break;
            default:
                break;
        }
    }

    public bool CheckUnlockedFood(int id)
    {
        if (UserData.unlockedFoodIdList.Contains(id))
        {
           
            return true;
        }
        if (DayManager.Instance.UseDayInUserData)
        {
            UserData.unlockedFoodIdList.Add(id);
        }
    
        return false;
    }

    public void AddCoin(float coin)
    {
        UserData.coin += coin;
    }
}

public enum GameStates
{
    Play, Win, Lose, Home, Tutorial, Start, Retry, NextLevel
}
