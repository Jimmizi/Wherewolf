using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class CaseFileRenderer : MonoBehaviour {
    
    public GameObject PagePrefab;
    
    public GameObject CharacterAttributesPrefab;
    public GameObject StatementPrefab;
    public Canvas Canvas;
    public DropDownList charactersDropdown;

    public Dictionary<Character, List<CaseFilePageRenderer>> CharacterPagesMap = new Dictionary<Character, List<CaseFilePageRenderer>>();

    private Character characterPagesToDisplay = null;

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

    private CaseFilePageRenderer NewPage(Character c) {
        if (PagePrefab == null) return null;

        GameObject gameObject = Instantiate(PagePrefab); //, transform);
        
        /* Apply random rotation */
        gameObject.transform.Rotate(Vector3.forward, Random.Range(-4f, 4f));
        
        CaseFilePageRenderer pageRenderer = gameObject.GetComponent<CaseFilePageRenderer>();
        pageRenderer.SetCanvas(Canvas);
        CharacterPagesMap[c].Add(pageRenderer);
        
        return pageRenderer;
    }

    private void HideAll()
    {
        foreach(var pagelist in CharacterPagesMap)
        {
            foreach(var page in pagelist.Value)
            {
                page.gameObject.SetActive(false);
            }
        }
    }

    private void ShowForCharacter(Character c)
    {
        foreach (var page in CharacterPagesMap[c])
        {
            page.gameObject.SetActive(true);
        }
    }
    private void HideForCharacter(Character c)
    {
        foreach (var page in CharacterPagesMap[c])
        {
            page.gameObject.SetActive(false);
        }
    }

    private void OnChangedCharacterSelection(int index)
    {
        
    }

    public void AddClueToFile(ClueObject clue)
    {
        if (!CharacterPagesMap.ContainsKey(clue.RelatesToCharacter))
        {
            CharacterPagesMap.Add(clue.RelatesToCharacter, new List<CaseFilePageRenderer>());
        }

        AddStatement(clue.RelatesToCharacter, clue);
    }

    private void AddStatement(Character c, ClueObject clue)
    {
        CaseFilePageRenderer currentPage;

        if (CharacterPagesMap[c].Count == 0)
        {
            currentPage = NewPage(c);

            // First page gen

            GameObject attributesObject = Instantiate(CharacterAttributesPrefab);
            RectTransform attributesRectTransform = attributesObject.GetComponent<RectTransform>();

            CaseFileAttributesCollection attributeCollection = attributesObject.GetComponent<CaseFileAttributesCollection>();
            if (attributeCollection)
            {
                attributeCollection.NameText.text = c.Name;
                attributeCollection.AgeText.text = c.Age.ToString();
                attributeCollection.ProfilePicture.SetAttributes(CharacterGenerator.Instance.AttributesForEmoteType(c.GetHeadshotEmoteSubType()));
            }

            currentPage.AddSection(attributesRectTransform);

            PopulateCharactersDropdown();
        }
        else
        {
            currentPage = CharacterPagesMap[c][CharacterPagesMap[c].Count - 1];
        }

        GameObject statementObject = Instantiate(StatementPrefab);
        RectTransform statementRectTransform = statementObject.GetComponent<RectTransform>();
        CaseFileStatementRenderer statementRenderer = statementObject.GetComponent<CaseFileStatementRenderer>();

        TextMeshProUGUI statementName = statementRenderer.GetComponentInChildren<TextMeshProUGUI>();
        if(statementName)
        {
            statementName.text = string.Format("Day {0}, {1}", Service.Game.CurrentDay, Service.Game.CurrentTimeOfDay.ToString());
        }


        statementRenderer.Render(clue.Emotes);

        if (!currentPage.TryAddSection(statementRectTransform))
        {
            currentPage = NewPage(c);
            currentPage.AddSection(statementRectTransform);
        }

        for (int i = CharacterPagesMap[c].Count - 1; i >= 0; i--)
        {
            CharacterPagesMap[c][i].transform.SetParent(transform, false);
            CharacterPagesMap[c][i].gameObject.SetActive(false);
        }
    }
    
    private void GenerateFile(Character c) 
    {
        CaseFilePageRenderer currentPage = NewPage(c);
        
        /* Render attributes at the top of the first page. */
        GameObject attributesObject = Instantiate(CharacterAttributesPrefab);
        RectTransform attributesRectTransform = attributesObject.GetComponent<RectTransform>();

        CaseFileAttributesCollection attributeCollection = attributesObject.GetComponent<CaseFileAttributesCollection>();
        if(attributeCollection)
        {
            attributeCollection.NameText.text = c.Name;
            attributeCollection.AgeText.text = c.Age.ToString();
            attributeCollection.ProfilePicture.SetAttributes(CharacterGenerator.Instance.AttributesForEmoteType(c.GetHeadshotEmoteSubType()));
        }

        currentPage.AddSection(attributesRectTransform);
        
        /* Render statements on succesive pages. */
        foreach (string statement in _statements) 
        {
            GameObject statementObject = Instantiate(StatementPrefab);
            RectTransform statementRectTransform = statementObject.GetComponent<RectTransform>();
            CaseFileStatementRenderer statementRenderer = statementObject.GetComponent<CaseFileStatementRenderer>();

            if(Service.InfoManager != null)
            {
                if (statementRenderer != null)
                {
                    List<Emote> emotes = new List<Emote> 
                    {
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
            else if (statementRenderer != null) 
            {
                List<Emote> emotes = new List<Emote> 
                {
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50)),
                    new Emote((Emote.EmoteSubType) Random.Range(0, 50))
                };
                
                statementRenderer.Render(emotes);
            }
            
            if (!currentPage.TryAddSection(statementRectTransform)) 
            {
                currentPage = NewPage(c);
                currentPage.AddSection(statementRectTransform);
            }
        }
        
        for (int i = CharacterPagesMap[c].Count - 1; i >= 0; i--) 
        {
            CharacterPagesMap[c][i].transform.SetParent(transform, false);
            CharacterPagesMap[c][i].gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        Service.CaseFile = this;
        charactersDropdown.OnSelectionChanged.AddListener(OnChangedCharacterSelection);
    }

    private void Start() {
        
        PopulateCharactersDropdown();
    }

    private void Update()
    {
        if (charactersDropdown != null)
        {
            DropDownListItem<Character> itemToUse = (DropDownListItem<Character>)charactersDropdown.SelectedItem;
            if (itemToUse == null || itemToUse.Data == null)
            {
                return;
            }

            if(itemToUse.Data.Name == "None")
            {
                HideAll();
                characterPagesToDisplay = null;
                return;
            }

            if (characterPagesToDisplay != itemToUse.Data)
            {
                HideAll();

                characterPagesToDisplay = itemToUse.Data;
                ShowForCharacter(characterPagesToDisplay);
            }
        }
    }

    public void BringUpPages()
    {
        PopulateCharactersDropdown();
    }

    public void PopulateCharactersDropdown()
    {
        if (charactersDropdown == null) return;

        HideAll();
        if(!charactersDropdown.Initialised)
        {
            charactersDropdown.DoInitialise();
        }
        charactersDropdown.ResetItems();

        // Add a dummy character so we can always select to get a random character clue
        Character dummyCharacter = new Character()
        {
            Name = "None"
        };

        charactersDropdown.AddItem(new DropDownListItem<Character>(data: dummyCharacter));

        foreach (var c in CharacterPagesMap)
        {
            charactersDropdown.AddItem(new DropDownListItem<Character>(data: c.Key));
        }
    }
}