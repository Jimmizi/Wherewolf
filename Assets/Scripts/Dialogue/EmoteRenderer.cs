using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EmoteRenderer : MonoBehaviour {
    public Image Image;

    public Emote Emote {
        get; private set;
    }

    private void SetSprite(Emote.EmoteSubType type) {
        Image.sprite = EmoteLibrary.Instance.FindSprite(type);
    }
    
    public void Start() {
        SetSprite((Emote.EmoteSubType) Random.Range(0, 10));
    }

    public void SetEmote(Emote emote) {
        Emote = emote;
        SetSprite(emote.SubType);
    }
}
