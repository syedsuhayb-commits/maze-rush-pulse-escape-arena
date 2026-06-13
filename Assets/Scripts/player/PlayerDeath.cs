using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    public Image bloodScreen;

    public float fadeSpeed = 3f;
    public float restartDelay = 1.2f;

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;

            bloodScreen.color = new Color(
                bloodScreen.color.r,
                bloodScreen.color.g,
                bloodScreen.color.b,
                alpha
            );

            yield return null;
        }

        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}