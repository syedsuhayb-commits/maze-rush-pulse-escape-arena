using UnityEngine;
using System.Collections.Generic;

public class GhostRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public Transform playerTransform;
    public float recordInterval = 0.1f;

    private float recordTimer = 0f;
    private float runTime = 0f;
    private bool isRecording = false;

    private List<Vector3> recordedPositions = new List<Vector3>();
    private List<Quaternion> recordedRotations = new List<Quaternion>();
    private List<float> recordedTimes = new List<float>();

    void Update()
    {
        if (!isRecording)
        {
            return;
        }

        runTime += Time.deltaTime;
        recordTimer += Time.deltaTime;

        if (recordTimer >= recordInterval)
        {
            RecordPoint();
            recordTimer = 0f;
        }
    }

    public void StartRecording()
    {
        recordedPositions.Clear();
        recordedRotations.Clear();
        recordedTimes.Clear();

        runTime = 0f;
        recordTimer = 0f;
        isRecording = true;

        RecordPoint();
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    void RecordPoint()
    {
        if (playerTransform == null)
        {
            return;
        }

        recordedPositions.Add(playerTransform.position);
        recordedRotations.Add(playerTransform.rotation);
        recordedTimes.Add(runTime);
    }

    public void SaveGhostPath(string ghostName, float ghostTime)
    {
        PlayerPrefs.SetInt("GhostPointCount", recordedPositions.Count);
        PlayerPrefs.SetString("GhostName", ghostName);
        PlayerPrefs.SetFloat("GhostBestTime", ghostTime);

        for (int i = 0; i < recordedPositions.Count; i++)
        {
            Vector3 pos = recordedPositions[i];
            Quaternion rot = recordedRotations[i];

            PlayerPrefs.SetFloat("GhostPosX_" + i, pos.x);
            PlayerPrefs.SetFloat("GhostPosY_" + i, pos.y);
            PlayerPrefs.SetFloat("GhostPosZ_" + i, pos.z);

            PlayerPrefs.SetFloat("GhostRotX_" + i, rot.x);
            PlayerPrefs.SetFloat("GhostRotY_" + i, rot.y);
            PlayerPrefs.SetFloat("GhostRotZ_" + i, rot.z);
            PlayerPrefs.SetFloat("GhostRotW_" + i, rot.w);

            PlayerPrefs.SetFloat("GhostTime_" + i, recordedTimes[i]);
        }

        PlayerPrefs.Save();

        Debug.Log("Ghost path saved for " + ghostName + " with " + recordedPositions.Count + " points.");
    }

    public int GetRecordedPointCount()
    {
        return recordedPositions.Count;
    }
}