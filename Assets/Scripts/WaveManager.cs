using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float preparationTime = 60f;
    
    [Header("Ссылки")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Citadel citadel; // Компонент Citadel
    [SerializeField] private GoldManager goldManager;
    

    private int currentWave = 0;
    private int enemiesLeft = 0;
    private List<EnemyAI> activeEnemies = new List<EnemyAI>();
    void Start() => StartCoroutine(WaveSystem());

    private IEnumerator WaveSystem()
    {
         while(true)
        {
            uiManager.TogglePreparationPanel(true);
            
            float timer = preparationTime;
            while(timer > 0)
            {
                uiManager.UpdateTimerUI(Mathf.CeilToInt(timer));
                timer -= Time.deltaTime;
                yield return null;
            }
            
            uiManager.TogglePreparationPanel(false);
            SpawnWave();
            
            // Измененное условие ожидания
            yield return new WaitUntil(() => activeEnemies.Count == 0);
            
            OnWaveCompleted();
        }
    }
    

    private void SpawnWave()
    {
        currentWave++;
        enemiesLeft = currentWave * 5; // Устанавливаем базовое значение
        activeEnemies.Clear();

        for(int i = 0; i < enemiesLeft; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], 
                spawnPoint.position, 
                Quaternion.identity
            );
            
            var enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.citadel = citadel;
            enemyAI.GoldManager = goldManager;
            
            // Измененная подписка на событие
            enemyAI.OnEnemyDeath += () => 
            {
                enemiesLeft--;
                activeEnemies.Remove(enemyAI);
                uiManager.UpdateWaveUI(currentWave, enemiesLeft);
            };
            
            activeEnemies.Add(enemyAI);
        }

        uiManager.UpdateWaveUI(currentWave, enemiesLeft);
    }

    private void HandleEnemyDeath()
    {
       enemiesLeft--;
    
    // Удаляем null элементы из списка
    activeEnemies.RemoveAll(item => item == null);
    
    uiManager.UpdateWaveUI(currentWave, enemiesLeft);
    
    if(enemiesLeft <= 0)
    {
        OnWaveCompleted();
    }
    }

    private void OnWaveCompleted()
    {
        goldManager.AddGold(Random.Range(1, 300));
        citadel.AddSettlers(Random.Range(1, 5));
    }
}