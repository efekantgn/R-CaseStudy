using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    public UnityEvent OnScoreUpdated;
    public int Score;
}