using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreenManager : MonoBehaviour
{
    [SerializeField]
    public GameObject PanelToRotate;

    [SerializeField]
    public GameObject Sun;

    [SerializeField]
    public GameObject Moon;

    [SerializeField]
    public CanvasGroup AlphaGroup;

    float fRotationTarget = 0.0f;
    private bool isTransitioning = false;


    public bool Performing => isTransitioning;

    public void SetIsDay()
    {
        Debug.Log("Setting transition screen as being in daytime");
        Debug.Assert(PanelToRotate && Sun && Moon);

        PanelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        Sun.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        Moon.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void SetIsNight()
    {
        Debug.Log("Setting transition screen as being in nighttime");

        Debug.Assert(PanelToRotate && Sun && Moon);

        PanelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        Sun.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        Moon.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        
    }

    public void HidePanel()
    {
        //PanelToRotate?.SetActive(false);
        AlphaGroup.alpha = 1.0f;
        StartCoroutine(DoPanelAlpha(1.0f, 0.0f));
    }

    public void ShowPanel(float fDelay = 0.0f)
    {
        //PanelToRotate?.SetActive(true);
        AlphaGroup.alpha = 0.0f;
        StartCoroutine(DoPanelAlpha(1.0f, 1.0f, fDelay));
    }

    IEnumerator DoPanelAlpha(float fDuration, float fTarget, float fDelay = 0.0f)
    {
        if(fDelay > 0.0f)
        {
            yield return new WaitForSeconds(fDelay);
        }

        float fTimer = 0.0f;
        bool bDone = false;

        float fCurrentAlpha = AlphaGroup.alpha;
        bool bAlphaRise = fCurrentAlpha < fTarget;

        while (!bDone)
        {
            fTimer += Time.deltaTime;

            if(bAlphaRise)
            {
                if (fCurrentAlpha < fTarget)
                {
                    fCurrentAlpha += Time.deltaTime;
                }
                else
                {
                    bDone = true;
                }
            }
            else
            {
                if (fCurrentAlpha > fTarget)
                {
                    fCurrentAlpha -= Time.deltaTime;
                }
                else
                {
                    bDone = true;
                }
            }

            AlphaGroup.alpha = fCurrentAlpha;

            yield return new WaitForSeconds(fDuration * Time.deltaTime);
        }

        AlphaGroup.alpha = fTarget;

    }

    public void PerformTransition(float fDuration = 5.0f, float fDelay = 0.0f)
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            fRotationTarget = PanelToRotate.transform.eulerAngles.z == 0
                ? -180.0f
                : 0.0f;

            StartCoroutine(DoTransition(fDuration, fDelay));
        }
    }

    IEnumerator DoTransition(float fDuration = 4.0f, float fDelay = 0.0f)
    {
        if (fDelay > 0.0f)
        {
            yield return new WaitForSeconds(fDelay);
        }

        isTransitioning = true;
        float fTimer = 0.0f;
        bool bDone = false;

        float fCurrentRotation = PanelToRotate.transform.eulerAngles.z;

        while (!bDone)
        {
            fTimer += Time.deltaTime;

            if(fCurrentRotation > fRotationTarget)
            {
                fCurrentRotation -= (180.0f * Time.deltaTime);
            }
            else
            {
                bDone = true;
            }

            PanelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, fCurrentRotation);
            Sun.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fCurrentRotation);
            Moon.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fCurrentRotation);

            yield return new WaitForSeconds(fDuration * Time.deltaTime);
        }

        PanelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, fRotationTarget);
        Sun.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fRotationTarget);
        Moon.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fRotationTarget);

        isTransitioning = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Service.TransitionScreen = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
