using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier {
    public void UpdateTime(float time);

    public void Modify(CharacterData characterData);

    public bool IsActive { get; }
}