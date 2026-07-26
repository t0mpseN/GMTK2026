using UnityEngine;

public class UpgradeTreePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private bool _refundOnReset = true;
    private UpgradeNode[] _nodes;
    [SerializeField] private UpgradeTreeArrows _arrows;

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

    public void Open()
    {
        _root.SetActive(true);
        RefreshAll();
    }

    public void Close() => _root.SetActive(false);

    public void OnResetPressed()
    {
        UpgradeSystem.Instance.ResetUpgrades(_refundOnReset);
    }

    private void RefreshAll()
    {
        foreach (UpgradeNode node in _nodes)
            node.Refresh();

        _arrows?.Refresh();
    }

    private void HandleChanged(UpgradeId id, int level) => RefreshAll();
    private void HandleCurrencyChanged(float currency) => RefreshAll();
}