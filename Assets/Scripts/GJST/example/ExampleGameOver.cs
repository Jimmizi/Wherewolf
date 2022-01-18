using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleGameOver : MonoBehaviour
{
    [SerializeField]
    Text GameOverText;

    [SerializeField]
    GameManager Manager;
    [SerializeField]
    string RestartInputName = "Submit";
    // Start is called before the first frame update
    void Start()
    {
        if (GameOverText != null && Manager != null)
        {
            GameOverText.text = "You lost :(\nPress " + RestartInputName + " to start again.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager != null && Input.GetButtonUp(RestartInputName))
        {
            Manager.Restart();
        }
    }
}
