using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
public class GlobalConfig : Singleton<GlobalConfig>
{
    public UserData userData
    {
        get; private set;
    }

    protected override void Awake()
    {
        base.Awake();
        Game.Launch();
        DontDestroyOnLoad(gameObject);
        userData = Game.Data.Load<UserData>();
    }
}