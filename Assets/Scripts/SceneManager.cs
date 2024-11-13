using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;
    public void RaiseLoadScene()
    {
        SetGameScore(0);
        SceneManager.LoadScene(0);
    }

    public void SetGameScore(int value)
    {
        gameSettings.Score = 0;
    }
}