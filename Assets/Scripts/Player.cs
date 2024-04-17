using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    //SETUP VARIABLES
    int playerIndex;
    List<Card> fullDeck = new List<Card>();
    int deckSize = 30;
    int maxCardDuplicate = 2;
    int maxPV = 20;
    int startMana = 0;
    int startHandSize = 4;
    int maxHP = 20;

    //GAME VARIABLE
    List<Card> remainingDeck = new List<Card>();
    List<Card> hand = new List<Card>();
    List<Card> board = new List<Card>();
    int remainingHP;
    int currentMaxMana;
    int remainingMana;

    //Montecarlo variable
    List<Card> fullDeckPreviousRound = new List<Card>();
    int previousWonRound = 0;
    int wonRound = 0;

    //Properties
    public List<Card> Board => board;
    public int PlayerIndex => playerIndex;
    public int WonRound => wonRound;
    public int RemainingHP => remainingHP;

    public Player(int index)
    {
        fullDeck = GenerateRandomDeck();
        fullDeckPreviousRound = new List<Card>(fullDeck);
        playerIndex = index;
    }

    public Player(int index, List<Card> playerDeck)
    {
        fullDeck = playerDeck;
        fullDeckPreviousRound = new List<Card>(fullDeck);
        playerIndex = index;
    }

    List<Card> GenerateRandomDeck()
    {
        List<Card> newDeck = new List<Card>();

        while (newDeck.Count < deckSize)
        {
            Card newCard = SetListHandler.GetRandomCard();

            int cardAlreadyInDeckAmount = 0;

            foreach (Card card in newDeck)
            {
                if (card.Index == newCard.Index)
                {
                    cardAlreadyInDeckAmount++;

                    if (maxCardDuplicate <= cardAlreadyInDeckAmount)
                    {
                        break;
                    }
                }
            }
            if (maxCardDuplicate <= cardAlreadyInDeckAmount)
                continue;

            newDeck.Add(newCard);
        }

        return newDeck;
    }

    public void PrintAllDeck()
    {
        foreach (var card in fullDeck)
        {
            Debug.Log(card.ToString());
        }
    }

    public void InitPlayer()
    {
        remainingHP = maxHP;
        currentMaxMana = startMana;
        remainingMana = startMana;

        remainingDeck = new List<Card>(fullDeck);
        board = new List<Card>();
        hand = new List<Card>();

        for (int i = 0; i < startHandSize; i++)
        {
            DrawCardFromDeckToHand();
        }
    }

    public void DrawCardFromDeckToHand()
    {
        if (remainingDeck.Count <= 0)
            return;

        Card drawedCard = remainingDeck[UnityEngine.Random.Range(0, remainingDeck.Count - 1)];
        remainingDeck.Remove(drawedCard);
        hand.Add(drawedCard);
    }

    public void PlayAllPossibleCards()
    {
        while (true)
        {
            Card cardToPlay = null;

            foreach (Card cardInHand in hand)
            {
                if (cardInHand.Cost <= remainingMana && (cardToPlay == null || cardToPlay.Cost < cardInHand.Cost))
                {
                    cardToPlay = cardInHand;
                }
            }

            if (cardToPlay == null)
            {
                break;
            }
            else
            {
                hand.Remove(cardToPlay);
                board.Add(cardToPlay);
                remainingMana -= cardToPlay.Cost;
            }
        }
    }

    public void AttackPlayer(int atk)
    {
        remainingHP -= atk;
    }

    internal void OnNewTurn()
    {
        currentMaxMana++;
        remainingMana = currentMaxMana;
        DrawCardFromDeckToHand();
    }

    public void ApplyRandom()
    {
        if (WonRound < previousWonRound)
        {
            //reset to previous deck
            fullDeck = new List<Card>(fullDeckPreviousRound);
        }

        fullDeckPreviousRound = new List<Card>(fullDeck);

        int indexToRemove = UnityEngine.Random.Range(0, fullDeck.Count - 1);

        fullDeck.RemoveAt(indexToRemove);

        while (fullDeck.Count < deckSize)
        {
            Card newCard = SetListHandler.GetRandomCard();

            int cardAlreadyInDeckAmount = 0;

            foreach (Card card in fullDeck)
            {
                if (card.Index == newCard.Index)
                {
                    cardAlreadyInDeckAmount++;

                    if (maxCardDuplicate <= cardAlreadyInDeckAmount)
                    {
                        break;
                    }
                }
            }
            if (maxCardDuplicate <= cardAlreadyInDeckAmount)
                continue;

            fullDeck.Add(newCard);
        }

        if (previousWonRound <= WonRound)
        {
            previousWonRound = WonRound;
        }

        wonRound = 0;
    }

    public void OnWin()
    {
        wonRound++;
    }
}
