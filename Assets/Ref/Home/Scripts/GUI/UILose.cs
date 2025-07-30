using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILose : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => true;

    public override bool UseBehindPanel => true;

    [SerializeField] Button retryButton;
    [SerializeField] GameObject failedText;

    public void RetryButton()
    {
        GameManager.Instance.ChangeState(GameStates.Retry);
        Hide();
    }

    public override void Show()
    {
        base.Show();
        VFXAnimationManager.Instance.PulsingAnimation(failedText, new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, 0.5f);
        retryButton.gameObject.SetActive(false);
        StartCoroutine(ShowRetryButton());
    }

    IEnumerator ShowRetryButton()
    {
        yield return new WaitForSeconds(1);
        retryButton.onClick.AddListener(RetryButton);
        retryButton.gameObject.SetActive(true);
    }
}
