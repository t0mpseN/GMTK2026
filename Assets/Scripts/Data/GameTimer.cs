using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static GameTimer Instance { get; private set; }
    protected virtual float StartingTime => ConfigRegistry.Instance.Run.StartingTime + UpgradeSystem.Instance.GetValue(UpgradeId.TimePerRun);

    public float TimeRemaining { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;


    // METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        TimeRemaining = StartingTime;
    }

    private void Start()
    {
        IsRunning = true;
        OnTimeChanged?.Invoke(TimeRemaining);
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsRunning = false;
            OnTimeChanged?.Invoke(TimeRemaining);
            OnTimeExpired?.Invoke();
            return;
        }

        OnTimeChanged?.Invoke(TimeRemaining);
    }

    public void AddTime(float seconds)
    {
        if (seconds <= 0f)
            return;

        TimeRemaining = Mathf.Min(TimeRemaining + seconds, StartingTime);
        OnTimeChanged?.Invoke(TimeRemaining);
    }
}
