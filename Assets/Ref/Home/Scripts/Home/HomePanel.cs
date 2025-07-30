using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    [SerializeField] HeaderPanel headerPanel;
    [SerializeField] ShopBuilding shopBuilding;
    [SerializeField] private ButtonBuyShopBuilding buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    private List<ButtonBuyShopBuilding> buildingButtons = new List<ButtonBuyShopBuilding>();

    private Dictionary<ShopBuildingType, int> buildingPrices = new Dictionary<ShopBuildingType, int>
    {
        { ShopBuildingType.LightGreen, 100 },
        { ShopBuildingType.Purple, 200 },
        { ShopBuildingType.Pink, 300 },
        { ShopBuildingType.Brown, 400 },
        { ShopBuildingType.White, 500 },
        { ShopBuildingType.Orange, 600 }
    };

    private int currentIndex = 0;

    private float totalProgress = 6;
    private float currentProgress = 0;
    [SerializeField] ShopComplete shopComplete;

    void Start()
    {
        Application.targetFrameRate = 60;
        InitializeButtons();
        ShowButtons();
    }

    private void InitializeButtons()
    {
        int id = 1;
        foreach (ShopBuildingType type in System.Enum.GetValues(typeof(ShopBuildingType)))
        {
            ButtonBuyShopBuilding button = Instantiate(buttonPrefab, buttonContainer);
            button.gameObject.SetActive(false);
            button.Init(id, buildingPrices[type], type);
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(type));
            buildingButtons.Add(button);
            id++;
        }
    }

    private void ShowButtons()
    {
        HideAllButtons();

        for (int i = 0; i < 3; i++)
        {
            buildingButtons[i].gameObject.SetActive(true);
            currentIndex++;
        }
    }

    private void HideAllButtons()
    {
        foreach (var button in buildingButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void OnButtonClick(ShopBuildingType type)
    {
        Debug.Log($"Selected building type: {type}, Price: ${buildingPrices[type]} & current index: {currentIndex}");
        shopBuilding.ShowBuilding(type);

        currentProgress++;
        headerPanel.UpdateProgress(currentProgress, totalProgress);
        headerPanel.Buy(buildingPrices[type]);
        
        //hide selected button
        foreach (var button in buildingButtons)
        {
            if (button.ShopBuildingType == type)
            {
                button.Selected(() =>
                {
                    //show next button
                    if (currentIndex < buildingButtons.Count)
                    {
                        buildingButtons[currentIndex].gameObject.SetActive(true);
                        buildingButtons[currentIndex].Show();
                        currentIndex++;
                    }
                });
                break;
            }
        }
        
        if (currentProgress >= totalProgress)
        {
            //complete
            DOVirtual.DelayedCall(1, () =>
            {
                shopComplete.CompleteShop();
            });
        }
    }
}