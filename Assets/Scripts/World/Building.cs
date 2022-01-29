using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Emote.EmoteSubType BuildingType;
    public Vector3 UseBuildingPosition;

    public Character Owner = null;
    public bool IsHome = false;

    public int Location;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gameObject.transform.position + UseBuildingPosition, 0.2f);
    }
#endif
}
