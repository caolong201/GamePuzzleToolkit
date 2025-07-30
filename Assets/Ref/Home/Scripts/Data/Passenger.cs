using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Passenger 
{
    public List<FoodOrder> FoodOrderList = new List<FoodOrder>();
}

[Serializable]
public class FoodOrder
{
    public int Quantity;
    public int FoodId;
}
