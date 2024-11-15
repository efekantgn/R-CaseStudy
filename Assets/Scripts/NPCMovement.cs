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
    public List<Transform> OptimalPath = new();


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

    public void StartMoving(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        isMoving = true;
        OnStartMoving?.Invoke();
    }

    public void StopMoving()
    {
        navMeshAgent.ResetPath();
        isMoving = false;
        OnStopMoving?.Invoke();
    }

    /// <summary>
    /// Calculates the rate of health loss per unit distance traveled based on the movement speed.
    /// The health loss is determined by the drain rate from the health controller and the movement speed.
    /// </summary>
    /// <param name="movementSpeed">The movement speed of the character or entity.</param>
    /// <returns>The amount of health lost per unit distance traveled.</returns>
    public float GetHealthLossPerUnitDistance(float movementSpeed)
    {
        return healthController.GetDrainRate() / movementSpeed;
    }

    /// <summary>
    /// Calculates the total path distance between two points using the NavMesh system.
    /// The function calculates the path from the start point to the end point and sums the distances
    /// between each corner of the calculated path to determine the total distance.
    /// </summary>
    /// <param name="startPoint">The starting point of the path.</param>
    /// <param name="endPoint">The destination point of the path.</param>
    /// <returns>The total distance of the calculated path, or <c>float.MinValue</c> if no valid path is found.</returns>
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

    /// <summary>
    /// Calculates the optimal path for picking up and delivering golf balls using a greedy algorithm.
    /// The function evaluates each remaining golf ball based on the distance and health cost to the player,
    /// selecting the ball with the best score (highest value-to-distance ratio). It continues until all balls are collected
    /// or the player cannot afford the health cost to pick up and deliver a ball.
    /// </summary>
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
                float healthLossOnDistanceToBall = healthLossPerUnitDistance * distanceToBall;

                float distancePlayerToCart = GetPathDistance(golfBall.transform.position, GolfCart.transform.position);
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
}
