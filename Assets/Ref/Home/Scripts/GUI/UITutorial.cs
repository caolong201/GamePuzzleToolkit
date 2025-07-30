using UnityEngine;
using UnityEngine.UI;

public class UITutorial : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => true;

    public override bool UseBehindPanel => false;

    [SerializeField] Button stage1Button;
    [SerializeField] Button stage2Button;
    [SerializeField] Button stageCompletedButton;

    [SerializeField] GameObject stage1;
    [SerializeField] GameObject stage2;
    [SerializeField] GameObject stage3;
    [SerializeField] GameObject stageFailed;
    [SerializeField] GameObject stageCompleted;

    [SerializeField] GameObject pointerStage1;
    [SerializeField] GameObject pointerStage2;
    [SerializeField] GameObject pointerStage3;

    public override void Show()
    {
        base.Show();
        stage1.SetActive(true);
        stage1Button.onClick.AddListener(Stage1Button);
        VFXAnimationManager.Instance.PulsingAnimation(pointerStage1, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, 0.5f);
        stage2.SetActive(false);
        stage3.SetActive(false);
        stageCompleted.SetActive(false);
        stageFailed.SetActive(false);
    }
    public void Stage1Button()
    {
        stage1.SetActive(false);
        stage2.SetActive(true);
        VFXAnimationManager.Instance.PulsingAnimation(pointerStage2, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, 0.5f);

        stage2Button.onClick.AddListener(Stage2Button);
        GameUI.Instance.Get<UIInGame>().Tutorial();
    }

    public void Stage2Button()
    {
        stage2.SetActive(false);
        stage3.SetActive(true);
        VFXAnimationManager.Instance.PulsingAnimation(pointerStage3, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, 0.5f);
        GameUI.Instance.Get<UIInGame>().StopTutorial();
    }

    public void StageCompletedButton()
    {
        Hide();
    }
    public void ShowFailedStage()
    {
        stage3.SetActive(false);
        stageFailed.SetActive(true);

    }

    public void ShowSuccessStage()
    {
        stageCompleted.SetActive(true);
        stageFailed.SetActive(false);
        stage3.SetActive(false);

        stageCompletedButton.onClick.AddListener(StageCompletedButton);
    }
}
