using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopBuildingType
{
    LightGreen = 1,
    Purple = 2,
    Pink = 3,
    Brown = 4,
    White = 5,
    Orange = 6
}

public class ShopBuilding : MonoBehaviour
{
    private Dictionary<ShopBuildingType, List<ShopBuildingComponent>> buildings;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeBuildings();
    }

    private void InitializeBuildings()
    {
        buildings = new Dictionary<ShopBuildingType, List<ShopBuildingComponent>>();

        // Find all ShopBuildingComponents in child objects
        ShopBuildingComponent[] components = GetComponentsInChildren<ShopBuildingComponent>();

        foreach (var component in components)
        {
            ShopBuildingType type = component.ShopBuildingType;

            if (!buildings.ContainsKey(type))
            {
                buildings[type] = new List<ShopBuildingComponent>();
            }

            buildings[type].Add(component);
        }
        
        // //hide all objects
        // foreach (var component in components)
        // {
        //    component.gameObject.SetActive(false);
        // }
        //
        // // Debug to verify the result
        // DebugBuildings();
    }

    private void DebugBuildings()
    {
        foreach (var pair in buildings)
        {
            Debug.Log($"Building Type: {pair.Key}, Count: {pair.Value.Count}");
        }
    }

    public void ShowBuilding(ShopBuildingType shopBuildingType)
    {
        if (buildings == null || !buildings.ContainsKey(shopBuildingType))
        {
            Debug.LogWarning($"No buildings found for type: {shopBuildingType}");
            return;
        }

        foreach (var buildingComponent in buildings[shopBuildingType])
        {
            buildingComponent.Show();
        }

        Debug.Log($"Showing buildings of type: {shopBuildingType}");
    }
}
