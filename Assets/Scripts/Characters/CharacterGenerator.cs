using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour {
    public static CharacterGenerator Instance { get; private set; }

    public List<Sprite> BaseSprites;
    public List<Sprite> EarsSprites;
    public List<Sprite> EyesSprites;
    public List<Sprite> NoseSprites;
    public List<Sprite> MouthSprites;
    
    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public CharacterAttributes Generate() {
        CharacterAttributes attributes = new CharacterAttributes() {
            BaseType = (BaseSprites != null) ? Random.Range(0, BaseSprites.Count) : 0,
            EarsType = (EarsSprites != null) ? Random.Range(0, EarsSprites.Count) : 0,
            EyesType = (EyesSprites != null) ? Random.Range(0, EyesSprites.Count) : 0,
            NoseType = (NoseSprites != null) ? Random.Range(0, NoseSprites.Count) : 0,
            MouthType = (MouthSprites != null) ? Random.Range(0, MouthSprites.Count) : 0,
        };

        return attributes;
    }
}
