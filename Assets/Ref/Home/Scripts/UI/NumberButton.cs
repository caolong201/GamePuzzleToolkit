using UnityEngine;
using UnityEngine.UI;

public class NumberButton : MonoBehaviour
{
    public int Value;
    public Button Button { get; private set; }

    public UIInGame UIInGame { get; set; }

    private void Start()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        UIInGame.OnClickNumberButton(Value);
    }
}
