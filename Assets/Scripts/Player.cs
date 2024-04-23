using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    //SETUP VARIABLES
    int playerIndex;
    List<Card> fullDeck = new List<Card>();
    List<Card> fullDeckAtStart = new List<Card>();
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
    Card removedCard;
    Card addedCard;
    List<Card> completeReferenceDeck = new List<Card>();
    int bestWinRate = 0;
    int currentWinRate = 0;

    //Properties
    public List<Card> FullDeckAtStart => fullDeckAtStart;
    public List<Card> CompleteReferenceDeck => completeReferenceDeck;
    public List<Card> FullDeck { get { return fullDeck; } set { fullDeck = value; } }
    public List<Card> Board => board;
    public int PlayerIndex => playerIndex;
    public int CurrentWinRate { get { return currentWinRate; } set { currentWinRate = value; } }
    public int RemainingHP => remainingHP;

    public Player(int index, bool useLowCostCards = true)
    {
        if (useLowCostCards)
            fullDeck = SetListHandler.GetFirstXCards(deckSize);
        else
            GetRandomDeckFromSetList(false);
        {

        }
        completeReferenceDeck = new List<Card>(fullDeck);
        fullDeckAtStart = new List<Card>(fullDeck);
        playerIndex = index;
    }

    public Player(int index, List<Card> playerDeck)
    {
        fullDeck = playerDeck;
        completeReferenceDeck = new List<Card>(fullDeck);
        fullDeckAtStart = new List<Card>(fullDeck);
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
                board.Add(new Card(cardToPlay));
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

    public void ApplyRandom(bool trainOnOptimizedList)
    {
        if (CurrentWinRate < bestWinRate)
        {
            //reset to previous deck
            fullDeck = new List<Card>(completeReferenceDeck);
            if (!trainOnOptimizedList)
            {
                removedCard.AddToValueIndex(1);
                addedCard.AddToValueIndex(-1);
            }
        }
        else if (!trainOnOptimizedList)
        {
            removedCard?.AddToValueIndex(-1);
            addedCard?.AddToValueIndex(1);
        }

        completeReferenceDeck = new List<Card>(fullDeck);

        int indexToRemove = UnityEngine.Random.Range(0, fullDeck.Count - 1);

        removedCard = fullDeck[indexToRemove];

        fullDeck.RemoveAt(indexToRemove);

        while (fullDeck.Count < deckSize)
        {
            Card newCard = trainOnOptimizedList ? SetListHandler.GetRandomCardFromOptimizedSetList() : SetListHandler.GetRandomCard();

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

            addedCard = newCard;
            fullDeck.Add(newCard);
        }

        if (bestWinRate <= CurrentWinRate)
        {
            bestWinRate = CurrentWinRate;
        }

        currentWinRate = 0;
    }

    public void OnWin()
    {
        currentWinRate++;
    }

    public void ResetWinRate()
    {
        currentWinRate = 0;
        bestWinRate = 0;
    }

    public void GetRandomDeckFromSetList(bool useOptimizedSetList = false)
    {
        fullDeck = new List<Card>();

        while (fullDeck.Count < deckSize)
        {
            Card newCard = useOptimizedSetList ? SetListHandler.GetRandomCardFromOptimizedSetList() : SetListHandler.GetRandomCard();

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
    }

    public void KillCardOnBoard(Card killedCard)
    {
        board.Remove(killedCard);
    }
}
