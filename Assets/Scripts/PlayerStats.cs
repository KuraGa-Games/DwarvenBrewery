using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Mana (for Abilities)")]
    public int maxMana = 100;
    public int currentMana;

    [Header("Animation")]
    private Animator animator;
    public float playerDeathAnimationDuration = 2.0f;

    [Header("Game Over Settings (In-Script)")]
    public KeyCode restartKey = KeyCode.R; // ������� ��� �����������
    private bool isGameOver = false; // ����, ��� ���� ��������

    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("PlayerStats: ��������� Animator �� ������ �� ������ " + gameObject.name);
        }
        // UI ���������� ���� ����������������, ���� ��� �� ������������
        // UpdateHealthUI();
        // UpdateManaUI();
    }

    // ������� Update ��� �������� ����������� ����� Game Over
    void Update()
    {
        if (isGameOver && Input.GetKeyDown(restartKey))
        {
            RestartLevel();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);
        // UpdateHealthUI();

        if (animator != null)
        {
            animator.SetTrigger("HurtTrigger");
            // Debug.Log(gameObject.name + " HurtTrigger set.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool UseMana(int amount)
    {
        if (isDead) return false;
        if (currentMana >= amount)
        {
            currentMana -= amount;
            // UpdateManaUI();
            return true;
        }
        return false;
    }

    // ... (RestoreMana, RestoreHealth - ���� ��� ����) ...

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " Died!");

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            // animator.ResetTrigger("AttackTrigger"); // ���� ���� ����� � ������
            animator.ResetTrigger("HurtTrigger");
            animator.SetTrigger("DieTrigger");
            // Debug.Log(gameObject.name + " DieTrigger set.");
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false; // ��������� ����������
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ��������� ��������, ������� ���������� "Game Over" ����� �������� ������
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // ���� ��������� �������� ������
        yield return new WaitForSeconds(playerDeathAnimationDuration);

        Debug.Log("Game Over! Press '" + restartKey.ToString() + "' to Restart.");
        isGameOver = true;
        Time.timeScale = 0f; // ������������� ���� (�����������, �� ����� ��������)

        // ����� ����� ���� �� ������������ UI ������� "Game Over", ���� �� �� ���.
        // ���� ����� �������� ��������� � ������� � � OnGUI.
    }

    // ��������� ������ ���������� "Game Over" �� ������ ��� UI ���������
    void OnGUI()
    {
        if (isGameOver)
        {
            // ����� ��� ������
            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            // ������������ ������������� ��� ������ �� ������ ������
            float width = 400;
            float height = 100;
            float x = (Screen.width - width) / 2;
            float y = (Screen.height - height) / 2;
            Rect rect = new Rect(x, y, width, height);

            GUI.Label(rect, "Game Over!\nPress '" + restartKey.ToString() + "' to Restart", style);
        }
    }

    void RestartLevel()
    {
        Time.timeScale = 1f; // ����������� ��������������� ����� ����� ��������� ����� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // void UpdateHealthUI() { /* ... */ }
    // void UpdateManaUI() { /* ... */ }
}