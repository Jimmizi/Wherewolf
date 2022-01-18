using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleGame : MonoBehaviour
{
    [SerializeField]
    GameManager Manager;
    [SerializeField]
    string EmulateWinActionName = "Jump";
    [SerializeField]
    string EmulateLoseActionName = "Fire1";
    [SerializeField]
    Text WinText;
    [SerializeField]
    Text GameOverText;

    void Awake()
    {
        if (WinText)
        {
            WinText.text = "Press the " + EmulateWinActionName + " button to emulate Winning.";
        }

        if (GameOverText)
        {
            GameOverText.text = "Press the " + EmulateLoseActionName + " button to emulate Losing.";
        }
    }

    void Update()
    {
        if (Manager == null)
        {
            return;
        }

        if (Input.GetButtonUp(EmulateWinActionName))
        {
            Manager.WinGame();
        }

        if (Input.GetButtonUp(EmulateLoseActionName))
        {
            Manager.LoseGame();
        }
    }
}
