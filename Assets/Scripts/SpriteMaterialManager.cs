using System;
using UnityEngine;

public class SpriteMaterialManager : MonoBehaviour {
    public Material Material;

    private Material _material;
    private WerewolfGame.TOD _time;

    private void UpdateParameters() {
        if (_material == null) return;

        float shaderTime = (_time == WerewolfGame.TOD.Night) ? 1f : 0f;
        _material.SetFloat("_TimeOfDay", shaderTime);
    }

    private void Start() {
        _material = new Material(Material);
        ManagedMaterialSprite[] sprites = Resources.FindObjectsOfTypeAll<ManagedMaterialSprite>();
        foreach (ManagedMaterialSprite sprite in sprites) {
            sprite.SetMaterial(_material);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Service.Game.CurrentTimeOfDay != _time) {
            _time = Service.Game.CurrentTimeOfDay;
            UpdateParameters();
        }
    }
}