using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LocationManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> Locations;

    // Start is called before the first frame update
    void Awake()
    {
        Service.Location = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 GetRandomPositionInLocation(int iLocation)
    {
        Transform t = Locations[iLocation].transform;

        Vector2 vRange = new Vector2(t.localScale.x / 2, t.localScale.z / 2);

        float randomXPosition = t.position.x + UnityEngine.Random.Range(-vRange.x, vRange.x);
        float randomZPosition = t.position.z + UnityEngine.Random.Range(-vRange.y, vRange.y);

        return new Vector3(randomXPosition, 0.5f, randomZPosition);

    }

    public Vector3 GetRandomNavmeshPositionInLocation(int iLocation)
    {
        Debug.Assert(Emote.IsLocationValid(iLocation));
        Debug.Assert(Locations.Count == 9);

        for(int i = 0; i < 5; ++i)
        {
            Vector3 vPosition;
            if(GetNearestNavmeshLocation(GetRandomPositionInLocation(iLocation), out vPosition))
            {
                return vPosition;
            }
        }

        Debug.LogError("Failed to find a random position in " + iLocation);
        return Vector3.zero;
    }

    bool GetNearestNavmeshLocation(Vector3 vPosition, out Vector3 vPositionOut)
    {
        vPositionOut = Vector3.zero;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(vPosition, out hit, 5.0f, 1))
        {
            vPositionOut = hit.position;
            return true;
        }

        return false;
    }

    /*public Vector3 RandomNavmeshLocation(float radius) {
         Vector3 randomDirection = Random.insideUnitSphere * radius;
         randomDirection += transform.position;
         NavMeshHit hit;
         Vector3 finalPosition = Vector3.zero;
         if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
             finalPosition = hit.position;            
         }
         return finalPosition;
     }*/
}
