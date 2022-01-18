using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleWin : MonoBehaviour
{
    [SerializeField]
    Text WinText;
    [SerializeField]
    string RestartInputName = "Submit";

    [SerializeField]
    GameManager Manager;
     
    void Start()
    {
    }

    void Update()
    {
        if (Manager == null)
        {
            return;
        }

        if (WinText != null)
        {
            WinText.text = "You won!\nWinning streak: " + Manager.WinStreak + ". Press " + RestartInputName + " to start again.";
        }
        
        if (Input.GetButtonUp(RestartInputName))
        {
            Manager.Restart();
        }
    }
}
