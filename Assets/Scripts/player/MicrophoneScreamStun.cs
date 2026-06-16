using UnityEngine;
using UnityEngine.UI;
public class MicrophoneScreamStun : MonoBehaviour
{
    public MonsterAI[] monsters;

    public float screamThreshold = 0.35f;
    public float stunDuration = 5f;
    public float cooldown = 20f;
    public float stunRange = 8f;

    private AudioClip micClip;
    private string micName;
    private float nextScreamTime = 0f;
    private float cooldownStartTime;

    public Image screamCooldownBar;

    void Start()
    {

        if (screamCooldownBar != null)
        {
            screamCooldownBar.fillAmount = 1f;
        }

        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 1, 44100);
            Debug.Log("Microphone started: " + micName);
        }
        else
        {
            Debug.LogError("No microphone found!");
        }

    }

    void Update()
    {

        if (micClip == null) return;

        float loudness = GetMicLoudness();

        if (loudness >= screamThreshold && Time.time >= nextScreamTime)
        {
            StunNearestMonster();
            cooldownStartTime = Time.time;
            nextScreamTime = Time.time + cooldown;
            Debug.Log("SCREAM DETECTED. Loudness: " + loudness);
        }

        UpdateCooldownUI();
    }

    float GetMicLoudness()
    {
        float[] samples = new float[256];
        int micPosition = Microphone.GetPosition(micName) - samples.Length;

        if (micPosition < 0) return 0f;

        micClip.GetData(samples, micPosition);

        float total = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            total += Mathf.Abs(samples[i]);
        }

        return total / samples.Length;
    }

    void StunNearestMonster()
    {
        MonsterAI nearestMonster = null;
        float nearestDistance = Mathf.Infinity;

        foreach (MonsterAI monster in monsters)
        {
            if (monster == null) continue;

            float distance = Vector3.Distance(transform.position, monster.transform.position);

            if (distance < nearestDistance && distance <= stunRange)
            {
                nearestDistance = distance;
                nearestMonster = monster;
            }
        }

        if (nearestMonster != null)
        {
            nearestMonster.StunMonster(stunDuration);
        }
        else
        {
            Debug.Log("Scream detected but no monster nearby");
        }
    }

    void UpdateCooldownUI()
    {
        if (screamCooldownBar == null)
            return;

        if (Time.time >= nextScreamTime)
        {
            screamCooldownBar.fillAmount = 1f;
            return;
        }

        float elapsed = Time.time - cooldownStartTime;

        screamCooldownBar.fillAmount = elapsed / cooldown;
    }
}