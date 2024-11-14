using System.Collections.Generic;
using UnityEngine;

public class GolfBallSpawner : MonoBehaviour
{
    [Header("Tier Settings")]
    public GolfBall tier1Prefab;
    public GolfBall tier2Prefab;
    public GolfBall tier3Prefab;

    [Header("Spawn Counts")]
    public int tier1Count = 10;
    public int tier2Count = 8;
    public int tier3Count = 5;

    [Header("Spawn Distances")]
    public float tier1MinDistance = 10f;
    public float tier1MaxDistance = 15f;

    public float tier2MinDistance = 15f;
    public float tier2MaxDistance = 20f;

    public float tier3MinDistance = 20f;
    public float tier3MaxDistance = 25f;

    [Header("Spawn Settings")]
    public Transform spawnOrigin;
    public float spawnAngle = 90f;
    public Terrain terrain;

    public List<GolfBall> SpawnedObjects = new List<GolfBall>();

    void Start()
    {
        SpawnGolfBalls(tier1Prefab, tier1Count, tier1MinDistance, tier1MaxDistance);
        SpawnGolfBalls(tier2Prefab, tier2Count, tier2MinDistance, tier2MaxDistance);
        SpawnGolfBalls(tier3Prefab, tier3Count, tier3MinDistance, tier3MaxDistance);
    }

    void SpawnGolfBalls(GolfBall prefab, int count, float minDistance, float maxDistance)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInArc(minDistance, maxDistance);
            spawnPosition.y = spawnOrigin.position.y;
            if (terrain != null)
            {
                float terrainHeight = terrain.SampleHeight(spawnPosition);
                spawnPosition.y = terrainHeight + .1f;
            }
            GolfBall gb = Instantiate(prefab, spawnPosition, Quaternion.identity, spawnOrigin);
            gb.Spawner = this;
            SpawnedObjects.Add(gb);
        }
    }

    Vector3 GetRandomPositionInArc(float minDistance, float maxDistance)
    {
        float angle = Random.Range(-spawnAngle / 2f, spawnAngle / 2f);
        float angleRad = angle * Mathf.Deg2Rad;

        float radius = Random.Range(minDistance, maxDistance);
        Vector3 localOffset = new Vector3(Mathf.Sin(angleRad) * radius, 0, Mathf.Cos(angleRad) * radius);

        return spawnOrigin.position + spawnOrigin.TransformDirection(localOffset);
    }

    #region VisualizeOnEditor
    void OnDrawGizmos()
    {
        if (spawnOrigin == null)
            return;

        Gizmos.color = Color.green;

        // Min ve Max yayları çiz
        DrawArc(spawnOrigin.position, tier1MinDistance, tier1MaxDistance, spawnAngle);
        DrawArc(spawnOrigin.position, tier2MinDistance, tier2MaxDistance, spawnAngle);
        DrawArc(spawnOrigin.position, tier3MinDistance, tier3MaxDistance, spawnAngle);
    }

    void DrawArc(Vector3 origin, float minRadius, float maxRadius, float angle)
    {
        int segments = 50;
        float angleStep = angle / segments;

        // SpawnOrigin yönüne göre hesapla
        Quaternion rotation = spawnOrigin.rotation;

        Vector3 lastMinPoint = origin + rotation * PolarToCartesian(-angle / 2f, minRadius);
        Vector3 lastMaxPoint = origin + rotation * PolarToCartesian(-angle / 2f, maxRadius);

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -angle / 2f + i * angleStep;

            Vector3 minPoint = origin + rotation * PolarToCartesian(currentAngle, minRadius);
            Vector3 maxPoint = origin + rotation * PolarToCartesian(currentAngle, maxRadius);

            // Yay segmentlerini çiz
            Gizmos.DrawLine(lastMinPoint, minPoint);
            Gizmos.DrawLine(lastMaxPoint, maxPoint);

            // Min ve Max yayları birleştir
            if (i == 1 || i == segments)
                Gizmos.DrawLine(minPoint, maxPoint);

            lastMinPoint = minPoint;
            lastMaxPoint = maxPoint;
        }
    }

    Vector3 PolarToCartesian(float angle, float radius)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad) * radius, 0, Mathf.Cos(angleRad) * radius);
    }
    #endregion

}