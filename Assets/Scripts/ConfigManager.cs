using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    [SerializeField]
    public int CharacterHasOccupationChance = 50;

    [SerializeField]
    public float WanderRandomTimeMin = 20.0f;
    [SerializeField]
    public float WanderRandomTimeMax = 60.0f;

    [SerializeField]
    public float IdleRandomTimeMin = 15.0f;
    [SerializeField]
    public float IdleRandomTimeMax = 45.0f;

    [SerializeField]
    public float WorkRandomTimeMin = 30.0f;
    [SerializeField]
    public float WorkRandomTimeMax = 120.0f;

    // To give the player an extra hint, have the werewolf "sleep" the phase after killing a victim.
    [SerializeField]
    public bool WerewolfDisappearsAfterMurder = true;

    // Start is called before the first frame update
    void Awake()
    {
        Service.Config = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
