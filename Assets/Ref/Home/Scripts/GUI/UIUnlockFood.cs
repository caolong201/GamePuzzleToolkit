using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnlockFood : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => false;

    public override bool UseBehindPanel => true;

    [SerializeField] Button claimButton;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Image icon;

    [SerializeField] List<Food> needToShowFoodList = new List<Food>();
    Food currentShowFood;
    public void SetUp(Food food)
    {
        Sprite sprite = food.Icon;
        float price = food.Price;

        icon.sprite = sprite;
        coinText.text = price.ToString();
    }

    public void AddToShowList(Food food)
    {
        needToShowFoodList.Add(food);
        SetUp(food);
        currentShowFood = food;
    }

    public void ClaimButton()
    {
        if(needToShowFoodList.Count <= 0) Hide();
        else
        {
            needToShowFoodList.Remove(currentShowFood);
            int index = needToShowFoodList.Count - 1;
            if(index < 0)
            {
                Hide();
                return;
            }
            currentShowFood = needToShowFoodList[index];
            SetUp(currentShowFood);
        }
    }

    private void Start()
    {
        claimButton.onClick.AddListener(ClaimButton);
    }
}
