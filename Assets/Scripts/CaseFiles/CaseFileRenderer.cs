using System.Collections.Generic;
using UnityEngine;

public class CaseFileRenderer : MonoBehaviour {
    
    public GameObject PagePrefab;
    
    public GameObject CharacterAttributesPrefab;
    public GameObject StatementPrefab;

    private List<CaseFilePageRenderer> _pages;

    private static readonly List<string> _statements = new List<string> {
        "This is a first statement.",
        "This is a second statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
        "This is a third statement.",
    };

    private CaseFilePageRenderer NewPage() {
        if (PagePrefab == null) return null;

        GameObject gameObject = Instantiate(PagePrefab); //, transform);
        
        /* Apply random rotation */
        gameObject.transform.Rotate(Vector3.forward, Random.Range(-4f, 4f));
        
        CaseFilePageRenderer pageRenderer = gameObject.GetComponent<CaseFilePageRenderer>();
        _pages.Add(pageRenderer);
        
        return pageRenderer;
    }
    
    private void GenerateFile() {
        CaseFilePageRenderer currentPage = NewPage();
        
        /* Render attributes at the top of the first page. */
        GameObject attributesObject = Instantiate(CharacterAttributesPrefab);
        RectTransform attributesRectTransform = attributesObject.GetComponent<RectTransform>();
        
        currentPage.AddSection(attributesRectTransform);
        
        /* Render statements on succesive pages. */
        foreach (string statement in _statements) {
            GameObject statementObject = Instantiate(StatementPrefab);
            RectTransform statementRectTransform = statementObject.GetComponent<RectTransform>();
            CaseFileStatementRenderer statementRenderer = statementObject.GetComponent<CaseFileStatementRenderer>();

            if(Service.InfoManager != null)
            {
                if (statementRenderer != null)
                {
                    List<Emote> emotes = new List<Emote> {
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)],
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)],
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)],
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)],
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)],
                    Service.InfoManager.EmoteMapBySubType[(Emote.EmoteSubType) Random.Range(0, 50)]
                };

                    statementRenderer.Render(emotes);
                }
            }
            else if (statementRenderer != null) {
                List<Emote> emotes = new List<Emote> {
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50))
                };
                
                statementRenderer.Render(emotes);
            }
            
            if (!currentPage.TryAddSection(statementRectTransform)) {
                currentPage = NewPage();
                currentPage.AddSection(statementRectTransform);
            }
        }
        
        for (int i = _pages.Count - 1; i >= 0; i--) {
            _pages[i].transform.SetParent(transform, false);
        }
    }

    private void Start() {
        _pages = new List<CaseFilePageRenderer>();
        GenerateFile();
    }
}