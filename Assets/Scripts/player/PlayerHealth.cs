using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public GameObject damageFlash;

    public float restartDelay = 1.5f;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (damageFlash != null)
        {
            damageFlash.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        Debug.Log("PLAYER TOOK DAMAGE. Health = " + currentHealth);

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

        Debug.Log("PLAYER DEAD. Health = 0");

        Invoke(nameof(RestartLevel), restartDelay);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}