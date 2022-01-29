using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterEmoteRenderer : BaseCharacterRenderer {
    public Image BaseSpriteRenderer;
    public Image EarsSpriteRenderer;
    public Image EyesSpriteRenderer;
    public Image NoseSpriteRenderer;
    public Image MouthSpriteRenderer;
    
    protected override void Assemble() {
        if (CharacterGenerator.Instance == null) return;
        
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
    
    public void Start() {
        if (CharacterGenerator.Instance != null) {
            _characterAttributes = CharacterGenerator.Instance.Generate();
            Assemble();
        }
    }
}