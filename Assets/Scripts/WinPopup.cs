using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public TextMeshProUGUI winnerText;
    public Button enterDivinationButton;

    void Start()
    {
        popupPanel.SetActive(false);
        enterDivinationButton.onClick.AddListener(OnEnterDivination);
    }

    public void Show(string winnerName)
    {
        popupPanel.SetActive(true);
        winnerText.text = winnerName + " 勝利！\n" + winnerName + " WINS!";
    }

    void OnEnterDivination()
    {
        if (GameManager.instance != null) GameManager.instance.GoToDivinationNow();
    }
}