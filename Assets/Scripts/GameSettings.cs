using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A ScriptableObject class that holds game settings, including the current score and an event for when the score is updated.
/// The <c>OnScoreUpdated</c> event is invoked whenever the score changes, allowing other parts of the game to react to the update.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    public UnityEvent OnScoreUpdated;

    public int Score;
}