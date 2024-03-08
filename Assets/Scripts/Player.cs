using System.Collections.Generic;
using UnityEngine;

public enum Troop
{
    Infantry,
    Tanks,
}

public class Player
{
    Color color;
    int numberOfTroops;

    List<Country> ownedCountries;

    public Player(Color color)
    {
        switch (Map1.PlayerCount)
        {
            case 2: this.numberOfTroops = 40; break;
            case 3: this.numberOfTroops = 35; break;
            case 4: this.numberOfTroops = 30; break;
            case 5: this.numberOfTroops = 25; break;
            case 6: this.numberOfTroops = 20; break;
        }

        this.color = color;
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

    public Color GetColor() => color;

    public int GetNumberOfTroops() => numberOfTroops;

    // use getnumberofownedcountries() for the distribution phase
    public void ChangeNumberOfTroops(int difference)
    {
        numberOfTroops += difference;
    }

    public void SetNumberOfTroops(int troops)
    {
        numberOfTroops = troops;
    }

    public int GetNumberOfOwnedCountries() => ownedCountries.Count;
}