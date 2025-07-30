using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> malePassengers = new List<GameObject>();
    [SerializeField] List<GameObject> femalePassengers = new List<GameObject>();

    [SerializeField] CurrentPassengerController currentPassengerController;
    [SerializeField] NextPassengerController nextPassengerController;
    public CurrentPassengerController CurrentPassengerController => currentPassengerController;
    public NextPassengerController NextPassengerController => nextPassengerController;

    [SerializeField] DayManager dayManager;
    public DayManager DayManager => dayManager;

    public void GetRandomPassengerObject()
    {
        int gender = Random.Range(0, 2);
        if(gender == 0)
        {
            //nam
            int choice = Random.Range(0, malePassengers.Count);
            GameObject passengerObj = malePassengers[choice];
            //FatnessLevel fatnessLevel = passengerObj.GetComponent<FatnessLevel>();
            //HairSelector.HairSelector hairSelector = passengerObj.GetComponent<HairSelector.HairSelector>();
            //HeadSelector.HeadSelector headSelector = passengerObj.GetComponent<HeadSelector.HeadSelector>();
            //SkinSelector.SkinSelector skinSelector = passengerObj.GetComponent<SkinSelector.SkinSelector>();
            //GlassesSelector.GlassesSelector glassesSelector = passengerObj.GetComponent <GlassesSelector.GlassesSelector>();

            nextPassengerController.SetPassengerPrefab(passengerObj, gender, choice);
        }
        else
        {
            //nu
            int choice = Random.Range(0, femalePassengers.Count);
            GameObject passengerObj = femalePassengers[choice];

            nextPassengerController.SetPassengerPrefab(passengerObj, gender, choice);
        }
    }
}
