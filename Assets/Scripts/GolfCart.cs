using UnityEngine;
using UnityEngine.Events;

public class GolfCart : MonoBehaviour
{
    public UnityEvent<int> OnCartCollect;

    public void RaiseEventOnCartCollect(int score)
    {
        OnCartCollect?.Invoke(score);
    }
}