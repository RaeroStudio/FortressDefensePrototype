using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Здоровье цитадели")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText; // Добавляем текстовое поле

    [Header("Другие элементы UI")]
    [SerializeField] private Text settlersText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text enemiesLeftText;
    [SerializeField] private Text goldText;
    [SerializeField] private GameObject preparationPanel;
    [SerializeField] private Text timerText;

    public void UpdateSettlersUI(int current, int max)
    {
        
        settlersText.text = $"Поселенцы: {current}/{max}";
    }

    public void UpdateWaveUI(int wave, int enemies)
    {
        waveText.text = $"Волна: {wave}";
        enemiesLeftText.text = $"Врагов: {enemies}";
    }

    public void UpdateGoldUI(int gold)
    {
        goldText.text = $"Золото: {gold}";
    }

    public void UpdateHealthUI(float health, float maxHealth)
    {
        healthSlider.value = health / maxHealth;
        healthText.text = $"{Mathf.CeilToInt(health)}/{Mathf.CeilToInt(maxHealth)}";
    }

    public void TogglePreparationPanel(bool state)
    {
        preparationPanel.SetActive(state);
    }

    public void UpdateTimerUI(int seconds)
    {
        timerText.text = $"Подготовка: {seconds} сек";
    }
}