using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playersAmount = 2;
    [SerializeField] System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();
    List<Player> alivePlayers = new List<Player>();
    [SerializeField] List<int> win = new List<int>();

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

        while (alivePlayers.Count > 1)
        {
            //Play each player turn
            for (int playerIndex = alivePlayers.Count - 1; playerIndex >= 0; playerIndex--)
            {
                alivePlayers[playerIndex].OnNewTurn();
                //play all the cards you can
                alivePlayers[playerIndex].PlayAllPossibleCards();

                //attack the player to your left
                int attackValue = GetBoardAttackValue(alivePlayers[playerIndex].Board);

                Player attackedPlayer = alivePlayers[
                    (playerIndex - 1) < 0
                    ? alivePlayers.Count - 1
                    : playerIndex - 1];

                //Debug.Log($"Player {alivePlayers[playerIndex].PlayerIndex} attacks Player {attackedPlayer.PlayerIndex} for {attackValue} !");

                attackedPlayer.AttackPlayer(attackValue);

                if (attackedPlayer.RemainingHP <= 0)
                {
                    alivePlayers.Remove(attackedPlayer);
                    playerIndex--;
                }

                //check if only one player remains
                if (alivePlayers.Count <= 1)
                    break;
            }
        }

        foreach (var item in alivePlayers)
        {
            win[item.PlayerIndex]++;
           // Debug.Log($"Player {item.PlayerIndex} won !");
            item.OnWin();
        }
    }

    [Button]
    void Play1000x1000Games()
    {
       StartCoroutine(PlayAllGames());  
    }

    IEnumerator PlayAllGames()
    {
        for (int i = 0; i < 1000; i++)
        {
            PlayGames(1000);
            Debug.Log($"Batch {i} played");
            yield return null;
        }
        players[0].PrintAllDeck();

        TextWriter tw = new StreamWriter(Application.dataPath + "/GAME_LOG_" + DateTime.Now.Year.ToString() + "_"
            + DateTime.Now.Month.ToString() + "_"
            + DateTime.Now.Day.ToString() + "_"
            + DateTime.Now.Hour.ToString() + "_"
            + DateTime.Now.Minute.ToString()
            + ".csv", false);

        tw.Write(output);
        tw.Close();
    }

    void Play1000Games()
    {
        PlayGames(1000);
    }

    int GetBoardAttackValue(List<Card> board)
    {
        int attackValue = 0;

        foreach (Card card in board)
        {
            attackValue += card.ATK;
        }

        return attackValue;
    }

    string output = "";

    void PlayGames(int gameAmount)
    {
        float startExecutionTime = Time.realtimeSinceStartup;
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

        output += players[0].WonRound.ToString() + "\n";

        players[0].ApplyRandom();

        //Debug.Log("Executed in " + (Time.realtimeSinceStartup - startExecutionTime) * 1000 + "ms");
    }
}
