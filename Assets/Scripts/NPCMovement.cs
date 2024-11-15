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
    [SerializeField] private float criticalHealth = 5;
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

            float healthLossToCart = GetPathDistance(ball.transform.position, GolfCart.transform.position) * GetHealthLossPerUnitDistance(moveSpeed);

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
            float totalDistance = Vector3.Distance(startPoint, path.corners[0]);
            for (int i = 1; i < path.corners.Length; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return totalDistance;
        }

        return float.MinValue;
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
        Debug.Log("t:" + t, t);
        return t;
    }

    public void CalculateGreedyPath()
    {
        List<GolfBall> remainingGolfBalls = new List<GolfBall>(Spawner.SpawnedObjects);

        float healthLossOnPickUpAnim = animationController.GetTimeAnimationClip("Picking Up") * healthController.GetDrainRate();
        float healthLossOnDropAnim = animationController.GetTimeAnimationClip("Drop") * healthController.GetDrainRate();
        float healthLossPerUnitDistance = GetHealthLossPerUnitDistance(moveSpeed);

        while (remainingGolfBalls.Count > 0)
        {

            GolfBall mostValuableTransform = null;
            float bestScore = float.MinValue;

            foreach (GolfBall golfBall in remainingGolfBalls)
            {
                float distanceToBall = GetPathDistance(transform.position, golfBall.transform.position);
                Debug.Log($"{gameObject.name}-{golfBall.name} distance:{distanceToBall}", golfBall);
                float healthLossOnDistanceToBall = healthLossPerUnitDistance * distanceToBall;

                float distancePlayerToCart = GetPathDistance(golfBall.transform.position, GolfCart.transform.position);
                Debug.Log($"{golfBall.name}-{GolfCart.name} distance:{distancePlayerToCart}", GolfCart);
                if (distancePlayerToCart == float.MinValue)
                {
                    continue;
                }
                float healthLossOnDistancePlayerToCart = healthLossPerUnitDistance * distancePlayerToCart + healthLossOnPickUpAnim + healthLossOnDropAnim + criticalHealth + healthLossOnDistanceToBall;

                if (healthLossOnDistancePlayerToCart > healthController.GetCurrentHealth())
                {
                    continue;
                }
                float totalDistance = distancePlayerToCart + distanceToBall;

                float score = golfBall.BallScore / totalDistance;
                if (score > bestScore)
                {
                    bestScore = score;
                    mostValuableTransform = golfBall;
                }
            }
            if (mostValuableTransform == null)
            {
                remainingGolfBalls.Clear();
                OptimalPath.Add(GolfCart.transform);
                return;
            }
            else
            {
                remainingGolfBalls.Remove(mostValuableTransform);
                OptimalPath.Add(mostValuableTransform.transform);

            }

        }
    }



    float TotalHealthLossToPoint(Transform point)
    {
        float healthLossOnPickUpAnim = animationController.GetTimeAnimationClip("Picking Up") * healthController.GetDrainRate();
        float healthLossOnDropAnim = animationController.GetTimeAnimationClip("Drop") * healthController.GetDrainRate();
        float healthLossPerUnitDistance = GetHealthLossPerUnitDistance(moveSpeed);
        // Topa ulaşma mesafesi
        float distanceToPoint = GetPathDistance(transform.position, point.position);

        float healthLossToBall = distanceToPoint * healthLossPerUnitDistance + healthLossOnPickUpAnim;

        // Arabaya dönüş mesafesi
        float distanceToCart = GetPathDistance(point.position, GolfCart.transform.position);

        float healthLossToCart = distanceToCart * healthLossPerUnitDistance + healthLossOnDropAnim;

        return healthLossToBall + healthLossToCart;
    }

    void OnDrawGizmos()
    {
        // Listeyi kontrol et
        if (OptimalPath != null && OptimalPath.Count > 1)
        {
            for (int i = 0; i < OptimalPath.Count - 1; i++)
            {
                // Her elemanı bir sonrakine bağla
                if (OptimalPath[i] != null)
                    Debug.DrawLine(OptimalPath[i].transform.position, OptimalPath[i + 1].transform.position, Color.red);
            }
        }
    }
    #endregion
}
