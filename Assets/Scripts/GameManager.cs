using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * GameManager.cs
 * 掛在戰鬥場景裡一個空的 GameObject 上，命名 "GameManager"。
 *
 * === Unity 設置步驟 ===
 * 1. 建立空 GameObject "GameManager"，掛上這個腳本
 * 2. 在 File > Build Settings 裡，把「主頁」「規則」「戰鬥」「占卜」四個場景
 *    都拖進 Scenes In Build 清單 (順序不重要，但都要加進去)
 * 3. 把 divinationSceneName 改成你占卜場景的實際名稱 (要跟場景檔案名一致)
 * 4. 倒數計時的UI文字 (可選) 拖進 countdownText 欄位
 */

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("場景設定")]
    public string divinationSceneName = "Divination";
    public float delayBeforeSceneChange = 1.2f;

    [Header("倒數計時 (可選)")]
    public bool useCountdown = true;
    public float countdownSeconds = 3f;

    public static string lastWinner = ""; // "Justice" 或 "Fool"，給占卜場景讀取

    private bool roundOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (useCountdown)
        {
            Time.timeScale = 0f; // 倒數時暫停遊戲
            Invoke(nameof(StartRound), countdownSeconds);
        }
    }

    void StartRound()
    {
        Time.timeScale = 1f;
    }

    // 由 FighterController 在角色死亡時呼叫
    public void OnFighterDefeated(FighterController defeated)
    {
        if (roundOver) return;
        roundOver = true;

        // defeated 是輸家，所以贏家是它的 opponent
        string winnerName = defeated.opponent.isPlayer1 ? "Justice" : "Fool";
        lastWinner = winnerName;

        Debug.Log(winnerName + " 獲勝！" + delayBeforeSceneChange + "秒後進入占卜畫面");
        Invoke(nameof(GoToDivination), delayBeforeSceneChange);
    }

    void GoToDivination()
    {
        SceneManager.LoadScene(divinationSceneName);
    }
}
