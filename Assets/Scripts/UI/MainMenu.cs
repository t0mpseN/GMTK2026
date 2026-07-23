using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private string _gameSceneName = "Game";


    // METHODS
    public void OnPlayPressed()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}
