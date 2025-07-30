using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DayConfig : ScriptableObject
{
    public List<Day> Days = new List<Day>();

    public Dictionary<int, Day> CreateDayDictionary()
    {
        Dictionary<int, Day> DayDict = new Dictionary<int, Day>();
        foreach (Day day in Days)
        {
            DayDict.Add(day.DayID, day);
        }

        return DayDict;
    }
}

[Serializable]
public class Day
{
    public int DayID;
    public List<Passenger> PassengerList = new List<Passenger>();
}
