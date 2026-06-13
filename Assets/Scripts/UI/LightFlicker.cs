using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light lightSource;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        lightSource.intensity = Random.Range(1.2f, 2f);
    }
}