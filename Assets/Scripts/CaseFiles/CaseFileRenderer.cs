using System.Collections.Generic;
using UnityEngine;

public class CaseFileRenderer : MonoBehaviour {
    
    public GameObject PagePrefab;
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
    };

    private CaseFilePageRenderer NewPage() {
        if (PagePrefab == null) return null;

        GameObject gameObject = Instantiate(PagePrefab, transform);
        
        /* Apply random rotation */
        gameObject.transform.Rotate(Vector3.forward, Random.Range(-0.2f, 0.2f));
        
        return gameObject.GetComponent<CaseFilePageRenderer>();
    }
    
    private void GenerateFile() {
        CaseFilePageRenderer currentPage = NewPage();
        
        foreach (string statement in _statements) {
            GameObject statementObject = Instantiate(StatementPrefab);
            RectTransform statementRectTransform = statementObject.GetComponent<RectTransform>();

            if (!currentPage.TryAddStatement(statementRectTransform)) {
                _pages.Add(currentPage);
                currentPage = NewPage();
                currentPage.AddStatement(statementRectTransform);
            }
        }    
    }

    private void Start() {
        _pages = new List<CaseFilePageRenderer>();
        GenerateFile();
    }
}