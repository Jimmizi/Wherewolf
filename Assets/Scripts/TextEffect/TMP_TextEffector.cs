using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("UI/TMP Text Effector")]
public class TMP_TextEffector : MonoBehaviour {
    [SerializeField] private TMP_Text _TMPText;

    private TMP_Text TMPText {
        get {
            if (_TMPText == null) {
                _TMPText = GetComponent<TMP_Text>();
                if (_TMPText == null)
                    _TMPText = GetComponentInChildren<TMP_Text>();
            }

            return _TMPText;
        }
    }

    private RectTransform rectTransform;

    public RectTransform RectTransform {
        get {
            if (rectTransform == null)
                rectTransform = (RectTransform) transform;
            return rectTransform;
        }
    }

    public string Text {
        get { return TMPText.text; }
        set {
            TMPText.text = value;
            SetDirty();
            UpdateIfDirty();
        }
    }

    [SerializeField] [Range(0.0f, 1.0f)] private float progress;

    public float Progress {
        get { return progress; }
    }

    [SerializeField] private bool playWhenReady = true;

    [SerializeField] private bool loop;

    [SerializeField] private bool playForever;

    [SerializeField] private bool animationControlled;

    private bool isPlaying;

    public bool IsPlaying {
        get { return isPlaying; }
    }

    private CharacterData[] charactersData;
    private List<IModifier> modifiers;
    private TMP_MeshInfo[] cachedMeshInfo;
    private TMP_TextInfo textInfo;
    private string cachedText = string.Empty;

    private float internalTime;
    private bool isDirty = true;
    private bool dispatchedAfterReadyMethod;
    private bool updateGeometry;
    private bool updateVertexData;
    private bool forceUpdate;

    private void OnValidate() {
        cachedText = string.Empty;
        SetDirty();

        if (_TMPText == null) {
            _TMPText = GetComponent<TMP_Text>();
            if (_TMPText == null)
                _TMPText = GetComponentInChildren<TMP_Text>();
        }
    }

    private void Awake() {
        if (!animationControlled && Application.isPlaying)
            SetProgress(0);
    }

    private void Start() {
        ColorModifier cm = new ColorModifier();
        cm.Colors = new[] {
            Color.blue,
            Color.magenta,
            Color.green,
            Color.yellow,
            Color.red
        };
        cm.StartingTime = 0f;
        cm.TotalAnimationTime = 10f;
        cm.Loop = true;

        modifiers = new List<IModifier>();
        modifiers.Add(cm);
        playForever = true;
        
        WaveModifier wm = new WaveModifier();
        wm.StartingTime = 0f;
        wm.TotalAnimationTime = 1f;
        wm.Loop = true;
        modifiers.Add(wm);

    }

    private void OnDisable() {
        forceUpdate = true;
    }

    public void Update() {
        if (!IsAllComponentsReady())
            return;

        UpdateIfDirty();

        if (!dispatchedAfterReadyMethod) {
            AfterIsReady();
            dispatchedAfterReadyMethod = true;
        }

        CheckProgress();
        UpdateTime();
        
        if (IsPlaying || animationControlled || forceUpdate) {
            ApplyModifiers();
        }
    }

    public void Restart() {
        internalTime = 0;
    }

    public void Play() {
        Play(true);
    }

    public void Play(bool fromBeginning = true) {
        if (!IsAllComponentsReady()) {
            playWhenReady = true;
            return;
        }

        if (fromBeginning)
            Restart();

        isPlaying = true;
    }

    public void Complete() {
        if (IsPlaying)
            progress = 1.0f;
    }

    public void Stop() {
        isPlaying = false;
    }

    public void SetProgress(float targetProgress) {
        progress = targetProgress;
        internalTime = progress; // * realTotalAnimationTime;
        UpdateTime();
        ApplyModifiers();
        _TMPText.havePropertiesChanged = true;
    }

