using System;
using UnityEngine;

public class GameData : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static GameData Instance { get; private set; }
    public PlayerData Data { get; private set; } = new PlayerData();


    // EVENTS
    public event Action<int> OnCurrencyChanged;


    // METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Data = SaveSystem.Load();
    }

    public void AddCurrency(int amount)
    {
        Data.currency += amount;
        OnCurrencyChanged?.Invoke(Data.currency);
    }

    public void RemoveCurrency(int amount)
    {
        Data.currency -= amount;

        if (Data.currency < 0)
            Data.currency = 0;

        OnCurrencyChanged?.Invoke(Data.currency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (Data.currency < amount)
            return false;

        Data.currency -= amount;
        OnCurrencyChanged?.Invoke(Data.currency);
        return true;
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save(Data);
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveSystem.Save(Data);
    }
}
