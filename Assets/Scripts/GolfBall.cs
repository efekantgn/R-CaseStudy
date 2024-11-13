using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallTier ballTier;
    [SerializeField] private GameSettings gameSettings;
    public GolfBallSpawner Spawner;
    public int BallScore;
    public UnityEvent OnBallPickUp;


    private void OnEnable()
    {
        OnBallPickUp.AddListener(DestroyOnPickup);
    }
    private void OnDisable()
    {
        OnBallPickUp.RemoveListener(DestroyOnPickup);
    }
    public void DestroyOnPickup()
    {
        UpdateScoreBoard();
        Destroy(gameObject, .3f);
        Spawner.SpawnedObjects.Remove(this);
    }

    private void UpdateScoreBoard()
    {
        gameSettings.Score += BallScore;
        gameSettings.OnScoreUpdated?.Invoke();
    }
}