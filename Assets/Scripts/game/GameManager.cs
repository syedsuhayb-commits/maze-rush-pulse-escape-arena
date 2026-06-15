using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Battery UI")]
    public Image batteryImage;

    public Sprite batteryFull;
    public Sprite batteryMedium;
    public Sprite batteryLow;

    private float batteryPercent;

    [Header("Light")]
    public Light flashlight;
    private bool flickered20 = false;
    private bool flickered10 = false;
    private bool flickered5 = false;

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
            objectiveText.text = "Find the Battery";
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
        batteryPercent = (currentTime / totalTime) * 100f;

        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(batteryPercent) + "%";

            if (batteryPercent <= 20f)
            {
                timerText.color = Color.red;

                float pulse =
                    0.5f +
                    Mathf.PingPong(Time.time * 2f, 0.5f);

                timerText.alpha = pulse;
            }
            else
            {
                timerText.color = Color.white;
                timerText.alpha = 1f;
            }
        }

        if (batteryImage != null)
        {
            if (batteryPercent > 60f)
            {
                batteryImage.sprite = batteryFull;
            }
            else if (batteryPercent > 20f)
            {
                batteryImage.sprite = batteryMedium;
            }
            else
            {
                batteryImage.sprite = batteryLow;
            }
        }

        if (batteryPercent <= 20f && !flickered20)
        {
            flickered20 = true;
            StartCoroutine(FlashlightFlicker());
        }

        if (batteryPercent <= 10f && !flickered10)
        {
            flickered10 = true;
            StartCoroutine(FlashlightFlicker());
        }

        if (batteryPercent <= 5f)
        {
            if (!IsInvoking(nameof(StartFlicker)))
            {
                InvokeRepeating(nameof(StartFlicker), 0f, 0.5f);
            }
        }

        if (batteryPercent <= 0f && flashlight != null)
        {
            StopAllCoroutines();
            flashlight.enabled = false;
        }

    }

    void StartFlicker()
    {
        StartCoroutine(FlashlightFlicker());
    }

    IEnumerator FlashlightFlicker()
    {
        if (flashlight == null)
        {
            yield break;
        }

        flashlight.enabled = false;
        yield return new WaitForSeconds(0.1f);

        flashlight.enabled = true;
        yield return new WaitForSeconds(0.1f);

        flashlight.enabled = false;
        yield return new WaitForSeconds(0.15f);

        flashlight.enabled = true;
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
            objectiveText.text = "Reach the Exit";
        }

        ShowMessage("Key Collected! Exit Unlocked");
        messageText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 3f);
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

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
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