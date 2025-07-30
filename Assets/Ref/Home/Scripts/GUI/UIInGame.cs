using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => false;

    public override bool UseBehindPanel => false;

    [SerializeField] FoodIcon foodIconPref;
    [SerializeField] Transform foodIconParent;

    List<Food> availableFoods = new List<Food>();
    [SerializeField] List<FoodIcon> availableFoodIcons = new List<FoodIcon>();

    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] List<NumberButton> numberButtons = new List<NumberButton>();

    [SerializeField] Button plusButton;
    [SerializeField] Button minusButton;
    [SerializeField] Button multiplyButton;
    [SerializeField] Button divideButton;
    [SerializeField] Button equalButton;
    [SerializeField] Button undoButton;

    [SerializeField] Button decimalButton;

    [SerializeField] string number_1 = "";
    [SerializeField] string number_2 = "";

    [SerializeField] CaculatorStage caculatorStage = CaculatorStage.FirstEnter;
    [SerializeField] MathematicalType mathematicalType = MathematicalType.Plus;

    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] Image slider;
    [SerializeField] GameObject progressPanel;

    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI passengerText;
    public override void Show()
    {
        base.Show();
        
        availableFoodIcons.Clear();
        List<Food> foods = DayManager.Instance.FoodDict.Values.ToList();
        int day = DayManager.Instance.DayIndex;
        foreach (Food food in foods)
        {
            if (food.DayUnlock <= day && !availableFoods.Contains(food))
            {
                availableFoods.Add(food);
                FoodIcon foodIcon = Instantiate(foodIconPref, foodIconParent);
                foodIcon.SetUp(food);
                availableFoodIcons.Add(foodIcon);
            }
        }

        foreach(NumberButton numberButton in numberButtons)
        {
            numberButton.UIInGame = this;
        }
    }

    public void UndoButton()
    {
        Reset();
    }

    public void DecimalButton()
    {     
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            if (mathematicalType == MathematicalType.None)
            {
                number_1 = "";
                mathematicalType = MathematicalType.Plus;
            }
            if (number_1.Length < 11 && number_1.Length > 0 && !number_1.Contains("."))
            {
                number_1 += ".";
           
            }
            else
            {
                Debug.LogError("Can't enter it here !!!");
            }
            DisplayValueText(number_1);
        }
        else
        {
            if (number_2.Length < 11 && number_2.Length > 0 && !number_2.Contains("."))
            {
                number_2 += ".";
                
            }
            else
            {
                Debug.LogError("Can't enter it here !!!");
            }
            DisplayValueText(number_2);
        }
    }
    public void OnClickNumberButton(int value)
    {
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            if (mathematicalType == MathematicalType.None)
            {
                number_1 = "";
                mathematicalType = MathematicalType.Plus;
            }
            if (number_1.Length < 12)
            {
                number_1 += value.ToString();
                
            }
            else
            {
                Debug.LogError("Can't enter too many number !!!");
            }
            DisplayValueText(number_1);
        }
        else
        {
            if (number_2.Length < 12)
            {
                number_2 += value.ToString();
               
            }
            else
            {
                Debug.LogError("Can't enter too many number !!!");
            }
            DisplayValueText(number_2);
        }
    }

    public void EqualButton()
    {
        CaculateLastMath();
        DisplayValueText(number_1);
  
        number_2 = "";
        caculatorStage = CaculatorStage.FirstEnter;
        mathematicalType = MathematicalType.None;

        float result_1 = ConvertStringToFloat(number_1);
        DayManager.Instance.CheckAnswer(result_1);
    }
    public void PlusButton()
    {
        CaculateLastMath();
        Plus();
    }

    void Plus()
    {
        mathematicalType = MathematicalType.Plus;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 + result_2).ToString();
        number_2 = "";
        DisplayValueText(number_1);
    }
    public void MinusButton()
    {
        CaculateLastMath();
        Minus();
    }

    void Minus()
    {
        mathematicalType = MathematicalType.Minus;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 - result_2).ToString();
        number_2 = "";
        DisplayValueText(number_1);
    }
    public void MutiplyButton()
    {
        CaculateLastMath();
        Mutiply();
    }

    void Mutiply()
    {
        mathematicalType = MathematicalType.Mutiply;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);
      
        number_1 = (result_1 * result_2).ToString();
        number_2 = "";
        DisplayValueText(number_1);
    }
    public void DivideButton()
    {
        CaculateLastMath();
        Divide();
    }

    void Divide()
    {
        mathematicalType = MathematicalType.Divide;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 / result_2).ToString();
      
        DisplayValueText(number_1);
    }

    void CaculateLastMath()
    {
        switch (mathematicalType)
        {
            case MathematicalType.Plus:
                Plus();
                break;
            case MathematicalType.Minus:
                Minus();
                break;
            case MathematicalType.Mutiply:
                Mutiply();
                break;
            case MathematicalType.Divide:
                Divide();
                break;
            default:
                break;
        }
    }
    void DisplayValueText(string str)
    {
        valueText.color = Color.black;
        valueText.text = str;
    }

    public void SetGreenColorValueText()
    {
        valueText.color = Color.green;
    }

    public void SetRedColorValueText()
    {
        valueText.color = Color.red;
    }

    float ConvertStringToFloat(string str)
    {
        float result_1 = 0;

        if (float.TryParse(str, out result_1))
        {
            //Debug.Log(result);
        }
        else
        {
            result_1 = 0f;
            if (mathematicalType == MathematicalType.Mutiply || mathematicalType == MathematicalType.Divide)
            {
                result_1 = 1;
            }
        }

        return result_1;
    }

    private void Start()
    {
        equalButton.onClick.AddListener(EqualButton);
        decimalButton.onClick.AddListener(DecimalButton);
        plusButton.onClick.AddListener(PlusButton);
        minusButton.onClick.AddListener(MinusButton);
        multiplyButton.onClick.AddListener(MutiplyButton);
        divideButton.onClick.AddListener(DivideButton);
        undoButton.onClick.AddListener(UndoButton);
    }

    Coroutine sliderCoroutine;
    public void SetProgress(float a, float b)
    {
        //progressText.text = a.ToString() + "/" + b.ToString();
        Reset();
        progressText.text = GameManager.Instance.UserData.coin.ToString();
        passengerText.text = (DayManager.Instance.TotalDayPassenger - DayManager.Instance.ServedPassenger).ToString();


        if (sliderCoroutine != null) StopCoroutine(sliderCoroutine);
        sliderCoroutine = StartCoroutine(AnimateFillAmount(a / b));
    }
    private IEnumerator AnimateFillAmount(float targetFillAmount)
    {
        if(slider == null) yield break;
        float startFillAmount = slider.fillAmount;
        float fillDuration = 0.5f;
        float fillElapsed = 0f;

        Vector3 startScale = progressPanel.transform.localScale; 
        Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f); 
        Vector3 endScale = Vector3.one; 
        float scaleDuration = 0.5f; 
        float scaleElapsed = 0f;

        while (fillElapsed < fillDuration || scaleElapsed < scaleDuration)
        {
            if (fillElapsed < fillDuration)
            {
                fillElapsed += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, fillElapsed / fillDuration);
            }

            if (scaleElapsed < scaleDuration)
            {
                scaleElapsed += Time.deltaTime;
                progressPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleElapsed / scaleDuration);
            }

            yield return null; 
        }


        slider.fillAmount = targetFillAmount;
        progressPanel.transform.localScale = endScale; 
    }
    public void SetDayText(int day)
    {
        dayText.text = "Day " + (day + 1).ToString();
    }

    private void Reset()
    {
        number_1 = "";
        number_2 = "";
        caculatorStage = CaculatorStage.FirstEnter;
        mathematicalType = MathematicalType.None;
        DisplayValueText("0");
    }

    public void Tutorial()
    {
        foreach (FoodIcon foodIcon in availableFoodIcons)
        {
            foodIcon.Tutorial();
        }
    }

    public void StopTutorial()
    {
        foreach (FoodIcon foodIcon in availableFoodIcons)
        {
            foodIcon.StopTutorial();
        }
    }
}

[Serializable]
public enum CaculatorStage
{
    FirstEnter, SecondEnter, ThirdEnter
}

public enum MathematicalType
{
    None, Plus, Minus, Mutiply, Divide
}