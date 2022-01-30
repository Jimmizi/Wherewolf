using System.Collections.Generic;
using UnityEngine;

public class EmoteLibrary : MonoBehaviour {
    protected void Awake() {
        Service.EmoteLibrary = this;
    }

    public List<Sprite> EmoteSprites;

    public Sprite FindSprite(Emote.EmoteSubType emote) {
        if (EmoteSprites == null || EmoteSprites.Count == 0) return null;

        return EmoteSprites[((int) emote) % EmoteSprites.Count];
    }
}