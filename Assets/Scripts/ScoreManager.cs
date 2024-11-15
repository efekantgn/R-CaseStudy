using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public UnityEvent<string> OnTotalScoreUpdated;
    public UnityEvent<string> OnCharacterScoreUpdated;

    [SerializeField] private GameSettings gameSettings;

    private void OnEnable()
    {
        gameSettings.OnScoreUpdated.AddListener(OnScoreUpdated);
    }
    private void OnDisable()
    {
        gameSettings.OnScoreUpdated.RemoveListener(OnScoreUpdated);
    }

    private void OnScoreUpdated()
    {
        OnCharacterScoreUpdated?.Invoke(gameSettings.Score.ToString("N0"));
    }

    public void UpdateScoreBoard()
    {
        OnTotalScoreUpdated?.Invoke(gameSettings.Score.ToString("N0"));
    }

}
