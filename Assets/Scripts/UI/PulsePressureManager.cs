using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PulsePressureManager : MonoBehaviour
{
    public enum PressureProfile
    {
        NormalRun,
        StressRising,
        PanicSpike
    }

    public enum PressureState
    {
        Normal,
        StressRising,
        HighPressure
    }

    [Header("UI References")]
    public TMP_Text bpmText;
    public TMP_Text pressureText;
    public Image pressureOverlay;

    [Header("Audio")]
    public AudioSource heartbeatAudioSource;

    [Header("BPM Settings")]
    public int currentBPM = 85;
    public int minBPM = 80;
    public int maxBPM = 140;

    [Header("Timing Settings")]
    public float updateInterval = 1.5f;

    private PressureProfile selectedProfile;
    private PressureState currentState;

    private float updateTimer;
    private float runTimer;
    private bool pressureActive;

    private int highestBPM;
    private string finalStressRating = "Normal";

    void Start()
    {
        StartPressureSystem();
    }

    void Update()
    {
        if (!pressureActive)
            return;

        runTimer += Time.deltaTime;
        updateTimer += Time.deltaTime;

        UpdatePressureOverlay();
        UpdateHeartbeatSound();

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdatePulsePressure();
            UpdateUI();
        }
    }

    public void StartPressureSystem()
    {
        pressureActive = true;
        runTimer = 0f;
        updateTimer = 0f;

        SelectRandomProfile();

        currentBPM = Random.Range(82, 92);
        highestBPM = currentBPM;

        UpdatePressureState();
        UpdateUI();

        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.loop = true;
            heartbeatAudioSource.volume = 0f;
            heartbeatAudioSource.pitch = 1f;

            if (!heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Play();
            }
        }

        Debug.Log("Selected Pressure Profile: " + selectedProfile);
    }

    public void StopPressureSystem()
    {
        pressureActive = false;
        finalStressRating = GetFinalStressRating();

        if (pressureOverlay != null)
        {
            Color overlayColor = pressureOverlay.color;
            overlayColor.a = 0f;
            pressureOverlay.color = overlayColor;
        }

        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.Stop();
        }

        Debug.Log("Final Stress Rating: " + finalStressRating);
    }

    void SelectRandomProfile()
    {
        int randomValue = Random.Range(1, 101);

        if (randomValue <= 30)
        {
            selectedProfile = PressureProfile.NormalRun;
        }
        else if (randomValue <= 80)
        {
            selectedProfile = PressureProfile.StressRising;
        }
        else
        {
            selectedProfile = PressureProfile.PanicSpike;
        }
    }

    void UpdatePulsePressure()
    {
        switch (selectedProfile)
        {
            case PressureProfile.NormalRun:
                UpdateNormalRun();
                break;

            case PressureProfile.StressRising:
                UpdateStressRising();
                break;

            case PressureProfile.PanicSpike:
                UpdatePanicSpike();
                break;
        }

        currentBPM = Mathf.Clamp(currentBPM, minBPM, maxBPM);

        if (currentBPM > highestBPM)
        {
            highestBPM = currentBPM;
        }

        UpdatePressureState();
    }

    void UpdateNormalRun()
    {
        currentBPM += Random.Range(-3, 4);

        if (currentBPM < 82)
        {
            currentBPM = Random.Range(82, 88);
        }

        if (currentBPM > 98)
        {
            currentBPM = Random.Range(90, 98);
        }
    }

    void UpdateStressRising()
    {
        if (runTimer < 15f)
        {
            currentBPM += Random.Range(-1, 4);
        }
        else if (runTimer < 35f)
        {
            currentBPM += Random.Range(1, 6);
        }
        else
        {
            currentBPM += Random.Range(-2, 5);
        }
    }

    void UpdatePanicSpike()
    {
        int spikeChance = Random.Range(1, 101);

        if (spikeChance <= 25)
        {
            currentBPM += Random.Range(15, 28);
        }
        else
        {
            currentBPM += Random.Range(-4, 5);
        }

        if (currentBPM > 135)
        {
            currentBPM -= Random.Range(4, 10);
        }
    }

    void UpdatePressureState()
    {
        if (currentBPM < 100)
        {
            currentState = PressureState.Normal;
        }
        else if (currentBPM < 120)
        {
            currentState = PressureState.StressRising;
        }
        else
        {
            currentState = PressureState.HighPressure;
        }
    }

    void UpdateUI()
    {
        if (bpmText != null)
        {
            bpmText.text = "BPM: " + currentBPM;
        }

        if (pressureText != null)
        {
            if (currentState == PressureState.Normal)
            {
                pressureText.text = "PRESSURE: NORMAL";
            }
            else if (currentState == PressureState.StressRising)
            {
                pressureText.text = "PRESSURE: STRESS RISING";
            }
            else
            {
                pressureText.text = "PRESSURE: HIGH PRESSURE";
            }
        }

        UpdatePressureOverlay();
    }

    void UpdatePressureOverlay()
    {
        if (pressureOverlay == null)
            return;

        Color overlayColor = pressureOverlay.color;

        if (currentState == PressureState.Normal)
        {
            overlayColor.a = 0f;
        }
        else if (currentState == PressureState.StressRising)
        {
            overlayColor.a = 0.12f;
        }
        else
        {
            float pulse = Mathf.Abs(Mathf.Sin(Time.time * 5f));
            overlayColor.a = Mathf.Lerp(0.18f, 0.35f, pulse);
        }

        pressureOverlay.color = overlayColor;
    }

    void UpdateHeartbeatSound()
    {
        if (heartbeatAudioSource == null)
            return;

        if (!heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Play();
        }

        if (currentState == PressureState.Normal)
        {
            heartbeatAudioSource.volume = Mathf.Lerp(heartbeatAudioSource.volume, 0.05f, Time.deltaTime * 3f);
            heartbeatAudioSource.pitch = Mathf.Lerp(heartbeatAudioSource.pitch, 0.9f, Time.deltaTime * 3f);
        }
        else if (currentState == PressureState.StressRising)
        {
            heartbeatAudioSource.volume = Mathf.Lerp(heartbeatAudioSource.volume, 0.35f, Time.deltaTime * 3f);
            heartbeatAudioSource.pitch = Mathf.Lerp(heartbeatAudioSource.pitch, 1.15f, Time.deltaTime * 3f);
        }
        else
        {
            heartbeatAudioSource.volume = Mathf.Lerp(heartbeatAudioSource.volume, 0.75f, Time.deltaTime * 4f);
            heartbeatAudioSource.pitch = Mathf.Lerp(heartbeatAudioSource.pitch, 1.45f, Time.deltaTime * 4f);
        }
    }

    string GetFinalStressRating()
    {
        if (highestBPM < 100)
        {
            return "Normal";
        }
        else if (highestBPM < 120)
        {
            return "Stress Rising";
        }
        else
        {
            return "High Pressure";
        }
    }

    public string GetStressRating()
    {
        return finalStressRating;
    }

    public int GetHighestBPM()
    {
        return highestBPM;
    }

    public PressureProfile GetSelectedProfile()
    {
        return selectedProfile;
    }

    public PressureState GetCurrentState()
    {
        return currentState;
    }
}