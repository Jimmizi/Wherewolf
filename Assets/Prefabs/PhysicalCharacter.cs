using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhysicalCharacter : MonoBehaviour
{
    public Character AssociatedCharacter;

    private NavMeshAgent navMesh;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        Debug.Assert(navMesh);
    }

    // Update is called once per frame
    void Update()
    {
        if(navMesh)
        {
            //agent.destination = Destination.transform.position;
        }
    }
}
