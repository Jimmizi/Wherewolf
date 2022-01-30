using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MemoPileOfRandomEmotes : MemoPile
{
    protected override Memo CreateNewMemo()
    {
        List<Emote> emotes = new List<Emote>();
        int numEmotes = Random.Range(1, 6);
        
        int numEmoteTypes = Enum.GetNames(typeof(Emote.EmoteSubType)).Length;

        for (int i=0; i< numEmotes; i++)
        {
            var randomEmoteSubType = (Emote.EmoteSubType)Random.Range(0, numEmoteTypes);
            emotes.Add(Service.InfoManager.EmoteMapBySubType[randomEmoteSubType]);
        }

        return MemoFactory.instance.CreateNew("Rando", emotes, rectTransform.anchoredPosition, false);
    }
}
