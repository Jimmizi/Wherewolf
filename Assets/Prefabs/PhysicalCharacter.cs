using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhysicalCharacter : MonoBehaviour
{
    private Character _associatedChar;
    public Character AssociatedCharacter
    {
        get => _associatedChar;
        set
        {
            _associatedChar = value;
            var links = GetComponentsInChildren<CharacterLink>();
            foreach(var l in links)
            {
                l.LinkedCharacter = value;
            }
        }
    }

    public GameObject CharacterRenderer;
    private float bounceOffset;
    
    private NavMeshAgent navMesh;

    private Vector3 _destination = new Vector3();
    public Vector3 CurrentDestination
    {
        get => _destination;
        set
        {
            _destination = value;

            if (navMesh.enabled)
            {
                navMesh.destination = value;
            }
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
        bounceOffset = Random.Range(0f, 1f);
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

        if (navMesh != null && navMesh.velocity.sqrMagnitude > 0f) {
            float bounceIntensity = 0.12f;
            float speed = 24f;
            float time = Quantize(Time.time + bounceOffset, 12);
                
            CharacterRenderer.transform.localPosition = new Vector3(
                Mathf.Cos(time * speed) * bounceIntensity * 0.1f,
                Mathf.Abs(Mathf.Sin(time * speed) * bounceIntensity) + 0.65f,
                0f);
            CharacterRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * speed) * 5f);
        } else {
            CharacterRenderer.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            CharacterRenderer.transform.localRotation = Quaternion.identity;
        }
    }
    
    public static float Quantize(float x, int steps) {
        return Mathf.Round(x * steps) / steps;
    }
}
