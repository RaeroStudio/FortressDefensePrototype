using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    private int currentGold;

    public void Initialize(UIManager ui)
    {
        uiManager = ui;
        currentGold = 0;
        uiManager.UpdateGoldUI(currentGold);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        uiManager.UpdateGoldUI(currentGold);
    }
}