using UnityEngine.UI;

public class CharacterUIRenderer : BaseCharacterRenderer {
    public Image HeadSpriteRenderer;
    public Image HairSpriteRenderer;
    public Image EyesSpriteRenderer;
    public Image NoseSpriteRenderer;
    public Image MouthSpriteRenderer;
    public Image BodySpriteRenderer;
    
    protected override void Assemble() {
        if (CharacterGenerator.Instance == null) return;
        
        CharacterGenerator generator = CharacterGenerator.Instance;

        if (HeadSpriteRenderer != null && generator.HeadSprites != null) {
            HeadSpriteRenderer.sprite =
                generator.HeadSprites[_characterAttributes.HeadType % generator.HeadSprites.Count];
        }

        if (HairSpriteRenderer != null && generator.HairSprites != null) {
            HairSpriteRenderer.sprite =
                generator.HairSprites[_characterAttributes.HairType % generator.HairSprites.Count];
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
        
        if (BodySpriteRenderer != null && generator.BodySprites != null) {
            BodySpriteRenderer.sprite =
                generator.BodySprites[_characterAttributes.BodyType % generator.BodySprites.Count];
        }
    }

}