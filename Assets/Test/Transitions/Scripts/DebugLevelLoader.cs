using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class DebugLevelLoader : MonoBehaviour
{
    [SerializeField]
    public Animator TransitionAnimator;

    [SerializeField]
    public Image TransitionImage;

    [SerializeField]
    public SceneField TargetScene;

    public float TransitionLength = 1.0f;
    public float TransitionSpeed = 1.0f;

    public float ShaderBlendedInCutoff;
    public float ShaderBlendedOutCutoff;

    public bool AutoBlendOut = false;

    private bool StartedPlayGameProcess = false;
    private bool TriedPlayingGameBeforeTransitionEnd = false;
    private bool GoToSceneWhenBlendedIn = false;

    public List<Texture2D> GradientTextures;
    public int GradientIndexToUse;

    [SerializeField]
    public Slider MusicVolumeSlider;

    // Is the transition graphic not present
    public bool IsBlendedOut()
    {
        return IsReadyForFadeInTransition();
    }

    // Is the transition graphic covering the screen
    public bool IsBlendedIn()
    {
        return IsReadyForFadeOutTransition();
    }

    // Start removing the transition graphic
    public void BlendOut()
    {
        if (IsReadyForFadeOutTransition())
        {
            TransitionAnimator.SetTrigger("StartLevel");
        }
    }

    // Start bringing in the transition graphic
    public void BlendIn()
    {
        if (IsReadyForFadeInTransition())
        {
            TransitionAnimator.SetTrigger("EndLevel");
        }
    }

    public void StartGame()
    {
        AkSoundEngine.PostEvent("Click", this.gameObject);

        if (StartedPlayGameProcess)
        {
            return;
        }

        StartedPlayGameProcess = true;

        if (IsReadyForFadeInTransition())
        {
            TransitionAnimator.SetTrigger("EndLevel");
        }
        else
        {
            TriedPlayingGameBeforeTransitionEnd = true;
        }

        GoToSceneWhenBlendedIn = true;
        
    }

    public void SetGoToSceneWhenBlendedIn()
    {
        
        GoToSceneWhenBlendedIn = true;
    }

    public void PlayClickSound()
    {
        AkSoundEngine.PostEvent("Click", this.gameObject);
    }

    public void TriggerApplicationQuit()
    {
        Application.Quit();
    }

    private static Texture2D sm_TextureToUse;

    private void Awake()
    {
        Service.Transition = this;
    }

    void Start()
    {
        TransitionImage?.gameObject.SetActive(true);

        if (!sm_TextureToUse && GradientTextures.Count > 0 && GradientIndexToUse < GradientTextures.Count)
        {
            int iIndex = GradientIndexToUse >= 0 ? GradientIndexToUse : UnityEngine.Random.Range(0, GradientTextures.Count);

            sm_TextureToUse = GradientTextures[iIndex];
        }

        if (TransitionImage)
        {
            if (sm_TextureToUse)
            {
                TransitionImage.material.SetTexture("TransitionTex", sm_TextureToUse);
            }

            TransitionImage.material.SetFloat("Cutoff", ShaderBlendedInCutoff);
            // StartCoroutine(BlendOutLevelShader());
        }

        if (AutoBlendOut)
        {
            BlendOut();
        }
    }

    void Update()
    {
        if(MusicVolumeSlider)
        {
            //Service.MusicVolume = MusicVolumeSlider.value;
        }

        if(GoToSceneWhenBlendedIn)
        {
            if (TriedPlayingGameBeforeTransitionEnd)
            {
                Debug.Log("Waiting for transition to be ready to blend in.");
                if (IsReadyForFadeInTransition())
                {
                    TransitionAnimator.SetTrigger("EndLevel");
                    TriedPlayingGameBeforeTransitionEnd = false;
                }
            }
            else
            {
                Debug.Log(string.Format("Waiting for transition to blend in. Target scene: {0}", TargetScene != null ? TargetScene.SceneName : "invalid"));
                if (IsReadyForFadeOutTransition() && TargetScene != null)
                {
                    SceneManager.LoadScene(TargetScene.SceneName);
                    GoToSceneWhenBlendedIn = false;
                }
            }
        }

        //if (TransitionAnimator)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space) && IsReadyForFadeOutTransition())
        //    {
        //        TransitionAnimator.SetTrigger("StartLevel");
        //    }

        //    if (Input.GetKeyDown(KeyCode.Space) && IsReadyForFadeInTransition())
        //    {
        //        StartCoroutine(LoadLevel());
        //    }
        //}
        //else if(TransitionImage)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space) && TransitionImage.material.GetFloat("Cutoff") >= ShaderBlendedOutCutoff)
        //    {
        //        StartCoroutine(BlendInLevelShader());
        //    }
        //}
    }

    IEnumerator LoadLevel()
    {
        TransitionAnimator.SetTrigger("EndLevel");
        yield return new WaitForSeconds(TransitionLength);

        //SceneManager.LoadScene(TargetScene.name);

        yield return null;
    }

    IEnumerator BlendOutLevelShader()
    {
        float fCutoff = ShaderBlendedInCutoff;
        TransitionImage.material.SetFloat("Cutoff", fCutoff);
        TransitionImage.rectTransform.rotation = Quaternion.Euler(0, 0, 180.0f);

        while (fCutoff < ShaderBlendedOutCutoff)
        {
            fCutoff += Time.deltaTime * TransitionSpeed;
            TransitionImage.material.SetFloat("Cutoff", fCutoff);
            yield return null;
        }

        

        yield return null;
    }

    IEnumerator BlendInLevelShader()
    {
        float fCutoff = ShaderBlendedOutCutoff;
        TransitionImage.material.SetFloat("Cutoff", fCutoff);
        TransitionImage.rectTransform.rotation = Quaternion.Euler(0, 0, 0.0f);

        while (fCutoff > ShaderBlendedInCutoff)
        {
            fCutoff -= Time.deltaTime * TransitionSpeed;
            TransitionImage.material.SetFloat("Cutoff", fCutoff);
            yield return null;
        }

       // SceneManager.LoadScene(TargetScene.name);

        yield return null;
    }

    private bool IsReadyForFadeInTransition()
    {
        var anim = TransitionAnimator.GetCurrentAnimatorStateInfo(0);
        return anim.IsName("EndWait") && anim.normalizedTime <= 0.0f;
    }

    private bool IsReadyForFadeOutTransition()
    {
        var anim = TransitionAnimator.GetCurrentAnimatorStateInfo(0);
        return anim.IsName("StartWait") && anim.normalizedTime <= 0.0f;

    }
}
