using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> TutorialPageList = new List<GameObject>();

    public int CurrentPage = 0;

    [SerializeField]
    public Button GoPreviousButton;

    [SerializeField]
    public Button GoNextButton;

    [SerializeField]
    public Text GoNextButtonText;

    public void GoBackAPage()
    {
        if(CurrentPage > 0)
        {
            TutorialPageList[CurrentPage--].SetActive(false);
            TutorialPageList[CurrentPage].SetActive(true);
        }

        UpdateButtonStatus();
    }

    public void GoForwardsAPage()
    {
        if(CurrentPage == TutorialPageList.Count - 1)
        {
            CloseTutorial();
        }
        else if(CurrentPage < TutorialPageList.Count - 1)
        {
            TutorialPageList[CurrentPage++].SetActive(false);
            TutorialPageList[CurrentPage].SetActive(true);
        }

        UpdateButtonStatus();
    }

    void UpdateButtonStatus()
    {
        GoPreviousButton.gameObject.SetActive(CurrentPage != 0);
        GoNextButtonText.text =
            CurrentPage == TutorialPageList.Count - 1
            ? "Finish"
            : "Next";
    }

    private void OnEnable()
    {
        if (TutorialPageList.Count > 0)
        {
            foreach (var go in TutorialPageList)
            {
                go.SetActive(false);
            }

            CurrentPage = 0;
            GoPreviousButton.gameObject.SetActive(false);
            GoNextButtonText.text = "Next";

            TutorialPageList[CurrentPage].SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        gameObject.SetActive(false);
        CurrentPage = 0;
        GoPreviousButton.gameObject.SetActive(false);
        GoNextButtonText.text = "Next";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
