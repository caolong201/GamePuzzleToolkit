using UnityEngine;
using UnityEngine.UI;

public class UIStart : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => true;

    public override bool UseBehindPanel => true;

    [SerializeField] Button startButton;

    public void StartButton()
    {
        GameManager.Instance.ChangeState(GameStates.Start);
        Hide();
    }

    public override void Show()
    {
        base.Show();
        VFXAnimationManager.Instance.PulsingAnimation(startButton.gameObject, new Vector3(1.2f, 1.2f, 1.2f), new Vector3(0.7f, 0.7f, 0.7f), 0.5f);
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
    }
}
