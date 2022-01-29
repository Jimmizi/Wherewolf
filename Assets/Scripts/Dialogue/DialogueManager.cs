using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {
    public DialogueRenderer DialogueRenderer;

    private void Awake() {
        Service.DialogueManager = this;
    }

    public void StartConversation(Character character) {
        if (DialogueRenderer != null) {
            DialogueRenderer.StartConversation(character);
        }
    }
}