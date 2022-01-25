using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialogue class, with all the sentences that will be displayed in the Status
/// </summary>
public class Dialogue {
    public string Speaker;
    
    /// <summary>
    /// A dialog <see cref="Sentence"/>.
    /// </summary>
    public Sentence Sentence;
    
    public List<Choice> Choices;
    
    private int _invocations;

    public int Invocations {
        get => _invocations;
        set => _invocations = value;
    }
}