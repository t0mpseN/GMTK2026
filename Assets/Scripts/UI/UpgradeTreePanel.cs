using UnityEngine;

public class UpgradeTreePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    private UpgradeNode[] _nodes;

    private void Awake()
    {
        _nodes = _root.GetComponentsInChildren<UpgradeNode> (true);
    }

    private void Start()
    {
        UpgradeSystem.Instance.OnUpgradePurchased += HandleChanged;
        UpgradeSystem.Instance.OnUpgradesReset += RefreshAll;
        GameData.Instance.OnCurrencyChanged += HandleCurrencyChanged;
        Close();
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnUpgradePurchased -= HandleChanged;
            UpgradeSystem.Instance.OnUpgradesReset -= RefreshAll;
        }
        if (GameData.Instance != null)
            GameData.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
    }

    public void Open() => _root.SetActive(true);
    public void Close() => _root.SetActive(false);

    private void RefreshAll()
    {
        foreach (UpgradeNode node in _nodes)
            node.Refresh();
    }

    private void HandleChanged(UpgradeId id, int level) => RefreshAll();
    private void HandleCurrencyChanged(int currency) => RefreshAll();
}