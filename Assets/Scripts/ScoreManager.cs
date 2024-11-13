using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public UnityEvent<string> OnScoreUpdated;

    private int score = 0;

    public void AddScore(int amount)
    {
        score += amount;
        // Skor güncelleme eventini tetikle
        OnScoreUpdated?.Invoke(score.ToString("N0"));
    }
}
