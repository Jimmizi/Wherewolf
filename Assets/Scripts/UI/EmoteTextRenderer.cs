using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteTextRenderer : MonoBehaviour {
    public GameObject EmotePrefab;
    public RectTransform TextArea;

    public int MaxVisibleCharacters {
        get => _maxVisibleCharacters;
        set {
            _maxVisibleCharacters = value;
            ApplyMaxVisibleCharacters();
        }
    }

    [SerializeField] private int _maxVisibleCharacters = Int32.MaxValue;

    private List<EmoteRenderer> _emoteRenderers = new List<EmoteRenderer>();
    private List<Emote> _emotes;

    private void Awake() {
        _emoteRenderers = new List<EmoteRenderer>();
    }

    private EmoteRenderer NewEmote() {
        GameObject gameObject = Instantiate(EmotePrefab, TextArea);
        EmoteRenderer emoteRenderer = gameObject.GetComponent<EmoteRenderer>();
        return emoteRenderer;
    }

    private void ApplyMaxVisibleCharacters() {
        for (int i = 0; i < _emoteRenderers.Count; i++) {
            _emoteRenderers[i].gameObject.SetActive(i < _maxVisibleCharacters);
        }
    }

    private void Render() {
        for (int i = 0; i < _emotes.Count; i++) {
            if (i >= _emoteRenderers.Count) {
                EmoteRenderer emoteRenderer = NewEmote();
                _emoteRenderers.Add(emoteRenderer);
                emoteRenderer.SetEmote(_emotes[i]);
                emoteRenderer.gameObject.SetActive(true);
            } else {
                _emoteRenderers[i].SetEmote(_emotes[i]);
                _emoteRenderers[i].gameObject.SetActive(true);
            }
        }

        ApplyMaxVisibleCharacters();
    }

    public void SortEmotes(float fWidth) {
        if (_emoteRenderers.Count > 1) {
            for (int i = 1; i < _emoteRenderers.Count; ++i) {
                _emoteRenderers[i].transform.localPosition += new Vector3(i * fWidth, 0.0f, 0.0f);
            }
        }
    }

    public void ClearEmotes(bool makeNew = false) {
        if (_emotes != null) {
            _emotes.Clear();
        }

        if (makeNew) {
            _emotes = new List<Emote>();
        }
    }

    public void AddEmoteToRender(Emote e) {
        _emotes.Add(e);
    }

    public void RenderWithoutEmotes() {
        Render();
    }

    public void Render(List<Emote> emotes) {
        _emotes = emotes;
        Render();
    }

    public void Clear() {
        if (_emoteRenderers != null) {
            foreach (EmoteRenderer emoteRenderer in _emoteRenderers) {
                emoteRenderer.gameObject.SetActive(false);
            }
        }
    }
}