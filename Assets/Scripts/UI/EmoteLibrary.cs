using System.Collections.Generic;
using UnityEngine;

public class EmoteLibrary : MonoBehaviour {
    public static EmoteLibrary Instance { get; private set; }
    
    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            Service.EmoteLibrary = this;
        }
    }

    public List<Sprite> EmoteSprites;

    public Sprite FindSprite(Emote.EmoteSubType emote) {
        if (EmoteSprites == null || EmoteSprites.Count == 0) return null;

        return EmoteSprites[((int) emote) % EmoteSprites.Count];
    }
}