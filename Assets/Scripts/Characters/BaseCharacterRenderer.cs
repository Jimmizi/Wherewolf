using UnityEngine;

public abstract class BaseCharacterRenderer : MonoBehaviour {
    
    protected CharacterAttributes _characterAttributes;
    
    protected abstract void Assemble(); 

    public virtual void SetAttributes(CharacterAttributes attributes) {
        _characterAttributes = attributes;
        Assemble();
    }

}