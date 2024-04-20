using UnityEngine;

public class Card
{
    const float PROVOCATION_COST = 1.5f;
    const float STOMP_COST = 1f;
    const float DISTORTION_COST = 1f;
    const float FIRST_STRIKE_COST = 1f;

    int index;
    int atk;
    int def;
    int currentDef;
    int valueIndex;
    bool hasProvocation = false;
    bool hasTrample = false;
    bool hasDistortion = false;
    bool hasFirstStrike = false;

    public int Index => index;
    public int Attack => atk;
    public int Defense => def;
    public int CurrentDef => currentDef;
    public int ValueIndex => valueIndex;
    public bool HasProvocation => hasProvocation;
    public bool HasTrample => hasTrample;
    public bool HasDistortion => hasDistortion;
    public bool HasFirstStrike => hasFirstStrike;

    public int Cost => Mathf.CeilToInt(((atk + def) / 2.0f)
        + (HasProvocation ? PROVOCATION_COST : 0)
        + (hasTrample ? STOMP_COST : 0)
        + (hasDistortion ? DISTORTION_COST : 0)
        + (hasFirstStrike ? FIRST_STRIKE_COST : 0));

    public void AddToValueIndex(int v)
    {
        valueIndex += v;
    }

    public Card(int atk, int def, int index, bool hasProvocation, bool hasStomp, bool hasDistortion, bool hasFirstStrike)
    {
        this.index = index;
        this.atk = atk;
        this.def = def;
        this.hasProvocation = hasProvocation;
        this.hasTrample = hasStomp;
        this.hasDistortion = hasDistortion;
        this.hasFirstStrike = hasFirstStrike;
        currentDef = def;
    }

    public Card(Card card)
    {
        index = card.index;
        atk = card.atk;
        def = card.def;
        currentDef = card.def;
        hasProvocation = card.hasProvocation;
        hasTrample = card.hasTrample;
        hasDistortion = card.hasDistortion;
        hasFirstStrike = card.hasFirstStrike;
    }

    public void ResetCurrentDef()
    {
        currentDef = def;
    }

    public override string ToString()
    {
        return $"{CardNamer.GetCardName(this)};{index};{Attack};{Defense};{Cost};{valueIndex};{(hasProvocation ? "1" : "0")};{(hasTrample ? "1" : "0")};{(hasDistortion ? "1" : "0")};{(hasFirstStrike ? "1" : "0")}\n";
    }

    public void OnCardAttacked(int atk)
    {
        currentDef -= atk ;
    }
}