    public void SetPlayForever(bool shouldPlayForever) {
        playForever = shouldPlayForever;
    }

    private void AfterIsReady() {
        if (!Application.isPlaying)
            return;

        if (playWhenReady) {
            Play();
        } else {
            SetProgress(progress);
        }
    }

    private bool IsAllComponentsReady() {
        if (TMPText == null)
            return false;

        if (TMPText.textInfo == null)
            return false;

        if (TMPText.mesh == null)
            return false;

        if (TMPText.textInfo.meshInfo == null)
            return false;
        
        return true;
    }


    private void ApplyModifiers() {
        if (charactersData == null)
            return;

        _TMPText.ForceMeshUpdate(true);
        for (int i = 0; i < charactersData.Length; i++)
            ModifyCharacter(i, cachedMeshInfo);

        if (updateGeometry) {
            for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                TMPText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        if (updateVertexData)
            TMPText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ModifyCharacter(int info, TMP_MeshInfo[] meshInfo) {
        if (modifiers == null) return;
        
        CharacterData data = charactersData[info];
        data.Reset();
        
        foreach (IModifier modifier in modifiers) {
            modifier.Modify(data);
        }

        int materialIndex = 0;
        int vertexIndex = textInfo.characterInfo[info].vertexIndex;

        Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
        Vector3[] sourceVertices = meshInfo[materialIndex].vertices;
        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3 baselineOffset = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
        
        newVertexColors[vertexIndex + 0] = data.Color;
        newVertexColors[vertexIndex + 1] = data.Color;
        newVertexColors[vertexIndex + 2] = data.Color;
        newVertexColors[vertexIndex + 3] = data.Color;
        
        destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - baselineOffset;
        destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - baselineOffset;
        destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - baselineOffset;
        destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - baselineOffset;

        Matrix4x4 matrix = Matrix4x4.TRS(data.Translation, data.Rotation, data.Scaling);

        destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
        destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
        destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
        destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

        destinationVertices[vertexIndex + 0] += baselineOffset;
        destinationVertices[vertexIndex + 1] += baselineOffset;
        destinationVertices[vertexIndex + 2] += baselineOffset;
        destinationVertices[vertexIndex + 3] += baselineOffset;
    }

    private void CheckProgress() {
        if (IsPlaying) {
            internalTime += Time.deltaTime;
            if (playForever) return;

            if (loop) {
                internalTime = 0;
            } else {
                // TODO: On animation finish
                progress = 1.0f;
                Stop();
                OnAnimationCompleted();
            }
        }
    }

    private void OnAnimationCompleted() {
    }

    private void UpdateTime() {
        if (modifiers != null) {
            foreach (IModifier modifier in modifiers) {
                modifier.UpdateTime(internalTime);
            }
        }
    }

    private void UpdateIfDirty() {
        if (!isDirty)
            return;

        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
            return;

        if (modifiers != null) {
            foreach (IModifier modifier in modifiers) {
                //if (!updateGeometry && vertexModifier.ModifyGeometry)
                // TODO: Not every modifier needs a geometry update.
                updateGeometry = true;
                //if (!updateVertexData && vertexModifier.ModifyVertex)
                updateVertexData = true;
            }
        }

        if (string.IsNullOrEmpty(cachedText) || !cachedText.Equals(TMPText.text)) {
            TMPText.ForceMeshUpdate();
            textInfo = TMPText.textInfo;
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            List<CharacterData> newCharacterDataList = new List<CharacterData>();
            int indexCount = 0;
            for (int i = 0; i < textInfo.characterCount; i++) {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                CharacterData characterData = new CharacterData(i) {
                    Color = TMPText.color
                };
                
                newCharacterDataList.Add(characterData);
                indexCount += 1;
            }

            charactersData = newCharacterDataList.ToArray();
            cachedText = TMPText.text;
        }

        isDirty = false;
    }

    public void SetDirty() {
        isDirty = true;
    }
}