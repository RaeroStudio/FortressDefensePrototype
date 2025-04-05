using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGame : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Citadel citadel;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private UIManager uiManager;

    void Start()
    {
        
        citadel.Initialize(uiManager);
        goldManager.Initialize(uiManager);
        waveManager.GetComponent<WaveManager>().enabled = true;
        StartNewGame();
    }

    public void StartNewGame()
    {   
        Debug.Log("Начало игры");
        citadel.AddSettlers(Random.Range(1,2));
    }
}