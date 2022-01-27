using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InformationManager : MonoBehaviour
{
    // Saved variables

    [SerializeField]
    private List<string> customNames = new List<string>();

    [SerializeField]
    private List<Emote> emoteList = new List<Emote>();

    // Runtime variables

    public SortedDictionary<Emote.EmoteType, List<Emote>> EmoteMap = new SortedDictionary<Emote.EmoteType, List<Emote>>();

    private List<string> availableNamePool = new List<string>();
    private List<string> inUseNamePool = new List<string>();

    // API Access

    public string GetRandomName()
    {
        int iRandomIndex = UnityEngine.Random.Range(0, availableNamePool.Count);
        string name = availableNamePool[iRandomIndex];

        availableNamePool.RemoveAt(iRandomIndex);
        inUseNamePool.Add(name);

        return name;
    }

    public Emote GetRandomEmoteOfType(Emote.EmoteType eType)
    {
        return EmoteMap[eType][UnityEngine.Random.Range(0, EmoteMap[eType].Count)];
    }

    public Emote GetCharacterEmote(Character c)
    {
        int iIndex = Service.Population.ActiveCharacters.IndexOf(c);
        if(iIndex >= 0 && iIndex < Service.Population.ActiveCharacters.Count)
        {
            return GetCharacterEmote(iIndex);
        }

        return null;
    }

    public Emote GetCharacterEmote(int iIndex)
    {
        Debug.Assert(iIndex >= 0 && iIndex <= 19);
        return EmoteMap[Emote.EmoteType.CharacterHeadshot][iIndex];
    }

    public void Awake()
    {
        Service.InfoManager = this;

        AddDefaultNames();
        availableNamePool.AddRange(customNames);

#if DEBUG
        // Utility to help us populate default emotes
        if (emoteList.Count == 0)
        {
            AddDefaultEmotes();
        }
#endif

        SortEmotes();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddDefaultNames()
    {
        availableNamePool.Add("John");
        availableNamePool.Add("William");
        availableNamePool.Add("James");
        availableNamePool.Add("George");
        availableNamePool.Add("Charles");
        availableNamePool.Add("Frank");
        availableNamePool.Add("Joseph");
        availableNamePool.Add("Henry");
        availableNamePool.Add("Robert");
        availableNamePool.Add("Thomas");
        availableNamePool.Add("Edward");
        availableNamePool.Add("Harry");
        availableNamePool.Add("Walter");
        availableNamePool.Add("Arthur");
        availableNamePool.Add("Fred");
        availableNamePool.Add("Albert");
        availableNamePool.Add("Samuel");
        availableNamePool.Add("Clarence");
        availableNamePool.Add("Louis");
        availableNamePool.Add("David");
        availableNamePool.Add("Joe");
        availableNamePool.Add("Charlie");
        availableNamePool.Add("Richard");
        availableNamePool.Add("Ernest");
        availableNamePool.Add("Roy");
        availableNamePool.Add("Will");
        availableNamePool.Add("Andrew");
        availableNamePool.Add("Jesse");
        availableNamePool.Add("Oscar");
        availableNamePool.Add("Willie");
        availableNamePool.Add("Daniel");
        availableNamePool.Add("Benjamin");
        availableNamePool.Add("Carl");
        availableNamePool.Add("Sam");
        availableNamePool.Add("Alfred");
        availableNamePool.Add("Earl");
        availableNamePool.Add("Peter");
        availableNamePool.Add("Elmer");
        availableNamePool.Add("Frederick");
        availableNamePool.Add("Howard");
        availableNamePool.Add("Lewis");
        availableNamePool.Add("Ralph");
        availableNamePool.Add("Herbert");
        availableNamePool.Add("Paul");
        availableNamePool.Add("Lee");
        availableNamePool.Add("Tom");
        availableNamePool.Add("Herman");
        availableNamePool.Add("Martin");
        availableNamePool.Add("Jacob");
        availableNamePool.Add("Michael");
        availableNamePool.Add("Jim");
        availableNamePool.Add("Claude");
        availableNamePool.Add("Ben");
        availableNamePool.Add("Eugene");
        availableNamePool.Add("Francis");
        availableNamePool.Add("Grover");
        availableNamePool.Add("Raymond");
        availableNamePool.Add("Harvey");
        availableNamePool.Add("Clyde");
        availableNamePool.Add("Edwin");
        availableNamePool.Add("Edgar");
        availableNamePool.Add("Ed");
        availableNamePool.Add("Lawrence");
        availableNamePool.Add("Bert");
        availableNamePool.Add("Chester");
        availableNamePool.Add("Jack");
        availableNamePool.Add("Otto");
        availableNamePool.Add("Luther");
        availableNamePool.Add("Charley");
        availableNamePool.Add("Guy");
        availableNamePool.Add("Floyd");
        availableNamePool.Add("Ira");
        availableNamePool.Add("Ray");
        availableNamePool.Add("Hugh");
        availableNamePool.Add("Isaac");
        availableNamePool.Add("Oliver");
        availableNamePool.Add("Patrick");
        availableNamePool.Add("Homer");
        availableNamePool.Add("Theodore");
        availableNamePool.Add("Leonard");
        availableNamePool.Add("Leo");
        availableNamePool.Add("Alexander");
        availableNamePool.Add("August");
        availableNamePool.Add("Harold");
        availableNamePool.Add("Allen");
        availableNamePool.Add("Jessie");
        availableNamePool.Add("Archie");
        availableNamePool.Add("Philip");
        availableNamePool.Add("Stephen");
        availableNamePool.Add("Horace");
        availableNamePool.Add("Marion");
        availableNamePool.Add("Bernard");
        availableNamePool.Add("Anthony");
        availableNamePool.Add("Julius");
        availableNamePool.Add("Warren");
        availableNamePool.Add("Leroy");
        availableNamePool.Add("Clifford");
        availableNamePool.Add("Eddie");
        availableNamePool.Add("Sidney");
        availableNamePool.Add("Milton");
        availableNamePool.Add("Leon");
        availableNamePool.Add("Alex");
        availableNamePool.Add("Lester");
        availableNamePool.Add("Emil");
        availableNamePool.Add("Dan");
        availableNamePool.Add("Willis");
        availableNamePool.Add("Everett");
        availableNamePool.Add("Dave");
        availableNamePool.Add("Leslie");
        availableNamePool.Add("Rufus");
        availableNamePool.Add("Alvin");
        availableNamePool.Add("Perry");
        availableNamePool.Add("Lloyd");
        availableNamePool.Add("Victor");
        availableNamePool.Add("Calvin");
        availableNamePool.Add("Harrison");
        availableNamePool.Add("Norman");
        availableNamePool.Add("Wesley");
        availableNamePool.Add("Jess");
        availableNamePool.Add("Percy");
        availableNamePool.Add("Amos");
        availableNamePool.Add("Dennis");
        availableNamePool.Add("Jerry");
        availableNamePool.Add("Nathan");
        availableNamePool.Add("Franklin");
        availableNamePool.Add("Alonzo");
        availableNamePool.Add("Matthew");
        availableNamePool.Add("Mack");
        availableNamePool.Add("Earnest");
        availableNamePool.Add("Gus");
        availableNamePool.Add("Russell");
        availableNamePool.Add("Adam");
        availableNamePool.Add("Jay");
        availableNamePool.Add("Wallace");
        availableNamePool.Add("Otis");
        availableNamePool.Add("Stanley");
        availableNamePool.Add("Adolph");
        availableNamePool.Add("Jake");
        availableNamePool.Add("Roscoe");
        availableNamePool.Add("Maurice");
        availableNamePool.Add("Melvin");
        availableNamePool.Add("Gilbert");
        availableNamePool.Add("Ross");
        availableNamePool.Add("Willard");
        availableNamePool.Add("Mark");
        availableNamePool.Add("Levi");
        availableNamePool.Add("Wilbur");
        availableNamePool.Add("Cornelius");
        availableNamePool.Add("Jose");
        availableNamePool.Add("Aaron");
        availableNamePool.Add("Elbert");
        availableNamePool.Add("Emmett");
        availableNamePool.Add("Phillip");
        availableNamePool.Add("Morris");
        availableNamePool.Add("Noah");
        availableNamePool.Add("Claud");
        availableNamePool.Add("Clinton");
        availableNamePool.Add("Felix");
        availableNamePool.Add("Moses");
        availableNamePool.Add("Elijah");
        availableNamePool.Add("Nelson");
        availableNamePool.Add("Simon");
        availableNamePool.Add("Lonnie");
        availableNamePool.Add("Virgil");
        availableNamePool.Add("Hiram");
        availableNamePool.Add("Jasper");
        availableNamePool.Add("Marshall");
        availableNamePool.Add("Manuel");
        availableNamePool.Add("Sylvester");
        availableNamePool.Add("Fredrick");
        availableNamePool.Add("Mike");
        availableNamePool.Add("Abraham");
        availableNamePool.Add("Silas");
        availableNamePool.Add("Irvin");
        availableNamePool.Add("Max");
        availableNamePool.Add("Owen");
        availableNamePool.Add("Christopher");
        availableNamePool.Add("Reuben");
        availableNamePool.Add("Glenn");
        availableNamePool.Add("Nicholas");
        availableNamePool.Add("Ellis");
        availableNamePool.Add("Marvin");
        availableNamePool.Add("Wiley");
        availableNamePool.Add("Eli");
        availableNamePool.Add("Edmund");
        availableNamePool.Add("Ollie");
        availableNamePool.Add("Cecil");
        availableNamePool.Add("Cleveland");
        availableNamePool.Add("Curtis");
        availableNamePool.Add("Timothy");
        availableNamePool.Add("Harley");
        availableNamePool.Add("Jeff");
        availableNamePool.Add("Anton");
        availableNamePool.Add("Alva");
        availableNamePool.Add("Wilson");
        availableNamePool.Add("Irving");
        availableNamePool.Add("Clayton");
        availableNamePool.Add("Rudolph");
        availableNamePool.Add("Vernon");
        availableNamePool.Add("Hubert");
        availableNamePool.Add("Mary");
        availableNamePool.Add("Anna");
        availableNamePool.Add("Emma");
        availableNamePool.Add("Elizabeth");
        availableNamePool.Add("Margaret");
        availableNamePool.Add("Minnie");
        availableNamePool.Add("Ida");
        availableNamePool.Add("Bertha");
        availableNamePool.Add("Clara");
        availableNamePool.Add("Alice");
        availableNamePool.Add("Annie");
        availableNamePool.Add("Florence");
        availableNamePool.Add("Bessie");
        availableNamePool.Add("Grace");
        availableNamePool.Add("Ethel");
        availableNamePool.Add("Sarah");
        availableNamePool.Add("Ella");
        availableNamePool.Add("Martha");
        availableNamePool.Add("Nellie");
        availableNamePool.Add("Mabel");
        availableNamePool.Add("Laura");
        availableNamePool.Add("Carrie");
        availableNamePool.Add("Cora");
        availableNamePool.Add("Helen");
        availableNamePool.Add("Maude");
        availableNamePool.Add("Lillian");
        availableNamePool.Add("Gertrude");
        availableNamePool.Add("Rose");
        availableNamePool.Add("Edna");
        availableNamePool.Add("Pearl");
        availableNamePool.Add("Edith");
        availableNamePool.Add("Jennie");
        availableNamePool.Add("Hattie");
        availableNamePool.Add("Mattie");
        availableNamePool.Add("Eva");
        availableNamePool.Add("Julia");
        availableNamePool.Add("Myrtle");
        availableNamePool.Add("Louise");
        availableNamePool.Add("Lillie");
        availableNamePool.Add("Jessie");
        availableNamePool.Add("Frances");
        availableNamePool.Add("Catherine");
        availableNamePool.Add("Lula");
        availableNamePool.Add("Lena");
        availableNamePool.Add("Marie");
        availableNamePool.Add("Ada");
        availableNamePool.Add("Josephine");
        availableNamePool.Add("Fannie");
        availableNamePool.Add("Lucy");
        availableNamePool.Add("Dora");
        availableNamePool.Add("Agnes");
        availableNamePool.Add("Maggie");
        availableNamePool.Add("Katherine");
        availableNamePool.Add("Elsie");
        availableNamePool.Add("Nora");
        availableNamePool.Add("Mamie");
        availableNamePool.Add("Rosa");
        availableNamePool.Add("Stella");
        availableNamePool.Add("Daisy");
        availableNamePool.Add("May");
        availableNamePool.Add("Effie");
        availableNamePool.Add("Mae");
        availableNamePool.Add("Ellen");
        availableNamePool.Add("Nettie");
        availableNamePool.Add("Ruth");
        availableNamePool.Add("Alma");
        availableNamePool.Add("Della");
        availableNamePool.Add("Lizzie");
        availableNamePool.Add("Sadie");
        availableNamePool.Add("Sallie");
        availableNamePool.Add("Nancy");
        availableNamePool.Add("Susie");
        availableNamePool.Add("Maud");
        availableNamePool.Add("Flora");
        availableNamePool.Add("Irene");
        availableNamePool.Add("Etta");
        availableNamePool.Add("Katie");
        availableNamePool.Add("Lydia");
        availableNamePool.Add("Lottie");
        availableNamePool.Add("Viola");
        availableNamePool.Add("Caroline");
        availableNamePool.Add("Addie");
        availableNamePool.Add("Hazel");
        availableNamePool.Add("Georgia");
        availableNamePool.Add("Esther");
        availableNamePool.Add("Mollie");
        availableNamePool.Add("Olive");
        availableNamePool.Add("Willie");
        availableNamePool.Add("Harriet");
        availableNamePool.Add("Emily");
        availableNamePool.Add("Charlotte");
        availableNamePool.Add("Amanda");
        availableNamePool.Add("Kathryn");
        availableNamePool.Add("Lulu");
        availableNamePool.Add("Susan");
        availableNamePool.Add("Kate");
        availableNamePool.Add("Nannie");
        availableNamePool.Add("Jane");
        availableNamePool.Add("Amelia");
        availableNamePool.Add("Virginia");
        availableNamePool.Add("Mildred");
        availableNamePool.Add("Beulah");
        availableNamePool.Add("Eliza");
        availableNamePool.Add("Rebecca");
        availableNamePool.Add("Ollie");
        availableNamePool.Add("Belle");
        availableNamePool.Add("Ruby");
        availableNamePool.Add("Pauline");
        availableNamePool.Add("Matilda");
        availableNamePool.Add("Theresa");
        availableNamePool.Add("Hannah");
        availableNamePool.Add("Henrietta");
        availableNamePool.Add("Ora");
        availableNamePool.Add("Estella");
        availableNamePool.Add("Leona");
        availableNamePool.Add("Augusta");
        availableNamePool.Add("Eleanor");
        availableNamePool.Add("Rachel");
        availableNamePool.Add("Amy");
        availableNamePool.Add("Sara");
        availableNamePool.Add("Anne");
        availableNamePool.Add("Marion");
        availableNamePool.Add("Ivav");
        availableNamePool.Add("Ann");
        availableNamePool.Add("Nina");
        availableNamePool.Add("Dorothy");
        availableNamePool.Add("Lola");
        availableNamePool.Add("Lela");
        availableNamePool.Add("Beatrice");
        availableNamePool.Add("Josie");
        availableNamePool.Add("Sophia");
        availableNamePool.Add("Estelle");
        availableNamePool.Add("Mayme");
        availableNamePool.Add("Barbara");
        availableNamePool.Add("Evelyn");
        availableNamePool.Add("Maria");
        availableNamePool.Add("Inez");
        availableNamePool.Add("Allie");
        availableNamePool.Add("Essie");
        availableNamePool.Add("Delia");
        availableNamePool.Add("Mable");
        availableNamePool.Add("Millie");
        availableNamePool.Add("Alta");
        availableNamePool.Add("Betty");
        availableNamePool.Add("Callie");
        availableNamePool.Add("Janie");
        availableNamePool.Add("Rosie");
        availableNamePool.Add("Victoria");
        availableNamePool.Add("Ola");
        availableNamePool.Add("Gladys");
        availableNamePool.Add("Louisa");
        availableNamePool.Add("Ina");
        availableNamePool.Add("Eula");
        availableNamePool.Add("Luella");
        availableNamePool.Add("Vera");
        availableNamePool.Add("Lou");
        availableNamePool.Add("Celia");
        availableNamePool.Add("Nell");
        availableNamePool.Add("Goldie");
        availableNamePool.Add("Winifred");
        availableNamePool.Add("Bettie");
        availableNamePool.Add("Hilda");
        availableNamePool.Add("Sophie");
        availableNamePool.Add("Christine");
        availableNamePool.Add("Marguerite");
        availableNamePool.Add("Tillie");
        availableNamePool.Add("Birdie");
        availableNamePool.Add("Rena");
        availableNamePool.Add("Eunice");
        availableNamePool.Add("Bertie");
        availableNamePool.Add("Olga");
        availableNamePool.Add("Sylvia");
        availableNamePool.Add("Lucille");
        availableNamePool.Add("Bess");
        availableNamePool.Add("Isabelle");
        availableNamePool.Add("Genevieve");
        availableNamePool.Add("Leila");
        availableNamePool.Add("Mathilda");
        availableNamePool.Add("Dollie");
        availableNamePool.Add("Isabel");
        availableNamePool.Add("Verna");
        availableNamePool.Add("Bernice");
        availableNamePool.Add("Loretta");
        availableNamePool.Add("Rhoda");
        availableNamePool.Add("Cornelia");
        availableNamePool.Add("Sally");
        availableNamePool.Add("Jean");
        availableNamePool.Add("Alberta");
        availableNamePool.Add("Winnie");
        availableNamePool.Add("Lelia");
        availableNamePool.Add("Lois");
        availableNamePool.Add("Myra");
        availableNamePool.Add("Harriett");
        availableNamePool.Add("Roxie");
        availableNamePool.Add("Adeline");
        availableNamePool.Add("Abbie");
        availableNamePool.Add("Flossie");
        availableNamePool.Add("Sue");
        availableNamePool.Add("Christina");
    }

    void SortEmotes()
    {
        foreach (var emote in emoteList)
        {
            if (!EmoteMap.ContainsKey(emote.Type))
            {
                EmoteMap.Add(emote.Type, new List<Emote>());
            }

            EmoteMap[emote.Type].Add(emote);
        }
    }

#if DEBUG
    void AddDefaultEmotes()
    {
        for (int i = 0; i <= (int)Emote.EmoteSubType.Gossip_RelationshipMarriage; ++i)
        {
            Emote.EmoteSubType eType = (Emote.EmoteSubType)i;
            string sType = eType.ToString();

            Emote newEmote = new Emote();

            // Name
            newEmote.Name = sType.Substring(sType.IndexOf('_') + 1);
            for(int c = 0; c < newEmote.Name.Length; ++c)
            {
                if(c > 0 && char.IsUpper(newEmote.Name[c]))
                {
                    newEmote.Name = newEmote.Name.Insert(c, " ");
                    c++;
                }
            }

            // Type
            if (i <= (int)Emote.EmoteSubType.HairStyle_Bald)
            {
                newEmote.Type = Emote.EmoteType.HairStyle;
            }
            else if (i >= (int)Emote.EmoteSubType.Facial_Ugly && i <= (int)Emote.EmoteSubType.Facial_BigEars)
            {
                newEmote.Type = Emote.EmoteType.FacialFeature;
            }
            else if (i >= (int)Emote.EmoteSubType.Color_Brown && i <= (int)Emote.EmoteSubType.Color_Purple)
            {
                newEmote.Type = Emote.EmoteType.Color;
            }
            else if (i >= (int)Emote.EmoteSubType.Location_1 && i <= (int)Emote.EmoteSubType.Location_9)
            {
                newEmote.Type = Emote.EmoteType.Location;
            }
            else if (i >= (int)Emote.EmoteSubType.Occupation_Bank && i <= (int)Emote.EmoteSubType.Occupation_GeneralStore)
            {
                newEmote.Type = Emote.EmoteType.Occupation;
            }
            else if (i >= (int)Emote.EmoteSubType.TimeOfDay_Day && i <= (int)Emote.EmoteSubType.TimeOfDay_Night)
            {
                newEmote.Type = Emote.EmoteType.TimeOfDay;
            }
            else if (i >= (int)Emote.EmoteSubType.Condition_Dirty && i <= (int)Emote.EmoteSubType.Condition_Torn)
            {
                newEmote.Type = Emote.EmoteType.Condition;
            }
            else if (i >= (int)Emote.EmoteSubType.Clothing_Shirt && i <= (int)Emote.EmoteSubType.Clothing_Gloves)
            {
                newEmote.Type = Emote.EmoteType.Clothing;
            }
            else if (i >= (int)Emote.EmoteSubType.Specific_Footsteps && i <= (int)Emote.EmoteSubType.Specific_Werewolf)
            {
                newEmote.Type = Emote.EmoteType.Specific;
            }
            else if (i >= (int)Emote.EmoteSubType.Gossip_RelationshipKiss && i <= (int)Emote.EmoteSubType.Gossip_RelationshipMarriage)
            {
                newEmote.Type = Emote.EmoteType.Gossip;
            }
            else if (i >= (int)Emote.EmoteSubType.CharacterHeadshot_1 && i <= (int)Emote.EmoteSubType.CharacterHeadshot_20)
            {
                newEmote.Type = Emote.EmoteType.CharacterHeadshot;
            }
            else if (i >= (int)Emote.EmoteSubType.Opinion_Love && i <= (int)Emote.EmoteSubType.Opinion_Hate)
            {
                newEmote.Type = Emote.EmoteType.Opinion;
            }

            // SubType
            newEmote.SubType = (Emote.EmoteSubType)i;

            var spr = Resources.Load<Sprite>("Emotes/" + sType);
            if(spr != null)
            {
                newEmote.EmoteImage = spr;
            }

            emoteList.Add(newEmote);
        }
    }
#endif
}
