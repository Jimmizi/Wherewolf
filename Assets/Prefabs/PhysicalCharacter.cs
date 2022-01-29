using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhysicalCharacter : MonoBehaviour
{
    public Character AssociatedCharacter;

    private NavMeshAgent navMesh;

    private Vector3 _destination = new Vector3();
    public Vector3 CurrentDestination
    {
        get => _destination;
        set
        {
            _destination = value;
            navMesh.destination = value;
        }
    }

    private bool bWait = false;
    private float fTimer = 0.0f;

    public void ClearDestination()
    {
        CurrentDestination = transform.position;
        navMesh.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        Debug.Assert(navMesh);
    }

    public void SetNotWait()
    {
        bWait = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(AssociatedCharacter.IsBeingTalkedTo)
        {
            return;
        }

        if (!bWait)
        {
            // when a wander gets to the position, wait for a little bit
            if(AssociatedCharacter.CurrentTask != null
                && AssociatedCharacter.CurrentTask.Type == Task.TaskType.WanderArea
                && _destination != Vector3.zero)
            {
                if (Vector3.Distance(transform.position, _destination) < 1.5f)
                {
                    bWait = true;
                    fTimer = 0.0f;
                }
            }
        }
        else
        {
            // Only get in here if wandering
            if (AssociatedCharacter.CurrentTask == null
                || AssociatedCharacter.CurrentTask.Type != Task.TaskType.WanderArea)
            {
                bWait = false;
            }

            fTimer += Time.deltaTime;
            if(fTimer > 7.5f)
            {
                if(AssociatedCharacter.CurrentTask != null)
                {
                    int iLoc = AssociatedCharacter.CurrentTask.Location;
                    CurrentDestination = Service.Location.GetRandomNavmeshPositionInLocation(iLoc);
                    bWait = false;
                }
            }
        }
    }
}
