using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    int index;
    int atk;
    int def;

    public int Index => index;
    public int ATK => atk;
    public int DEF => def;
    public int Cost => Mathf.CeilToInt((float)(atk+def)/2);

    public Card(int atk, int def, int index)
    {
        this.index = index;
        this.atk = atk;
        this.def = def;
    }

    public override string ToString()
    {
        return$"i : {index} ATK : {ATK} DEF : {DEF}";
    }
}
