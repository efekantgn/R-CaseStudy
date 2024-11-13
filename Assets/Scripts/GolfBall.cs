using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallTier ballTier;
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
        Destroy(gameObject, 2);
        Spawner.SpawnedObjects.Remove(this);
    }
}