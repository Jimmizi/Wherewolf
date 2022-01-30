using System;
using UnityEngine;

public class SpriteMaterialManager : MonoBehaviour {
    public Material Material;

    private Material _material;
    private WerewolfGame.TOD _time;

    private bool initMaterials = false;

    private void UpdateParameters() {
        if (_material == null) return;

        float shaderTime = (_time == WerewolfGame.TOD.Night) ? 1f : 0f;
        _material.SetFloat("_TimeOfDay", shaderTime);
    }

    private void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        if(!initMaterials)
        {
            if(Service.Population.CharacterCreationDone)
            {
                initMaterials = true;
                _material = new Material(Material);
                ManagedMaterialSprite[] sprites = Resources.FindObjectsOfTypeAll<ManagedMaterialSprite>();
                foreach (ManagedMaterialSprite sprite in sprites)
                {
                    sprite.SetMaterial(_material);
                }
            }
        }

        if (initMaterials && Service.Game.CurrentTimeOfDay != _time) 
        {
            if (Service.Transition.IsBlendedIn())
            {
                _time = Service.Game.CurrentTimeOfDay;
                UpdateParameters();
            }
        }
    }
}