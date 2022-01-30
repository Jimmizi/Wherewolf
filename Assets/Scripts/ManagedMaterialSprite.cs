using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ManagedMaterialSprite : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMaterial(Material material) {
        _material = material;
        _spriteRenderer.sharedMaterial = material;
    }
}