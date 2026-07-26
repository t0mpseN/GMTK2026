using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeArrows : MonoBehaviour
{
    [SerializeField] private RectTransform _arrowPrefab;
    [SerializeField] private RectTransform _container;
    [SerializeField] private float _padding = 60f;
    private UpgradeNode[] _nodes;
    private readonly List<(RectTransform arrow, UpgradeNode target)> _arrows = new();

    private void Start()
    {
        _nodes = _container.GetComponentsInChildren<UpgradeNode>(true);
        BuildArrows();
        Refresh();
    }

    public void Refresh()
    {
        foreach (var (arrow, target) in _arrows)
        {
            bool unlocked = UpgradeSystem.Instance.ArePrerequisitesMet(target.Definition.Id);
            arrow.gameObject.SetActive(unlocked);
        }
    }

    private void BuildArrows()
    {
        foreach (UpgradeNode node in _nodes)
        {
            if (node.Definition == null) continue;

            foreach (UpgradeRequirement requirement in node.Definition.Requirements)
            {
                UpgradeNode source = FindNode(requirement.upgrade);
                if (source == null) continue;

                RectTransform arrow = CreateArrow(
                    source.GetComponent<RectTransform>(),
                    node.GetComponent<RectTransform>());

                _arrows.Add((arrow, node));
            }
        }
    }

    private UpgradeNode FindNode(UpgradeId id)
    {
        foreach (UpgradeNode node in _nodes)
        {
            if (node.Definition != null && node.Definition.Id == id)
                return node;
        }

        return null;
    }

    private RectTransform CreateArrow(RectTransform from, RectTransform to)
    {
        RectTransform arrow = Instantiate(_arrowPrefab, _container);
        arrow.SetAsFirstSibling(); // atrás dos nós

        Vector2 direction = to.anchoredPosition - from.anchoredPosition;
        float distance = direction.magnitude - _padding;

        arrow.anchoredPosition = from.anchoredPosition + direction.normalized * (_padding * 0.5f);
        arrow.sizeDelta = new Vector2(distance, arrow.sizeDelta.y);
        arrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        return arrow;
    }
}