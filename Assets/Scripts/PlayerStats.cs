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
    public KeyCode restartKey = KeyCode.R; // Клавиша для перезапуска
    private bool isGameOver = false; // Флаг, что игра окончена

    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("PlayerStats: Компонент Animator не найден на игроке " + gameObject.name);
        }
        // UI обновления пока закомментированы, если они не используются
        // UpdateHealthUI();
        // UpdateManaUI();
    }

    // Добавим Update для проверки перезапуска после Game Over
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

    // ... (RestoreMana, RestoreHealth - если они есть) ...

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " Died!");

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            // animator.ResetTrigger("AttackTrigger"); // Если есть атака у игрока
            animator.ResetTrigger("HurtTrigger");
            animator.SetTrigger("DieTrigger");
            // Debug.Log(gameObject.name + " DieTrigger set.");
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false; // Отключаем управление
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Запускаем корутину, которая обработает "Game Over" после анимации смерти
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // Ждем окончания анимации смерти
        yield return new WaitForSeconds(playerDeathAnimationDuration);

        Debug.Log("Game Over! Press '" + restartKey.ToString() + "' to Restart.");
        isGameOver = true;
        Time.timeScale = 0f; // Останавливаем игру (опционально, но часто делается)

        // Здесь можно было бы активировать UI элемент "Game Over", если бы он был.
        // Пока будем выводить сообщение в консоль и в OnGUI.
    }

    // Временный способ отобразить "Game Over" на экране без UI элементов
    void OnGUI()
    {
        if (isGameOver)
        {
            // Стиль для текста
            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            // Рассчитываем прямоугольник для текста по центру экрана
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
        Time.timeScale = 1f; // ОБЯЗАТЕЛЬНО восстанавливаем время перед загрузкой новой сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // void UpdateHealthUI() { /* ... */ }
    // void UpdateManaUI() { /* ... */ }
}