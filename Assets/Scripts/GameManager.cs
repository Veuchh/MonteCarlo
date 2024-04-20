using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playersAmount = 2;
    [SerializeField] System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();
    List<Player> alivePlayers = new List<Player>();

    string output = "";
    float totalTurnThisRound = 0;

    [Button]
    void ResetSetList()
    {
        SetListHandler.ResetSetList();
    }

    [Button]
    void GeneratePlayers()
    {
        for (int i = 0; i < playersAmount; i++)
        {
            players.Add(new Player(i));
        }
    }

    [Button]
    void PlayOneGame()
    {
        foreach (var player in players)
        {
            player.InitPlayer();
        }

        alivePlayers = new List<Player>(players);

        int maxTurn = 400;

        bool isTimedOut = false;

        for (int turnIndex = 0; turnIndex < maxTurn; turnIndex++)
        {
            totalTurnThisRound++;
            //Play each player turn
            for (int playerIndex = alivePlayers.Count - 1; playerIndex >= 0; playerIndex--)
            {
                alivePlayers[playerIndex].OnNewTurn();
                //play all the cards you can
                alivePlayers[playerIndex].PlayAllPossibleCards();

                Player attackedPlayer = alivePlayers[
                            (playerIndex - 1) < 0
                            ? alivePlayers.Count - 1
                            : playerIndex - 1];

                AttackPlayer(playerIndex, attackedPlayer);

                if (attackedPlayer.RemainingHP <= 0)
                {
                    alivePlayers.Remove(attackedPlayer);
                    playerIndex--;
                }

                //check if only one player remains
                if (alivePlayers.Count <= 1)
                    break;
            }
            if (alivePlayers.Count <= 1)
                break;
            if (turnIndex == maxTurn - 1)
            {
                isTimedOut = true;
                Debug.Log("Timed Out !");
            }
        }

        if (!isTimedOut)
            foreach (var item in alivePlayers)
            {
                // Debug.Log($"Player {item.PlayerIndex} won !");
                item.OnWin();
            }
    }

    private void AttackPlayer(int playerIndex, Player attackedPlayer)
    {
        foreach (var player in alivePlayers)
        {
            foreach (var card in player.Board)
            {
                card.ResetCurrentDef();
            }
        }

        Player attackingPlayer = alivePlayers[playerIndex];

        //For every attacking card
        for (int attackingCardIndex = 0; attackingCardIndex < attackingPlayer.Board.Count; attackingCardIndex++)
        {
            bool hasCardAttacked = false;
            Card attackingCard = attackingPlayer.Board[attackingCardIndex];
            attackingCard.ResetCurrentDef();
            //Check for enemy card in board with provocation
            for (int enemyCardIndex = 0; enemyCardIndex < attackedPlayer.Board.Count; enemyCardIndex++)
            {
                Card enemyCard = attackedPlayer.Board[enemyCardIndex];
                //attack provoking enemy
                if ((enemyCard.HasProvocation && !attackingCard.HasDistortion) || (attackingCard.HasDistortion && enemyCard.HasDistortion && enemyCard.HasProvocation))
                {
                    hasCardAttacked = true;
                    int bufferRemainingEnemyDef = enemyCard.CurrentDef;

                    int enemyCardDefBuffer = enemyCard.CurrentDef;
                    int attackingCardDefBuffer = attackingCard.CurrentDef;

                    if ((attackingCard.HasFirstStrike) ||
    (enemyCard.HasFirstStrike || attackingCardDefBuffer - enemyCard.Attack > 0))
                        enemyCard.OnCardAttacked(attackingCard.Attack);

                    //ENNEMY CARD ATTACKS ATTACKING CARD
                    if ((enemyCard.HasFirstStrike) ||
                        (attackingCard.HasFirstStrike || enemyCardDefBuffer - attackingCard.Attack > 0))
                        attackingCard.OnCardAttacked(enemyCard.Attack);

                    if (enemyCard.CurrentDef <= 0)
                    {
                        attackedPlayer.KillCardOnBoard(enemyCard);
                        enemyCardIndex--;
                        if (attackingCard.HasTrample)
                            attackedPlayer.AttackPlayer(attackingCard.Attack - bufferRemainingEnemyDef);
                    }

                    if (attackingCard.CurrentDef <= 0)
                    {
                        attackingPlayer.KillCardOnBoard(attackingCard);
                        attackingCardIndex--;
                    }
                    break;
                }
            }

            //If we got here, no enemy card had provocation
            if (!hasCardAttacked)
                attackedPlayer.AttackPlayer(attackingCard.Attack);
        }
    }

    [Button]
    void SequentialPlayerOptimization()
    {
        StartCoroutine(SequentialTraining());
    }

    [Button]
    void FullSequence()
    {
        StartCoroutine(FullSequenceRoutine());
    }

    IEnumerator FullSequenceRoutine()
    {
        GeneratePlayers();

        //train player 0, that player 1, and do it 10 times
        //this is about 10 000 000 games
        yield return StartCoroutine(SequentialTraining());

        //compute an optimized setList using cards that enrered more than they were thrown out of the deck
        SetListHandler.ComputeOptimizedSetList();
        GivePlayerOptimizedSetLists();

        //Train player 0 against the deck it used last game for about 1 000 000 games using only the optimized Set List
        yield return StartCoroutine(TrainOnSameDeck());
        yield return StartCoroutine(PlayWithRandomDeck(hasRandomDeckEachBatch: true));
  //      yield return StartCoroutine(PlayWithRandomDeck(hasRandomDeckEachBatch: false));

        output = "\n\n\n" + output;
        //Debug & data printing
        foreach (var item in players[0].FullDeck)
        {
            output = item.ToString() + output;
        }
        output = "Name;i;ATK;DEF;Cost;Value;Powers\n" + output;

        output = "\n\n\n" + output;

        //Debug & data printing
        foreach (var item in SetListHandler.OptimizedSetList)
        {
            output = item.ToString() + output;
        }
        output = "Name;i;ATK;DEF;Cost;Value;Powers\n" + output;

        PrintTOCSV();
    }

    [Button]
    void DebugSequence()
    {
        StartCoroutine(DebugSequenceRoutine());
    }

    IEnumerator DebugSequenceRoutine()
    {
        GeneratePlayers();

        yield return StartCoroutine(PlayWithRandomDeck());

        output = "\n\n\n" + output;
        //Debug & data printing
        foreach (var item in players[0].FullDeck)
        {
            output = item.ToString() + output;
        }
        output = "Name;i;ATK;DEF;Cost;Value;Powers\n" + output;

        output = "\n\n\n" + output;

        //Debug & data printing
        foreach (var item in SetListHandler.OptimizedSetList)
        {
            output = item.ToString() + output;
        }
        output = "Name;i;ATK;DEF;Cost;Value;Powers\n" + output;

        PrintTOCSV();
    }

    private IEnumerator TrainOnSameDeck()
    {
        int maxI = 3000;
        int gameBatchAmount = 1000;
        List<Card> player1ReferenceDeck = players[0].FullDeck;
        for (int i = 0; i < maxI; i++)
        {
            float t = Time.realtimeSinceStartup;

            players[1].FullDeck = player1ReferenceDeck;
            player1ReferenceDeck = players[0].FullDeck;
            PlayGameBatch(gameBatchAmount, 0, true, true);
            //Debug.Log($"Batch {i} played");
            yield return null;

            Debug.Log(i + " : Trained Player 1 on its deck. Executed in " + (Time.realtimeSinceStartup - t) + " s");
        }
    }

    private IEnumerator PlayWithRandomDeck(bool hasRandomDeckEachBatch = true)
    {
        int maxI = 100;
        int gameBatchAmount = 100;

        players[1].GetRandomDeckFromSetList(false);

        output += "\n\n\n";
        for (int i = 0; i < maxI; i++)
        {
            float t = Time.realtimeSinceStartup;

            if (hasRandomDeckEachBatch)
                players[1].GetRandomDeckFromSetList(false);

            PlayGameBatch(gameBatchAmount, 0, true, false);
            output += players[0].CurrentWinRate + "\n";
            //Debug.Log($"Batch {i} played");
            yield return null;

            Debug.Log(i + $" : Played against random deck and won {players[0].CurrentWinRate} times. Executed in " + (Time.realtimeSinceStartup - t) + " s");
        }
    }


    private void GivePlayerOptimizedSetLists()
    {
        foreach (var player in players)
        {
            player.GetRandomDeckFromSetList();
        }
    }

    IEnumerator SequentialTraining()
    {
        int trainingPerPlayer = 70;
        int cardsToChangePotentialPerTrining = 500;
        int gameBatchAmount = 700;
        for (int i = 0; i < trainingPerPlayer; i++)
        {
            float t = Time.realtimeSinceStartup;
            for (int j = 0; j < cardsToChangePotentialPerTrining; j++)
            {
                PlayGameBatch(gameBatchAmount, i % 2, false, true);
                //Debug.Log($"Batch {i} played");
                yield return null;
            }

            Debug.Log(i + " : Optimized Player " + i % 2 + ". Executed in " + (Time.realtimeSinceStartup - t) + " s");
        }
    }

    int GetBoardAttackValue(List<Card> board)
    {
        int attackValue = 0;

        foreach (Card card in board)
        {
            attackValue += card.Attack;
        }

        return attackValue;
    }

    void PlayGameBatch(int gameAmount, int optimizedPlayerIndex, bool trainOnOptimizedList = false, bool trainDeck = false)
    {
        players[optimizedPlayerIndex].ResetWinRate();

        //float startExecutionTime = Time.realtimeSinceStartup;

        for (int i = 0; i < gameAmount / 2; i++)
        {
            PlayOneGame();
        }

        players.Reverse();

        for (int i = 0; i < gameAmount / 2; i++)
        {
            PlayOneGame();
        }
        players.Reverse();

        float averageTurnsThisRound = (float)totalTurnThisRound / gameAmount;

        // output += players[0].CurrentWinRate.ToString() + ";" + averageTurnsThisRound + "\n";
        if (trainDeck)
            players[optimizedPlayerIndex].ApplyRandom(trainOnOptimizedList);
        totalTurnThisRound = 0;

        //Debug.Log("Executed in " + (Time.realtimeSinceStartup - startExecutionTime) * 1000 + "ms");
    }

    [Button]
    void PrintTOCSV()
    {
        output = "\n\n\n" + output;
        for (int i = SetListHandler.cardMaxCost; i > 0; i--)
        {

            output = $"{i.ToString()};{players[0].FullDeckAtStart.Where(c => c.Cost == i).Count()};{players[0].CompleteReferenceDeck.Where(c => c.Cost == i).Count()}"
                + "\n" + output;
        }

        output = "Cost;Start Deck Amount;End Deck Amount\n" + output;

        TextWriter tw = new StreamWriter(Application.dataPath + "/GAME_LOG_" + DateTime.Now.Year.ToString() + "_"
            + DateTime.Now.Month.ToString() + "_"
            + DateTime.Now.Day.ToString() + "_"
            + DateTime.Now.Hour.ToString() + "_"
            + DateTime.Now.Minute.ToString()
            + ".csv", false);

        tw.Write(output);
        tw.Close();
    }

    [Button]
    void ExportPlayer0DeckAsJSON()
    {
        string str = "{\n\t\"Cards\":[\n";

        foreach (var card in players[0].CompleteReferenceDeck)
        {
            str += "\t\t{\n"
                + "\t\t\t\"Cost\":" + card.Cost + "," + "\n"
                + "\t\t\t\"Attack\":" + card.Attack + "," + "\n"
                + "\t\t\t\"Defense\":" + card.Defense + "," + "\n"
                + "\t\t\t\"HasTaunt\":" + card.HasProvocation.ToString().ToLower() + "," + "\n"
                + "\t\t\t\"HasTrample\":" + card.HasTrample.ToString().ToLower() + "," + "\n"
                + "\t\t\t\"HasDistortion\":" + card.HasDistortion.ToString().ToLower() + "," + "\n"
                + "\t\t\t\"HasFirstStrike\":" + card.HasFirstStrike.ToString().ToLower() + "\n"
                + "\t\t},\n";
        }

        int lastCommaIndex = str.LastIndexOf(',');

        if (lastCommaIndex >= 0)
        {
            string stringWithoutLastComma = str.Substring(0, lastCommaIndex) + str.Substring(lastCommaIndex + 1);
            Debug.Log(stringWithoutLastComma);
        }

        str += "\t]\n}";
        File.WriteAllText(Application.dataPath + "/Decks/Deck_Veuch.json", str);
    }
}
