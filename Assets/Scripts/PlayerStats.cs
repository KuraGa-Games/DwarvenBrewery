using UnityEngine;
using UnityEngine.UI; // Добавь это, если будешь использовать UI элементы для здоровья (например, Slider)

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    // public Slider healthSlider; // Раскомментируй, если у тебя есть UI Slider для здоровья

    [Header("Mana (for Abilities)")]
    public int maxMana = 100;
    public int currentMana;
    // public Slider manaSlider; // Раскомментируй, если у тебя есть UI Slider для маны

    // Можно добавить другие статы, если они нужны по GDD
    // public int baseAttackDamage = 15; // Если урон Илурина может меняться

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHealthUI();
        UpdateManaUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Уже мертв, не наносим урон

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Не даем здоровью уйти ниже 0

        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateManaUI();
            Debug.Log(gameObject.name + " used " + amount + " mana. Current mana: " + currentMana);
            return true;
        }
        Debug.Log(gameObject.name + " not enough mana. Required: " + amount + ", Has: " + currentMana);
        return false;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
        Debug.Log(gameObject.name + " restored " + amount + " mana. Current mana: " + currentMana);
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        Debug.Log(gameObject.name + " restored " + amount + " health. Current health: " + currentHealth);
    }


    void Die()
    {
        Debug.Log(gameObject.name + " Died! Game Over!");
        // Здесь должна быть логика проигрыша игры согласно твоему GDD:
        // - Показать экран "Урки победили..."
        // - Возможно, остановить игру, предложить загрузить сохранение или выйти в меню
        // Пример:
        // Time.timeScale = 0; // Остановить время в игре
        // FindObjectOfType<GameManager>()?.HandlePlayerDeath(); // Если есть GameManager для этого

        // Пока можно просто отключить игрока или его управление
        // gameObject.SetActive(false); // Или более сложная логика
    }

    void UpdateHealthUI()
    {
        // if (healthSlider != null)
        // {
        //     healthSlider.value = (float)currentHealth / maxHealth;
        // }
        // Здесь ты будешь обновлять UI здоровья, если он у тебя есть
    }

    void UpdateManaUI()
    {
        // if (manaSlider != null)
        // {
        //     manaSlider.value = (float)currentMana / maxMana;
        // }
        // Здесь ты будешь обновлять UI маны, если он у тебя есть
    }
}