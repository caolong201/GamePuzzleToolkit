using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBuyShopBuilding : MonoBehaviour
{
    [SerializeField] Transform priceGroup, checkmarkGroup;
    [SerializeField] private TextMeshProUGUI txtId, txtPrice;
    [SerializeField] private Image imgIcon;
    private ShopBuildingType shopBuildingType;
    public ShopBuildingType ShopBuildingType => shopBuildingType;
    public void Init(int id, int price, ShopBuildingType shopBuildingType)
    {
        txtId.text = id.ToString();
        txtPrice.text = price.ToString();
        this.shopBuildingType = shopBuildingType;
        checkmarkGroup.localScale  = Vector3.zero;
        LoadSpriteToImage(id.ToString());
    }
    
    private void LoadSpriteToImage(string spriteName)
    {
        // Load the sprite from Resources/Items
        Sprite sprite = Resources.Load<Sprite>($"Items/{spriteName}");
        if (sprite != null)
        {
            imgIcon.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite '{spriteName}' not found in Resources/Items.");
        }
    }

    public void Selected(System.Action complete)
    {
        GetComponent<Button>().interactable = false;
        transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.2f).OnComplete(() =>
        {
            priceGroup.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                checkmarkGroup.DOScale(Vector3.one, 0.4f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    complete?.Invoke();
                });;
            });
        });
        
    }
    
    public void Show(System.Action complete = null)
    {
        transform.localScale  = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
        {
            complete?.Invoke();
        });;
    }
}
