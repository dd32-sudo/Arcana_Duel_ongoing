using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 引入場景管理

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button rulesButton;
    public Button quitButton;

    void Start()
    {
        // 透過程式碼直接綁定按鈕功能
        startButton.onClick.AddListener(OnStart);
        rulesButton.onClick.AddListener(OnRules);
        quitButton.onClick.AddListener(OnQuit);
    }

    void OnStart()
    {
        // 直接讀取 Battle 場景
        SceneManager.LoadScene("Battle");
    }

    void OnRules()
    {
        // 直接讀取 Rules 場景
        SceneManager.LoadScene("Rules");
    }

    void OnQuit()
    {
        Application.Quit();
        Debug.Log("遊戲已退出"); // 方便在編輯器測試時看到提示
    }
}