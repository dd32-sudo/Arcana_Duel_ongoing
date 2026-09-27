using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance => instance;

    public string battleSceneName = "Battle";
    public string rulesSceneName = "Rules";
    public string divinationSceneName = "Divination";
    public string mainMenuSceneName = "MainMenu";

    public bool useCountdown = true;
    public float countdownSeconds = 3f;

    public static string lastWinner = "";

    private bool roundOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (useCountdown && SceneManager.GetActiveScene().name == battleSceneName)
        {
            Time.timeScale = 0f;
            Invoke(nameof(StartRound), countdownSeconds);
        }
    }

    void StartRound()
    {
        Time.timeScale = 1f;
    }

    public void OnFighterDefeated(FighterController defeated)
    {
        if (roundOver) return;
        roundOver = true;

        string winnerName = defeated.opponent.isPlayer1 ? "Justice" : "Fool";
        lastWinner = winnerName;

        // 取消彈窗邏輯，改為延遲 1.5 秒後直接切入占卜場景
        Invoke(nameof(GoToDivinationNow), 1.5f);
    }

    public void GoToDivinationNow()
    {
        Time.timeScale = 1f;
        roundOver = false;
        SceneManager.LoadScene(divinationSceneName);
    }

    public void GoToBattle()
    {
        roundOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(battleSceneName);
    }

    public void GoToRules()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(rulesSceneName);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}