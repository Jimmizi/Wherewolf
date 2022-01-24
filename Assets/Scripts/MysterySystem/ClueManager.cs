using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Awake()
    {
        Service.Clue = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
