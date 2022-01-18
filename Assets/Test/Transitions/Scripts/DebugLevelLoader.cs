using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class DebugLevelLoader : MonoBehaviour
{
    public Animator TransitionAnimator;
    public Image TransitionImage;

    public Object TargetScene;

    public float TransitionLength = 1.0f;
    public float TransitionSpeed = 1.0f;

    public float ShaderBlendedInCutoff;
    public float ShaderBlendedOutCutoff;

    public List<Texture2D> GradientTextures;
    public int GradientIndexToUse;

    private static Texture2D sm_TextureToUse;

    void Start()
    {
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
            StartCoroutine(BlendOutLevelShader());
        }
    }

    void Update()
    {

        if (TransitionAnimator)
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsReadyForFadeOutTransition())
            {
                TransitionAnimator.SetTrigger("StartLevel");
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsReadyForFadeInTransition())
            {
                StartCoroutine(LoadLevel());
            }
        }
        else if(TransitionImage)
        {
            if (Input.GetKeyDown(KeyCode.Space) && TransitionImage.material.GetFloat("Cutoff") >= ShaderBlendedOutCutoff)
            {
                StartCoroutine(BlendInLevelShader());
            }
        }
    }

    IEnumerator LoadLevel()
    {
        TransitionAnimator.SetTrigger("EndLevel");
        yield return new WaitForSeconds(TransitionLength);

        SceneManager.LoadScene(TargetScene.name);

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

        SceneManager.LoadScene(TargetScene.name);

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
