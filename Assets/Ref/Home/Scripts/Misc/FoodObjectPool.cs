using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodObjectPool : MonoBehaviour
{
    public Dictionary<FoodControllerKey, List<FoodController>> ActiveFoods = new Dictionary<FoodControllerKey, List<FoodController>>();
    public Dictionary<FoodControllerKey, List<FoodController>> InativeFoods = new Dictionary<FoodControllerKey, List<FoodController>>();

    public Dictionary<PassengerObjectKey, List<GameObject>> ActivePassengers = new Dictionary<PassengerObjectKey, List<GameObject>>();
    public Dictionary<PassengerObjectKey, List<GameObject>> InactivePassengers = new Dictionary<PassengerObjectKey, List<GameObject>>();
    public FoodController GetFood(int id, int quantity, GameObject pref, Transform parent)
    {
        FoodControllerKey key = new FoodControllerKey(id, quantity);
        if (!InativeFoods.ContainsKey(key))
        {
            //Debug.Log(1);
            FoodController foodController = Instantiate(pref, parent).GetComponent<FoodController>();
            if (!ActiveFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                ActiveFoods.Add(key, list);
            }
            else
            {
                ActiveFoods[key].Add(foodController);
            }
            return foodController;
        }
        else
        {
            //Debug.Log(2);
            FoodController foodController = InativeFoods[key][0];
            InativeFoods[key].Remove(foodController);
            if (InativeFoods[key].Count == 0)
            {
                InativeFoods.Remove(key);
            }
            foodController.gameObject.SetActive(true);
            if (!ActiveFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                ActiveFoods.Add(key, list);
            }
            else
            {
                ActiveFoods[key].Add(foodController);
            }
            foodController.gameObject.transform.parent = parent;
            foodController.transform.position = parent.transform.position;

            return foodController;
        }
    }

    public void RemoveFood(FoodController foodController)
    {
        FoodControllerKey key = foodController.Key;
        if (!ActiveFoods.ContainsKey(key))
        {
            Debug.Log("Key don't exits !!!");
        }
        else
        {
            if (!ActiveFoods[key].Contains(foodController))
            {
                Debug.Log("Key don't contain it !!!");
                return;
            }

            ActiveFoods[key].Remove(foodController);
            if (!InativeFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                InativeFoods.Add(key, list);
            }
            else
            {
                InativeFoods[key].Add(foodController);
              
            }
            foodController.Reset();
            foodController.gameObject.SetActive(false);
        }
    }

    public void RemoveAllFood()
    {
        //Debug.Log("Remove All Food !!!");
        foreach (List<FoodController> value in ActiveFoods.Values)
        {
            for (int i = 0; i < value.Count; i++)
            {
                //Debug.Log(value[i].gameObject.name);
                RemoveFood(value[i]);
            }
        }
    }

    public GameObject GetPassenger(int gender, int id, GameObject pref, Transform parent)
    {
        PassengerObjectKey key = new PassengerObjectKey(gender, id);
        if (!InactivePassengers.ContainsKey(key))
        {
           
            GameObject passengerObject = Instantiate(pref, parent);
            if (!ActivePassengers.ContainsKey(key))
            {
                List<GameObject> list = new List<GameObject>();
                list.Add(passengerObject);
                ActivePassengers.Add(key, list);
            }
            else
            {
                ActivePassengers[key].Add(passengerObject);
            }
            return passengerObject;
        }
        else
        {
            
            GameObject passengerObject = InactivePassengers[key][0];
            InactivePassengers[key].Remove(passengerObject);
            if (InactivePassengers[key].Count == 0)
            {
                InactivePassengers.Remove(key);
            }

            passengerObject.gameObject.SetActive(true);
            if (!ActivePassengers.ContainsKey(key))
            {
                List<GameObject> list = new List<GameObject>();
                list.Add(passengerObject);
                ActivePassengers.Add(key, list);
            }
            else
            {
                ActivePassengers[key].Add(passengerObject);
            }
            passengerObject.gameObject.transform.parent = parent;
            passengerObject.transform.position = parent.transform.position;

            return passengerObject;
        }
    }

    public void RemovePassenger(GameObject passengerObject, int gender, int id)
    {
        PassengerObjectKey key = new PassengerObjectKey(gender, id);

        if (!ActivePassengers.ContainsKey(key))
        {
            Debug.Log("Key don't exits !!!");
        }
        else
        {
            if (!ActivePassengers[key].Contains(passengerObject))
            {
                Debug.Log("Key don't contain it !!!");
                return;
            }

            ActivePassengers[key].Remove(passengerObject);
            if (!InactivePassengers.ContainsKey(key))
            {
                List<GameObject> list = new List<GameObject>();
                list.Add(passengerObject);
                InactivePassengers.Add(key, list);
            }
            else
            {
                InactivePassengers[key].Add(passengerObject);

            }

            passengerObject.gameObject.SetActive(false);
        }
    }

    public void RemoveAllPassenger()
    {
        foreach(PassengerObjectKey key in ActivePassengers.Keys)
        {
            for (int i = 0; i < ActivePassengers[key].Count; i++)
            {
                RemovePassenger(ActivePassengers[key][i], key.Gender, key.Id);
            }
        }
    }
}

[Serializable]
public class FoodControllerKey
{
    int Id;
    int Quantity;

    public FoodControllerKey(int id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }

    public bool Compare(int id, int quantity)
    {
        if (Id == id && Quantity == quantity) return true;
        return false;
    }

    public bool Compare(FoodControllerKey key)
    {
        int id = key.Id;
        int quantity = key.Quantity;
        return Compare(id, quantity);
    }

    public override bool Equals(object obj)
    {
        if (obj is FoodControllerKey other)
        {
            return Id == other.Id && Quantity == other.Quantity;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ Quantity.GetHashCode();
    }
}

[Serializable]
public class PassengerObjectKey
{
    public int Gender;
    public int Id;

    public PassengerObjectKey(int gender, int id)
    {
        Gender = gender;
        Id = id;
    }

    public override bool Equals(object obj)
    {
        if (obj is PassengerObjectKey other)
        {
            return Id == other.Id && Gender == other.Gender;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ Gender.GetHashCode();
    }
}
