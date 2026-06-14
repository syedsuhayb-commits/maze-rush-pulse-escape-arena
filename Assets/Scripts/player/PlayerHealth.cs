using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public GameObject damageFlash;

    public float restartDelay = 1.5f;

    private bool isDead = false;

    public Image healthFill;
    public Image delayedFill;

    public float delayedSpeed = 3f;

    void Start()
    {
        currentHealth = maxHealth;

        healthFill.fillAmount = 1f;
        delayedFill.fillAmount = 1f;

        if (damageFlash != null)
        {
            damageFlash.SetActive(false);
        }
    }

    void Update()
    {
        delayedFill.fillAmount = Mathf.Lerp(
        delayedFill.fillAmount,
        healthFill.fillAmount,
        Time.deltaTime * delayedSpeed
        );
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        healthFill.fillAmount =
            (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(DamageFlashEffect());
    }

    System.Collections.IEnumerator DamageFlashEffect()
    {
        if (damageFlash != null)
        {
            damageFlash.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            damageFlash.SetActive(false);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        healthFill.fillAmount = 0f;
        delayedFill.fillAmount = 0f;

        Invoke(nameof(RestartLevel), restartDelay);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}