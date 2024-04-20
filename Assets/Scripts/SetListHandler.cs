using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SetListHandler
{
    public static List<Card> optimizedSetList = new List<Card>();
    public static List<Card> OptimizedSetList => optimizedSetList;
    public static int cardMaxCost = 8;

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

    public static void ComputeOptimizedSetList()
    {
        optimizedSetList = SetList.Where(c => c.ValueIndex > 0).Where(c=>(c.Defense>0||c.HasProvocation)).Where(c=>(c.HasTrample && c.Attack > 0) || !c.HasTrample).ToList();
    }

    public static List<Card> GenerateSetList()
    {
        string str = "";
        List<Card> newSetList = new List<Card>();
        for (int atk = 0; atk <= cardMaxCost * 2; atk++)
        {
            for (int def = 1; def <= cardMaxCost * 2; def++)
            {
                for (int provocation = 0; provocation <= 1; provocation++)
                {
                    for (int stomp = 0; stomp <= 1; stomp++)
                    {
                        for (int distortion = 0; distortion <= 1; distortion++)
                        {
                            for (int firstStrike = 0; firstStrike <= 1; firstStrike++)
                            {
                                Card card = new Card(atk, def, newSetList.Count, provocation == 1, stomp == 1, distortion == 1, firstStrike == 1);

                                if (card.Cost <= cardMaxCost)
                                {
                                    newSetList.Add(card);
                                    str += card.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        GUIUtility.systemCopyBuffer = str;

        return newSetList;
    }

    public static void ResetSetList()
    {
        optimizedSetList = new List<Card>();
        setList = new List<Card>();
    }

    public static Card GetRandomCard()
    {
        return SetList[Random.Range(0, SetList.Count - 1)];
    }

    public static Card GetRandomCardFromOptimizedSetList()
    {
        return OptimizedSetList[Random.Range(0, OptimizedSetList.Count - 1)];
    }

    public static List<Card> GetFirstXCards(int cardAmount)
    {
        List<Card> ordererdSetList = SetList.OrderBy(c => c.Cost).ToList();
        List<Card> cards = new List<Card>();
        for (int i = 0; i < cardAmount / 2; i++)
        {
            cards.Add(ordererdSetList.Where(c => c.Cost == 2).ToList()[i]);
        }
        for (int i = 0; i < cardAmount / 2; i++)
        {
            cards.Add(ordererdSetList.Where(c => c.Cost == 3).ToList()[i]);
        }

        int additionlCardIndex = 0;

        while (cards.Count < cardAmount)
        {
            cards.Add(ordererdSetList[additionlCardIndex]);
            additionlCardIndex++;
        }

        return cards;
    }

}
