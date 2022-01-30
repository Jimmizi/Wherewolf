using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseFileClose : MonoBehaviour
{
    public void CloseCaseFiles()
    {
        var cached = Service.CaseFile.charactersDropdown.SelectFirstItemOnStart;
        Service.CaseFile.charactersDropdown.SelectFirstItemOnStart = true;
        Service.CaseFile.charactersDropdown.DoInitialise();
        Service.CaseFile.charactersDropdown.SelectFirstItemOnStart = cached;
    }
}
