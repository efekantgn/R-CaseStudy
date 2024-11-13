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
    //TODO Pickup Animasyonu bitince 
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
