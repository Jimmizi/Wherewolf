using System.Collections.Generic;

public class Dialogue {
    public Character Speaker;
    public List<Emote> Sentence;

    public Dialogue(Character speaker, List<Emote> sentence) {
        Speaker = speaker;
        Sentence = sentence;
    }

    public static Dialogue FromClue(ClueObject clue) {
        Dialogue dialogue = new Dialogue(clue.GivenByCharacter, clue.Emotes);
        return dialogue;
    }
}