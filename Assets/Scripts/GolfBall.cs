using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallTier ballTier;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private TextEffect3DController effectPrefab;
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
        PlayTextEffect();
        Destroy(gameObject, .3f);
        Spawner.SpawnedObjects.Remove(this);
    }

    private void PlayTextEffect()
    {
        TextEffect3DController te3d = Instantiate(effectPrefab, transform.position, transform.rotation);
        te3d.gameObject.transform.SetParent(null);
        te3d.SetText(BallScore.ToString());
    }

    private void UpdateScoreBoard()
    {
        gameSettings.Score += BallScore;
        gameSettings.OnScoreUpdated?.Invoke();

    }

}