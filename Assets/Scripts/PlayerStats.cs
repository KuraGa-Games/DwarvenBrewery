using UnityEngine;
using UnityEngine.UI; // ������ ���, ���� ������ ������������ UI �������� ��� �������� (��������, Slider)

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    // public Slider healthSlider; // ��������������, ���� � ���� ���� UI Slider ��� ��������

    [Header("Mana (for Abilities)")]
    public int maxMana = 100;
    public int currentMana;
    // public Slider manaSlider; // ��������������, ���� � ���� ���� UI Slider ��� ����

    // ����� �������� ������ �����, ���� ��� ����� �� GDD
    // public int baseAttackDamage = 15; // ���� ���� ������� ����� ��������

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHealthUI();
        UpdateManaUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // ��� �����, �� ������� ����

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // �� ���� �������� ���� ���� 0

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
        // ����� ������ ���� ������ ��������� ���� �������� ������ GDD:
        // - �������� ����� "���� ��������..."
        // - ��������, ���������� ����, ���������� ��������� ���������� ��� ����� � ����
        // ������:
        // Time.timeScale = 0; // ���������� ����� � ����
        // FindObjectOfType<GameManager>()?.HandlePlayerDeath(); // ���� ���� GameManager ��� �����

        // ���� ����� ������ ��������� ������ ��� ��� ����������
        // gameObject.SetActive(false); // ��� ����� ������� ������
    }

    void UpdateHealthUI()
    {
        // if (healthSlider != null)
        // {
        //     healthSlider.value = (float)currentHealth / maxHealth;
        // }
        // ����� �� ������ ��������� UI ��������, ���� �� � ���� ����
    }

    void UpdateManaUI()
    {
        // if (manaSlider != null)
        // {
        //     manaSlider.value = (float)currentMana / maxMana;
        // }
        // ����� �� ������ ��������� UI ����, ���� �� � ���� ����
    }
}