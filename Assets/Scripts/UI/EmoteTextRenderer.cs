using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteTextRenderer : MonoBehaviour {
    public GameObject EmotePrefab;
    public RectTransform TextArea;

    private List<EmoteRenderer> _emoteRenderers;

    private void Awake() {
        _emoteRenderers = new List<EmoteRenderer>();
    }

    private EmoteRenderer NewEmote() {
        GameObject gameObject = Instantiate(EmotePrefab, TextArea);
        EmoteRenderer emoteRenderer = gameObject.GetComponent<EmoteRenderer>();
        return emoteRenderer;
    }
    
    public void Render(List<Emote> emotes) {
        for (int i = 0; i < emotes.Count; i++) {
            if (i >= _emoteRenderers.Count) {
                EmoteRenderer emoteRenderer = NewEmote();
                _emoteRenderers.Add(emoteRenderer);
                emoteRenderer.SetEmote(emotes[i]);
            } else {
                _emoteRenderers[i].SetEmote(emotes[i]);
            }
        }
        
    }
}
