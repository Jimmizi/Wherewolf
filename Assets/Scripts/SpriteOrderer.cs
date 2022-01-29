using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderer : MonoBehaviour
{
    [SerializeField]
    public GameObject ForPositionUseOverride;

    public SpriteRenderer Base = new SpriteRenderer();
    public List<SpriteRenderer> Children = new List<SpriteRenderer>();

    public float GetPositionZ()
    {
        if(ForPositionUseOverride)
        {
            return ForPositionUseOverride.transform.position.z;
        }

        return transform.position.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        Base = GetComponent<SpriteRenderer>();
        Children.AddRange(GetComponentsInChildren<SpriteRenderer>());

        if(Children.Contains(Base))
        {
            Children.Remove(Base);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
