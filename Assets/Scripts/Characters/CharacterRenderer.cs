using UnityEngine;

public class CharacterRenderer : BaseCharacterRenderer {
    public SpriteRenderer HeadSpriteRenderer;
    public SpriteRenderer HairSpriteRenderer;
    public SpriteRenderer EyesSpriteRenderer;
    public SpriteRenderer NoseSpriteRenderer;
    public SpriteRenderer MouthSpriteRenderer;
    public SpriteRenderer BodySpriteRenderer;
    public SpriteRenderer LegsSpriteRenderer;
    
    protected override void Assemble() {
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
        
        if (LegsSpriteRenderer != null && generator.LegsSprites != null) {
            LegsSpriteRenderer.sprite =
                generator.LegsSprites[_characterAttributes.LegsType % generator.LegsSprites.Count];
        }
    }
    
    
    public void Start() {
        _characterAttributes = CharacterGenerator.Instance.Generate();
        Assemble();
    }
    
}