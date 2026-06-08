using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalTime = 60f;
    private float currentTime;
    private bool gameRunning = true;

    [Header("Game State")]
    private bool hasBattery = false;
    private string playerName = "Player";

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text objectiveText;
    public TMP_Text messageText;
    public TMP_Text playerNameText;

    [Header("End Panel")]
    public GameObject endPanel;
    public TMP_Text endTitleText;
    public TMP_Text endSubText;
    public TMP_Text leaderboardText;

    [Header("Player")]
    public FirstPersonController playerController;

    [Header("Ghost System")]
    public GhostRecorder ghostRecorder;

    void Start()
    {
        currentTime = totalTime;
        gameRunning = true;
        hasBattery = false;

        Time.timeScale = 1f;

        playerName = PlayerPrefs.GetString("PlayerName", "Player");

        if (playerNameText != null)
        {
            playerNameText.text = "Player : " + playerName;
        }

        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        if (objectiveText != null)
        {
            objectiveText.text = "Objective: Find the Battery";
        }

        if (messageText != null)
        {
            messageText.text = "";
        }

        if (leaderboardText != null)
        {
            leaderboardText.text = "";
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateTimerUI();

        if (ghostRecorder != null)
        {
            ghostRecorder.StartRecording();
        }
    }

    void Update()
    {
        if (!gameRunning)
        {
            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateTimerUI();
            LoseGame();
            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + currentTime.ToString("F2");
        }
    }

    public void CollectBattery()
    {
        if (!gameRunning)
        {
            return;
        }

        hasBattery = true;

        if (objectiveText != null)
        {
            objectiveText.text = "Objective: Reach the Exit";
        }

        ShowMessage("Battery Collected! Exit Unlocked!");
    }

    public bool HasBattery()
    {
        return hasBattery;
    }

    public void WinGame()
    {
        if (!gameRunning)
        {
            return;
        }

        gameRunning = false;

        float completionTime = totalTime - currentTime;

        if (ghostRecorder != null)
        {
            ghostRecorder.StopRecording();

            if (IsBestGhostTime(completionTime))
            {
                ghostRecorder.SaveGhostPath(playerName, completionTime);
                ShowMessage("New Best Ghost Saved!");
            }
        }

        SaveScore(playerName, completionTime);

        ShowEndPanel(
            "YOU ESCAPED!",
            playerName + " finished in " + completionTime.ToString("F2") + " seconds"
        );

        ShowLeaderboard();

        StopPlayer();
    }

    public void LoseGame()
    {
        if (!gameRunning)
        {
            return;
        }

        gameRunning = false;

        if (ghostRecorder != null)
        {
            ghostRecorder.StopRecording();
        }

        ShowEndPanel(
            "TIME UP!",
            playerName + ", the maze defeated you. Try again!"
        );

        ShowLeaderboard();

        StopPlayer();
    }

    bool IsBestGhostTime(float completionTime)
    {
        if (!PlayerPrefs.HasKey("GhostBestTime"))
        {
            return true;
        }

        float currentBestTime = PlayerPrefs.GetFloat("GhostBestTime", 999f);

        return completionTime < currentBestTime;
    }

    void ShowEndPanel(string title, string subtitle)
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endTitleText != null)
        {
            endTitleText.text = title;
        }

        if (endSubText != null)
        {
            endSubText.text = subtitle;
        }
    }

    void StopPlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    public bool IsGameRunning()
    {
        return gameRunning;
    }

    public float GetCompletionTime()
    {
        return totalTime - currentTime;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ---------------- LEADERBOARD SYSTEM ----------------

    void SaveScore(string name, float time)
    {
        List<LeaderboardEntry> entries = LoadScores();

        LeaderboardEntry newEntry = new LeaderboardEntry();
        newEntry.playerName = name;
        newEntry.time = time;

        entries.Add(newEntry);

        entries.Sort((a, b) => a.time.CompareTo(b.time));

        if (entries.Count > 10)
        {
            entries.RemoveRange(10, entries.Count - 10);
        }

        PlayerPrefs.SetInt("LeaderboardCount", entries.Count);

        for (int i = 0; i < entries.Count; i++)
        {
            PlayerPrefs.SetString("LeaderboardName_" + i, entries[i].playerName);
            PlayerPrefs.SetFloat("LeaderboardTime_" + i, entries[i].time);
        }

        PlayerPrefs.Save();
    }

    List<LeaderboardEntry> LoadScores()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        int count = PlayerPrefs.GetInt("LeaderboardCount", 0);

        for (int i = 0; i < count; i++)
        {
            LeaderboardEntry entry = new LeaderboardEntry();
            entry.playerName = PlayerPrefs.GetString("LeaderboardName_" + i, "Player");
            entry.time = PlayerPrefs.GetFloat("LeaderboardTime_" + i, 999f);

            entries.Add(entry);
        }

        return entries;
    }

    void ShowLeaderboard()
    {
        if (leaderboardText == null)
        {
            return;
        }

        List<LeaderboardEntry> entries = LoadScores();

        if (entries.Count == 0)
        {
            leaderboardText.text = "No leaderboard records yet.";
            return;
        }

        string display = "TOP 10 FASTEST\n\n";

        for (int i = 0; i < entries.Count; i++)
        {
            display += (i + 1) + ". " + entries[i].playerName + " - " + entries[i].time.ToString("F2") + "s\n";
        }

        leaderboardText.text = display;
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public float time;
    }
}