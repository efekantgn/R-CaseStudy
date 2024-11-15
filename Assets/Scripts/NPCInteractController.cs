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

    /// <summary>
    /// Checks for nearby interactable objects within a specified radius and performs actions based on the type of object found.
    /// The method looks for GolfBall and GolfCart objects within a defined radius around the current object.
    /// If a GolfBall is found, it triggers a pick-up animation and stores the ball's score. 
    /// If a GolfCart is found, it triggers a drop animation.
    /// </summary>
    public void IsStopOnInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out GolfBall golfBall))
            {
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

    /// <summary>
    /// This method is called when the player interacts with a GolfCart.
    /// If the player has played drop animation,
    /// it raises an event on the GolfCart to update the cart's score with the collected ball's score.
    /// </summary>
    public void InteractedWithGolfCart()
    {
        if (collectedBallScore <= 0) return;

        movement.GolfCart.RaiseEventOnCartCollect(collectedBallScore);
    }

    /// <summary>
    /// This method is called when the player interacts with a GolfBall.
    /// It searches for the closest GolfBall in the scene and invokes its pick-up event if found.
    /// </summary>
    public void InteractedWithGolfBall()
    {
        GameObject closestBall = SearchClosestGameObject();

        if (closestBall.TryGetComponent(out GolfBall ball))
        {
            ball.OnBallPickUp?.Invoke();
        }

    }
    /// <summary>
    /// Searches for the closest game object within a specified radius in front of the object.
    /// It casts a sphere along the object's forward direction and checks for all hits within the interaction radius.
    /// The closest object is then determined by calculating the distances from the origin to each hit object.
    /// </summary>
    /// <returns>The closest game object within the interaction radius, or null if no objects are found.</returns>
    private GameObject SearchClosestGameObject()
    {
        Vector3 origin = transform.position;
        Ray ray = new Ray(origin, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, interactionRadius);

        if (hits.Length == 0) return null;

        return FindNearestObject(hits);
    }

    /// <summary>
    /// Determines the closest game object from an array of RaycastHit objects by comparing distances.
    /// It iterates through all hits and returns the one closest to the origin of the raycast.
    /// </summary>
    /// <param name="raycastHits">The array of RaycastHit objects that represent the detected objects.</param>
    /// <returns>The closest game object based on the raycast hits.</returns>
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

        return closest;
    }
}