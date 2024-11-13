using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteractController : MonoBehaviour
{
    private float interactionRadius = 1f;
    private NPCAnimationController animationController;
    private NPCMovement movement;
    private int collectedBallScore;
    private void Awake()
    {
        animationController = GetComponent<NPCAnimationController>();
        movement = GetComponent<NPCMovement>();
    }
    public void IsStopOnInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out GolfBall golfBall))
            {
                //NPCanim.Setbool pickup;
                animationController.SetPickUpTrue();
                collectedBallScore = golfBall.BallScore;
                return;
            }
            else if (item.TryGetComponent(out GolfCart golfCart))
            {

                animationController.SetDropTrue();
                return;
            }
        }
    }
    public void InteractedWithGolfCart()
    {
        if (collectedBallScore <= 0) return;

        movement.GolfCart.RaiseEventOnCartCollect(collectedBallScore);
    }


    public void InteractedWithGolfBall()
    {
        GameObject closestBall = SearchClosestGameObject();

        if (closestBall.TryGetComponent(out GolfBall ball))
        {
            ball.OnBallPickUp?.Invoke();
        }

    }

    private GameObject SearchClosestGameObject()
    {
        Vector3 origin = transform.position;
        Ray ray = new Ray(origin, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, interactionRadius);

        if (hits.Length == 0) return null;

        return FindNearestObject(hits);
    }

    private GameObject FindNearestObject(RaycastHit[] raycastHits)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity; // Sonsuz ile başlat

        foreach (RaycastHit hit in raycastHits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = hit.collider.gameObject;
            }
        }

        float nearestDistance = minDistance;
        return closest;
    }
}