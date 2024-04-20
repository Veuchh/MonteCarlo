using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardNamer
{
    public static string GetCardName(Card card)
    {
        string cardName = "";

        switch (card.Cost)
        {
            case 1:
                cardName += "Stagiaire";
                break;
            case 2:
                cardName += "Poussin";
                break;
            case 3:
                cardName += "Chaton";
                break;
            case 4:
                cardName += "Goblin";
                break;
            case 5:
                cardName += "Prog";
                break;
            case 6:
                cardName += "Hectolion";
                break;
            case 7:
                cardName += "Dragon";
                break;
            case 8:
                cardName += "Axel";
                break;
            default:
                cardName += "Entité inconnue";
                break;
        }

        cardName += " ";

        switch (card.Attack)
        {
            case int a when (a == 0):
                cardName += "Pacifiste";
                break;

            case int a when (a <= 3 && a >= 1):
                cardName += "Inoffensif";
                break;

            case int a when (a <= 6 && a >= 4):
                cardName += "Motivé";
                break;

            case int a when (a <= 9 && a >= 7):
                cardName += "Hargneux";
                break;

            case int a when (a <= 12 && a >= 10):
                cardName += "Violent";
                break;

            case int a when (a <= 15 && a >= 13):
                cardName += "Génocidaire";
                break;

            default:
                cardName += "Etrange";
                break;
        }

        if (card.HasProvocation)
            cardName += " GD";
        if (card.HasDistortion)
            cardName += " Parlementaire";
        if (card.HasFirstStrike)
            cardName += " Fourbe";
        if (card.HasTrample)
            cardName += " Géant";

        cardName += " ";

        switch (card.Defense)
        {
            case int a when (a == 1):
                cardName += "en Mousse";
                break;

            case int a when (a <= 4 && a >= 2):
                cardName += "en Carton";
                break;

            case int a when (a <= 7 && a >= 5):
                cardName += "en Plastique";
                break;

            case int a when (a <= 10 && a >= 8):
                cardName += "en Terre Cuite";
                break;

            case int a when (a <= 13 && a >= 11):
                cardName += "en Chêne";
                break;

            case int a when (a <= 16 && a >= 14):
                cardName += "de Métal";
                break;

            default:
                cardName += "Intangible";
                break;
        }

        return cardName;
    }
}
