using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodIcon : MonoBehaviour
{
    [SerializeField] Image foodIcon;
    [SerializeField] TextMeshProUGUI priceText;

    bool stopCoroutine = false;
    public void SetUp(Food food)
    {
        priceText.text = food.Price.ToString();
        foodIcon.sprite = food.Icon;
    }

    public void Tutorial()
    {
        StartCoroutine(ScaleOverTime(priceText.gameObject, 0.5f, new Vector3(2f, 2f, 2f), Vector3.one));
    }

    public void StopTutorial()
    {
        stopCoroutine = true;
    }
    IEnumerator ScaleOverTime(GameObject uiObject, float time, Vector3 maxScale, Vector3 minScale)
    {
        float currentTime = 0.0f;

        while (currentTime < time)
        {
            if (stopCoroutine)
            {
                stopCoroutine = false;
                uiObject.transform.localScale = Vector3.one;
                yield break;
            }
            currentTime += Time.deltaTime;
            uiObject.transform.localScale = Vector3.Lerp(maxScale, minScale, currentTime / time);
            yield return null;
        }


        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ScaleOverTime(uiObject, time, minScale, maxScale));
    }
}
