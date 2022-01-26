using UnityEngine;

public class CharacterRenderer : MonoBehaviour {
    public SpriteRenderer BaseSpriteRenderer;
    public SpriteRenderer EarsSpriteRenderer;
    public SpriteRenderer EyesSpriteRenderer;
    public SpriteRenderer NoseSpriteRenderer;
    public SpriteRenderer MouthSpriteRenderer;

    private CharacterAttributes _characterAttributes;

    private void Assemble() {
        CharacterGenerator generator = CharacterGenerator.Instance;

        if (BaseSpriteRenderer != null && generator.BaseSprites != null) {
            BaseSpriteRenderer.sprite =
                generator.BaseSprites[_characterAttributes.BaseType % generator.BaseSprites.Count];
        }

        if (EarsSpriteRenderer != null && generator.EarsSprites != null) {
            EarsSpriteRenderer.sprite =
                generator.EarsSprites[_characterAttributes.EarsType % generator.EarsSprites.Count];
        }

        if (EyesSpriteRenderer != null && generator.EyesSprites != null) {
            EyesSpriteRenderer.sprite =
                generator.EyesSprites[_characterAttributes.EyesType % generator.EyesSprites.Count];
        }

        if (NoseSpriteRenderer != null && generator.NoseSprites != null) {
            NoseSpriteRenderer.sprite =
                generator.NoseSprites[_characterAttributes.NoseType % generator.NoseSprites.Count];
        }

        if (MouthSpriteRenderer != null && generator.MouthSprites != null) {
            MouthSpriteRenderer.sprite =
                generator.MouthSprites[_characterAttributes.MouthType % generator.MouthSprites.Count];
        }
    }

    public void SetAttributes(CharacterAttributes attributes) {
        _characterAttributes = attributes;
        Assemble();
    }

    public void Start() {
        _characterAttributes = CharacterGenerator.Instance.Generate();
        Assemble();
    }
    
}