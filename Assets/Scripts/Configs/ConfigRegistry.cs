using UnityEngine;

public class ConfigRegistry : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static ConfigRegistry Instance { get; private set; }

    [Header("Economy Parameters")]
    [SerializeField] private EconomyConfig _economy;
    public EconomyConfig Economy => _economy;

    [Header("Weapon Parameters")]
    [SerializeField] private WeaponConfig _weapon;
    public WeaponConfig Weapon => _weapon;

    [Header("Run Parameters")]
    [SerializeField] private RunConfig _run;
    public RunConfig Run => _run;

    [Header("Spawn Parameters")]
    [SerializeField] private SpawnConfig _spawn;
    public SpawnConfig Spawn => _spawn;


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
    }
}