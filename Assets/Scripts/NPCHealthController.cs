using System;
using UnityEngine;
using UnityEngine.Events;
using Vitals;

[RequireComponent(typeof(Health))]
public class NPCHealthController : MonoBehaviour
{
    public UnityEvent OnHealthEmpty;
    private Health health;
    private void Awake()
    {
        health = GetComponent<Health>();
    }
    private void OnEnable()
    {
        health.OnValueEmpty += () => OnHealthEmpty?.Invoke();
    }
    public float GetDrainRate()
    {
        return health.Regeneration.DrainRate;
    }
    public float GetMaxHealth()
    {
        return health.MaxValue;
    }
    public float GetCurrentHealth()
    {
        return health.Value;
    }

}