using UnityEngine;

public class ExitDoorController : MonoBehaviour
{
    [Header("Door Pivot")]
    public Transform doorPivot;

    [Header("Door Open Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool openToRight = true;

    [Header("Exit Trigger Collider")]
    public Collider exitTriggerCollider;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isUnlocked = false;

    void Start()
    {
        if (doorPivot != null)
        {
            closedRotation = doorPivot.localRotation;

            float angle = openToRight ? openAngle : -openAngle;
            openRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);
        }

        if (exitTriggerCollider != null)
        {
            exitTriggerCollider.enabled = false;
        }
    }

    void Update()
    {
        if (isUnlocked && doorPivot != null)
        {
            doorPivot.localRotation = Quaternion.Slerp(
                doorPivot.localRotation,
                openRotation,
                openSpeed * Time.deltaTime
            );
        }
    }

    public void UnlockDoor()
    {
        if (isUnlocked)
            return;

        isUnlocked = true;

        if (exitTriggerCollider != null)
        {
            exitTriggerCollider.enabled = true;
        }

        Debug.Log("Door unlocked and rotating open.");
    }
}