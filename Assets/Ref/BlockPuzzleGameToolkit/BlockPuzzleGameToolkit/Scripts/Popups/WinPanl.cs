using TMPro;
using UnityEngine;
public class WinPanl : MonoBehaviour
{
    public TextMeshProUGUI txtStageCompleted;   
    public void ShowWin(int level)
    {
        txtStageCompleted.text = $"STAGE {level} COMPLETED";
    }
}
