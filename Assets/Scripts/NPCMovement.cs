using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float carrySpeed = 2.5f;
    public GolfCart GolfCart;
    public GolfBallSpawner Spawner;
    public UnityEvent OnStartMoving;
    public UnityEvent OnStopMoving;

    private NavMeshAgent navMeshAgent;
    private NPCAnimationController animationController;
    private NPCHealthController healthController;
    private bool isMoving = false;

    private void Awake()
    {
        animationController = GetComponent<NPCAnimationController>();
        healthController = GetComponentInChildren<NPCHealthController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        navMeshAgent.speed = moveSpeed;

    }

    void Update()
    {
        if (isMoving && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            StopMoving();
        }
    }

    [ContextMenu("GoCart")]
    public void GoToGolfCart()
    {
        StartMoving(GolfCart.transform.position);
    }
    public void GoToNewTarget()
    {
        Transform t = SelectFromGreedy();
        if (t == null)
        {
            return;
        }

        StartMoving(t.position);
    }

    // Hareket başlatma fonksiyonu
    public void StartMoving(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        isMoving = true;
        OnStartMoving?.Invoke();
    }

    // Hareket durdurma fonksiyonu
    public void StopMoving()
    {
        navMeshAgent.ResetPath();
        isMoving = false;
        OnStopMoving?.Invoke();
    }

    public Transform GetOptimalGolfBall()
    {
        GolfBall optimalBall = null;
        float bestScore = float.MinValue;
        float healthLossOnPickUpAnim = animationController.GetTimeAnimationClip("Picking Up") * healthController.GetDrainRate();
        float healthLossOnDropAnim = animationController.GetTimeAnimationClip("Drop") * healthController.GetDrainRate();

        foreach (GolfBall ball in Spawner.SpawnedObjects)
        {
            float healthLossToBall = GetPathDistance(transform.position, ball.transform.position) * GetHealthLossPerUnitDistance(moveSpeed);

            float healthLossToCart = GetPathDistance(ball.transform.position, GolfCart.transform.position) * GetHealthLossPerUnitDistance(carrySpeed);

            float totalHealthLoss = healthLossToBall + healthLossToCart + healthLossOnDropAnim + healthLossOnPickUpAnim;

            if (totalHealthLoss > healthController.GetCurrentHealth()) continue;

            float score = ball.BallScore / totalHealthLoss;

            if (score > bestScore)
            {
                bestScore = score;
                optimalBall = ball;
            }
        }
        return optimalBall.transform;
    }

    public float GetHealthLossPerUnitDistance(float movementSpeed)
    {
        return healthController.GetDrainRate() / movementSpeed;
    }

    float GetPathDistance(Vector3 startPoint, Vector3 endPoint)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                float totalDistance = 0f;
                for (int i = 1; i < path.corners.Length; i++)
                {
                    totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                return totalDistance;
            }
        }
        return -1f;
    }
    #region TSP Greedy solution ON TODO
    public List<Transform> OptimalPath = new();

    public Transform SelectFromGreedy()
    {
        if (OptimalPath.Count > 0)
            OptimalPath.Clear();

        CalculateGreedyPath();
        Transform t = OptimalPath[0];
        OptimalPath.RemoveAt(0);
        return t;
    }

    public void CalculateGreedyPath()
    {
        List<GolfBall> remainingPoints = new List<GolfBall>(Spawner.SpawnedObjects);

        while (remainingPoints.Count > 0)
        {

            GolfBall mostValueableGolfBall = null;
            float bestScore = float.MinValue;

            foreach (GolfBall golfBall in remainingPoints)
            {
                // Topa olan mesafeyi hesapla
                float totalHealthLoss = TotalHealthLossToPoint(golfBall.transform);

                // Eğer toplam sağlık kaybı mevcut sağlıktan fazlaysa, bu noktayı atla
                if (totalHealthLoss > healthController.GetCurrentHealth())
                {
                    continue;
                }


                float score = golfBall.BallScore / totalHealthLoss;

                if (score > bestScore)
                {
                    bestScore = score;
                    mostValueableGolfBall = golfBall;
                }


            }
            // Eğer hiçbir uygun nokta yoksa golf arabasına dön
            if (mostValueableGolfBall == null)
            {
                Debug.Log("No valid points left to visit. Returning to cart.");
                OptimalPath.Clear();
                OptimalPath.Add(GolfCart.transform);
                return;
            }

            // Seçilen noktayı rota listesine ekle ve remainingPoints listesinden kaldır
            Debug.Log($"{mostValueableGolfBall.name}: {bestScore}", mostValueableGolfBall.gameObject);

            OptimalPath.Add(mostValueableGolfBall.transform);
            remainingPoints.Remove(mostValueableGolfBall);
        }
        Debug.Log($"{OptimalPath[0].name}", OptimalPath[0].gameObject);

    }

    private float TotalHealthLossToPoint(Transform point)
    {
        float healthLossOnPickUpAnim = animationController.GetTimeAnimationClip("Picking Up") * healthController.GetDrainRate();
        float healthLossOnDropAnim = animationController.GetTimeAnimationClip("Drop") * healthController.GetDrainRate();
        float healthLossPerUnitDistance = GetHealthLossPerUnitDistance(moveSpeed);

        float distanceToPoint = GetPathDistance(transform.position, point.position);
        float healthLossToBall = distanceToPoint * healthLossPerUnitDistance + healthLossOnPickUpAnim;

        // Topu aldıktan sonra golf arabasına dönüş mesafesini hesapla
        float distanceToCart = GetPathDistance(point.position, GolfCart.transform.position);
        float healthLossToCart = distanceToCart * healthLossPerUnitDistance + healthLossOnDropAnim;

        // Toplam sağlık kaybını hesapla
        float totalHealthLoss = healthLossToBall + healthLossToCart;
        return totalHealthLoss;
    }

    void OnDrawGizmos()
    {
        // Listeyi kontrol et
        if (OptimalPath != null && OptimalPath.Count > 1)
        {
            for (int i = 0; i < OptimalPath.Count - 1; i++)
            {
                // Her elemanı bir sonrakine bağla
                Debug.DrawLine(OptimalPath[i].transform.position, OptimalPath[i + 1].transform.position, Color.red);
            }
        }
    }
    #endregion
}
