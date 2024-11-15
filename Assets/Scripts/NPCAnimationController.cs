using UnityEngine;
using UnityEngine.Events;

public class NPCAnimationController : MonoBehaviour
{
    private const string AnimatorWalking = "isWalking";
    private const string AnimatorPicking = "isPicking";
    private const string AnimatorGolfCart = "isDroping";
    private const string AnimatorDying = "isDying";
    private Animator animator;

    public UnityEvent PickUpAnimationStart;
    public UnityEvent PickUpAnimationComplete;
    public UnityEvent DropAnimationStart;
    public UnityEvent DropAnimationComplete;
    public UnityEvent DieAnimationStart;
    public UnityEvent DieAnimationComplete;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetWalkingTrue()
    {
        animator.SetBool(AnimatorWalking, true);
    }
    public void SetWalkingFalse()
    {
        animator.SetBool(AnimatorWalking, false);
    }
    public void SetPickUpFalse()
    {
        animator.SetBool(AnimatorPicking, false);
        PickUpAnimationComplete?.Invoke();
    }
    public void SetPickUpTrue()
    {
        animator.SetBool(AnimatorPicking, true);
        PickUpAnimationStart?.Invoke();
    }
    public void SetDropTrue()
    {
        animator.SetBool(AnimatorGolfCart, true);
        DropAnimationStart?.Invoke();
    }
    public void SetDropFalse()
    {
        animator.SetBool(AnimatorGolfCart, false);
        DropAnimationComplete?.Invoke();
    }

    public void SetDieFalse()
    {
        //animator.SetBool(AnimatorDying, false);
        DieAnimationComplete?.Invoke();
    }
    public void SetDieTrue()
    {
        // animator.SetBool(AnimatorDying, true);
        animator.SetTrigger(AnimatorDying);
        DieAnimationStart?.Invoke();
    }

    /// <summary>
    /// Retrieves the duration of a specified animation clip by its name from the animator's runtime controller.
    /// It searches through all animation clips in the controller and returns the length of the matching clip.
    /// If the clip is not found, it logs a debug message and returns <c>float.MaxValue</c>.
    /// </summary>
    /// <param name="animationName">The name of the animation clip whose duration is to be retrieved.</param>
    /// <returns>The length of the specified animation clip in seconds, or <c>float.MaxValue</c> if the clip is not found.</returns>
    public float GetTimeAnimationClip(string animationName)
    {
        foreach (var item in animator.runtimeAnimatorController.animationClips)
        {
            if (item.name.Equals(animationName))
            {
                return item.length;
            }
        }
        Debug.Log($"{animationName} not found in {animator.runtimeAnimatorController.name} object.");
        return float.MaxValue;

    }

}
