using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Player</c> holds all relevant methods for a local player.
/// </summary>
public class Player
{
    string name;
    Color color;
    int numberOfTroops;

    List<Country> ownedCountries;
    public List<Card> ownedCards;

    public Player(string name, Color color)
    {
        switch (Preferences.PlayerCount + Preferences.AgentCount)
        {
            case 2: this.numberOfTroops = 40; break;
            case 3: this.numberOfTroops = 35; break;
            case 4: this.numberOfTroops = 30; break;
            case 5: this.numberOfTroops = 25; break;
            case 6: this.numberOfTroops = 20; break;
        }

        this.name = name;
        this.color = color;
        ownedCountries = new List<Country>();
        ownedCards = new List<Card>();
    }

    /// <summary>
    /// <c>AddCountry</c> adds a newly owned country to the <c>ownedCountries</c> list.
    /// </summary>
    public void AddCountry(Country country)
    {
        ownedCountries.Add(country);
    }

    /// <summary>
    /// <c>RemoveCountry</c> removes a country from the <c>ownedCountries</c> list if it is not longer owned.
    /// </summary>
    public void RemoveCountry(Country country)
    {
        ownedCountries.Remove(country);
    }

    /// <summary>
    /// <c>AddCard</c> adds a newly owned card to the <c>ownedCards</c> list.
    /// </summary>
    public void AddCard(Card card)
    {
        ownedCards.Add(card);
    }

    /// <summary>
    /// <c>RemoveCard</c> removes a card from the <c>ownedCards</c> list if it is not longer owned.
    /// </summary>
    public void RemoveCard(Card card)
    {
        ownedCards.Remove(card);
    }

    public List<Country> GetCountries() => ownedCountries;

    public int GetNumberOfOwnedCountries() => ownedCountries.Count;

    public string GetName() => name;

    public Color GetColor() => color;

    public int GetNumberOfTroops() => numberOfTroops;

    /// <summary>
    /// <c>ChangeNumberOfTroops</c> removes or adds new troops. Use a negative <c>difference</c> to remove troops.
    /// </summary>
    public void ChangeNumberOfTroops(int difference)
    {
        numberOfTroops += difference;
    }

    /// <summary>
    /// <c>GetNewTroopsAndCards</c> used at the beginning of the draft phase to calculate how many troops the player should have.
    /// </summary>
    public void GetNewTroopsAndCards()
    {
        this.numberOfTroops = Math.Max(this.ownedCountries.Count / 3, 3); // you need to receive at least 3 armies
    }

    virtual public void TakeTurn() { } // only implemented in AIPlayer.cs
}