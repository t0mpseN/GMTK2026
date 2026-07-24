using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    [SerializeField] private string _menuSceneName = "Menu";

    [Tooltip("A pause before returning to the menu scene, in seconds.")]
    [SerializeField] private float _returnToMenuDelay = 1.5f;

    private void Start()
    {
        GameTimer.Instance.OnTimeExpired += HandleRunEnded;
    }

    private void OnDestroy()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimeExpired -= HandleRunEnded;
    }

    private void HandleRunEnded()
    {
        StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        SaveSystem.Save(GameData.Instance.Data);

        yield return new WaitForSeconds(_returnToMenuDelay);

        SceneManager.LoadScene(_menuSceneName);
    }
}
