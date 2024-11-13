using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

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
        GolfBall gb = GetOptimalGolfBall();
        if (gb == null)
        {
            Debug.Log("topu arabaya getiremeden ölecek.");
            return;
        }

        StartMoving(gb.transform.position);
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

    public GolfBall GetOptimalGolfBall()
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
        return optimalBall;
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
    // public List<Transform> OptimalPath = new();

    // [ContextMenu("Greedy")]
    // public void CalculateGreedyPath()
    // {
    //     List<Transform> remainingPoints = new List<Transform>(Spawner.SpawnedObjects.ConvertAll(ball => ball.transform));
    //     Transform currentPoint = transform;

    //     while (remainingPoints.Count > 0)
    //     {
    //         Transform closestPoint = null;
    //         float closestDistance = float.MaxValue;

    //         foreach (Transform point in remainingPoints)
    //         {
    //             float distance = Vector3.Distance(currentPoint.position, point.position);
    //             if (distance < closestDistance)
    //             {
    //                 closestDistance = distance;
    //                 closestPoint = point;
    //             }
    //         }

    //         if (closestPoint != null)
    //         {
    //             OptimalPath.Add(closestPoint);
    //             remainingPoints.Remove(closestPoint);
    //             currentPoint = closestPoint;
    //         }
    //     }

    //     OptimalPath.Add(GolfCart.transform);
    // }
    // void OnDrawGizmos()
    // {
    //     // Listeyi kontrol et
    //     if (OptimalPath != null && OptimalPath.Count > 1)
    //     {
    //         for (int i = 0; i < OptimalPath.Count - 1; i++)
    //         {
    //             // Her elemanı bir sonrakine bağla
    //             Debug.DrawLine(OptimalPath[i].transform.position, OptimalPath[i + 1].transform.position, Color.red);
    //         }
    //     }
    // }
    #endregion
}
