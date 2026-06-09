using UnityEngine;

public class JoystickButtonTester : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i <= 19; i++)
        {
            KeyCode button = KeyCode.JoystickButton0 + i;

            if (Input.GetKeyDown(button))
            {
                Debug.Log("Pressed: JoystickButton" + i);
            }
        }
    }
}