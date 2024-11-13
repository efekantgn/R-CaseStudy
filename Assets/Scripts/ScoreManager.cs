using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public UnityEvent<string> OnScoreBoardUpdated;

    [SerializeField] private GameSettings gameSettings;

    private void OnEnable()
    {
        gameSettings.OnScoreUpdated.AddListener(UpdateScoreBoard);
    }
    private void OnDisable()
    {
        gameSettings.OnScoreUpdated.RemoveListener(UpdateScoreBoard);
    }
    public void UpdateScoreBoard()
    {
        OnScoreBoardUpdated?.Invoke(gameSettings.Score.ToString("N0"));
    }
}
