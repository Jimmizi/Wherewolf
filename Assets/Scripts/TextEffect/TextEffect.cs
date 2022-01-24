using UnityEngine;

public abstract class TextEffect {
    public Vector3 _startPos;
    public GameObject gameObject;

    public TextEffect(GameObject gameObject) {
        this.gameObject = gameObject;
        this._startPos = gameObject.transform.localPosition;
    }

    public abstract void Update();
}