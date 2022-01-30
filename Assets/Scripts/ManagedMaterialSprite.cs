using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ManagedMaterialSprite : MonoBehaviour {
    [Header("Sway")]
    public bool Sway = false;
    public float MaxSway = 0.1f;
    public float Rigidness = 1f;
    
    private SpriteRenderer _spriteRenderer;
    private Material _material;

    private bool _materialChanged;
    private MaterialPropertyBlock _materialPropertyBlock;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyMaterialPropertyBlock();
    }

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyMaterialPropertyBlock();
    }

    private void ApplyMaterialPropertyBlock() {
        /* If there's no swaying, do not apply any material property block. */
        if (_materialPropertyBlock == null && !Sway) return;
        
        if (_materialPropertyBlock == null) {
            _materialPropertyBlock = new MaterialPropertyBlock();
            if (_spriteRenderer.HasPropertyBlock()) {
                _spriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            }
        }

        _materialPropertyBlock.SetFloat("_Sway", (Sway) ? 1f : 0f);
        _materialPropertyBlock.SetFloat("_SwayMax", MaxSway);
        _materialPropertyBlock.SetFloat("_Rigidness", Rigidness);
        _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void SetMaterial(Material material) {
        _material = material;

        if (!_spriteRenderer) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        _spriteRenderer.sharedMaterial = material;
        _materialChanged = true;
        ApplyMaterialPropertyBlock();
    }

    private void Update() {
        if (_materialChanged) {
            ApplyMaterialPropertyBlock();
            _materialChanged = false;
        }
    }
}