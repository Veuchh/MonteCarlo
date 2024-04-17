using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetListHandler
{
    public static List<Card> SetList {
        get 
        {
            if (setList == null || setList.Count == 0)
            {
                setList = GenerateSetList();
            }
            return setList;
        }
        private set 
        {
            setList = value;
        }
    }

    static List<Card> setList;

    public static List<Card> GenerateSetList()
    {
        List<Card> newSetList = new List<Card>();
        for (int atk = 0; atk <= 12; atk++)
        {
            for (int def = 1; def <= 12; def++)
            {
                Card card = new Card(atk, def, newSetList.Count -1);
                if (card.Cost <= 6)
                {
                    newSetList.Add(card);
                }
            }
        }

        return newSetList;
    }

    public static Card GetRandomCard()
    {
        return SetList[Random.Range(0, SetList.Count - 1)];
    }
}
