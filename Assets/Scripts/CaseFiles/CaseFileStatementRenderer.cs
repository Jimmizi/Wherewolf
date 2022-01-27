using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseFileStatementRenderer : MonoBehaviour {

    public EmoteTextRenderer EmoteTextRenderer;

    public void Render(List<Emote> emotes) {
        if (EmoteTextRenderer != null) {
            EmoteTextRenderer.Render(emotes);
        }        
    }
}