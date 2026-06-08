using UnityEngine;
using System.Collections.Generic;

public class GhostReplay : MonoBehaviour
{
    private List<Vector3> ghostPositions = new List<Vector3>();
    private List<Quaternion> ghostRotations = new List<Quaternion>();
    private List<float> ghostTimes = new List<float>();

    private float replayTimer = 0f;
    private int currentIndex = 0;
    private bool isReplaying = false;

    void Start()
    {
        LoadGhostPath();

        if (ghostPositions.Count > 1)
        {
            transform.position = ghostPositions[0];
            transform.rotation = ghostRotations[0];
            gameObject.SetActive(true);
            StartReplay();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isReplaying)
        {
            return;
        }

        replayTimer += Time.deltaTime;

        while (currentIndex < ghostTimes.Count - 1 && replayTimer >= ghostTimes[currentIndex + 1])
        {
            currentIndex++;
        }

        if (currentIndex >= ghostTimes.Count - 1)
        {
            isReplaying = false;
            return;
        }

        float currentTime = ghostTimes[currentIndex];
        float nextTime = ghostTimes[currentIndex + 1];

        float t = 0f;

        if (nextTime > currentTime)
        {
            t = (replayTimer - currentTime) / (nextTime - currentTime);
        }

        transform.position = Vector3.Lerp(
            ghostPositions[currentIndex],
            ghostPositions[currentIndex + 1],
            t
        );

        transform.rotation = Quaternion.Slerp(
            ghostRotations[currentIndex],
            ghostRotations[currentIndex + 1],
            t
        );
    }

    public void StartReplay()
    {
        replayTimer = 0f;
        currentIndex = 0;
        isReplaying = true;
    }

    void LoadGhostPath()
    {
        ghostPositions.Clear();
        ghostRotations.Clear();
        ghostTimes.Clear();

        int count = PlayerPrefs.GetInt("GhostPointCount", 0);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                PlayerPrefs.GetFloat("GhostPosX_" + i),
                PlayerPrefs.GetFloat("GhostPosY_" + i),
                PlayerPrefs.GetFloat("GhostPosZ_" + i)
            );

            Quaternion rot = new Quaternion(
                PlayerPrefs.GetFloat("GhostRotX_" + i),
                PlayerPrefs.GetFloat("GhostRotY_" + i),
                PlayerPrefs.GetFloat("GhostRotZ_" + i),
                PlayerPrefs.GetFloat("GhostRotW_" + i)
            );

            float time = PlayerPrefs.GetFloat("GhostTime_" + i);

            ghostPositions.Add(pos);
            ghostRotations.Add(rot);
            ghostTimes.Add(time);
        }
    }
}