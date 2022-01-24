using UnityEngine;

public class WaveModifier : BaseModifier {

    public float Amplitude = 10f;

    public override void Modify(CharacterData characterData) {
        float y = Mathf.Sin(_progress * Mathf.PI + characterData.CharacterIndex) * Amplitude;
        characterData.Translate(new Vector3(0f, y, 0f));
    }
}