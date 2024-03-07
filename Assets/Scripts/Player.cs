using System.Collections.Generic;
using UnityEngine;

public enum Troop
{
    Infantry,
    Tanks,
}

public class Player
{
    public Color Color;
    public int NumberOfTroops;

    List<Country> ownedCountries;

    public Player(Color color)
    {
        switch (Map1.PlayerCount)
        {
            case 2: NumberOfTroops = 40; break;
            case 3: NumberOfTroops = 35; break;
            case 4: NumberOfTroops = 30; break;
            case 5: NumberOfTroops = 25; break;
            case 6: NumberOfTroops = 20; break;
        }

        Color = color;
        ownedCountries = new List<Country>();
    }

    public void AddCountry(Country country)
    {
        ownedCountries.Add(country);
    }

    public void RemoveCountry(Country country)
    {
        ownedCountries.Remove(country);
    }

    public int GetOwnedCountries() => ownedCountries.Count;
}