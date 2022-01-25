using System.Collections.Generic;


/// <summary>
/// The Conversation Class contains all the posible Dialogues the NPC or situation can have.
/// </summary>
public struct Conversation {
    /// <summary> Name of the conversation. </summary>
    public string Name;
    
    public List<Dialogue> Dialogues;
}