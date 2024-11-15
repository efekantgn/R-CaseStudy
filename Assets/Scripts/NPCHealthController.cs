using System;
using UnityEngine;
using UnityEngine.Events;
using Vitals;
/// <summary>
/// A controller class that manages the health of NPCs, integrating with the Vitals asset.
/// It tracks the NPC's health value and triggers an event when health reaches zero.
/// The class provides methods to retrieve the NPC's health stats and the rate of health drain.
/// </summary>
/// <remarks>
/// This class uses the Vitals asset to handle health management, such as regeneration, drain rate, and maximum health.
/// </remarks>
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