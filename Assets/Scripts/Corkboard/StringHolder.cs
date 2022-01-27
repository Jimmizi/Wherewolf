using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringHolder : MonoBehaviour
{
    private static StringHolder _instance;
    public static StringHolder instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<StringHolder>();
            }
            return _instance;
        }
    }
    
    void Start()
    {
        if (!_instance)
        {
            _instance = this;
        }
    }
}
