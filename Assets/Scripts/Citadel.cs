using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Citadel : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private int maxSettlers = 10;
    [SerializeField] private GameObject settlerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private UIManager uiManager;

    private float currentHealth;
    private int currentSettlers;
 public Vector3 Position => transform.position; public void Initialize(UIManager ui)
{
    uiManager = ui;
    currentHealth = maxHealth;
    UpdateHealthUI(); // Добавьте эту строку
}

     public void TakeDamage(float damage)
    {
        currentHealth -= damage;
           Debug.Log($"Урон получен! Текущее здоровье: {currentHealth}"); // Добавьте это
        UpdateHealthUI();
        
        if(currentHealth <= 0) Defeat();
    }

    private void UpdateHealthUI()
    {
        uiManager.UpdateHealthUI(currentHealth, maxHealth);
    }

    private void Defeat()
    {
        Time.timeScale = 0;
        Debug.Log("Цитадель уничтожена!");
    }

    public void AddSettlers(int amount)
    {
        int availableSlots = maxSettlers - currentSettlers;
        int toAdd = Mathf.Min(amount, availableSlots);
        
        if(toAdd > 0)
        {
            currentSettlers += toAdd;
            SpawnSettlers(toAdd);
            uiManager.UpdateSettlersUI(currentSettlers, maxSettlers);
        }
    }

    private void SpawnSettlers(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 pos = spawnPoint.position + Random.insideUnitSphere * 2f;
            Instantiate(settlerPrefab, pos, Quaternion.identity);
        }
    }

 private void UpdateSettlersUI()
    {
        if(uiManager != null)
        {
            uiManager.UpdateSettlersUI(currentSettlers, maxSettlers);
        }
    }
      public void RemoveSettler(int amount)
    {
        currentSettlers = Mathf.Max(currentSettlers - amount, 0);
        UpdateSettlersUI(); // Теперь метод существует
    }
}
