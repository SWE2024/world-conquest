using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool gain_card = false;
    public List<Card> trade = new List<Card>();
    public List<Card> slot = new List<Card>();

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
        if (gain_card)
        {
            if (this is not AIPlayer) GameObject.Find("CardNotification").GetComponent<Image>().enabled = true;

            int index = UnityEngine.Random.Range(0, GameController.ListOfCards.Count);
            Card card = GameController.ListOfCards[index];
            GameController.ListOfCards.RemoveAt(index);
            this.ownedCards.Add(card);
        }
        gain_card = false;
    }

    /// <summary>
    /// <c>FillCards</c> gives 9 random cards to the player.
    /// </summary>
    public void FillCards()
    {
        for (int i = 0; i < 9; i++)
        {
            int index = UnityEngine.Random.Range(0, GameController.ListOfCards.Count);
            Card card = GameController.ListOfCards[index];
            GameController.ListOfCards.RemoveAt(index);
            this.ownedCards.Add(card);
        }
    }

    /// <summary>
    /// <c>InitializeSlot</c> clears current cards in the slow and then fills all slots with owned cards.
    /// </summary>
    public void InitializeSlot()
    {
        slot.Clear();
        trade.Clear();
        foreach (Card card in ownedCards) slot.Add(card);
    }

    /// <summary>
    /// <c>LoadSlot</c> physically fills the slots in game with the cards that the player owns.
    /// </summary>
    public void LoadSlot()
    {
        for (int i = 1; i < 7; i++)
        {
            Button slot = GameObject.Find($"slot{i}").GetComponent<Button>();
            slot.enabled = false;
            slot.GetComponent<Image>().enabled = false;
            slot.GetComponent<Image>().sprite = null;
        }

        if (slot.Count > 6)
        {
            GameObject.Find("NextCard").GetComponent<Button>().enabled = true;
            GameObject.Find("NextCard").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("NextCard").GetComponent<Button>().enabled = false;
            GameObject.Find("NextCard").GetComponent<Image>().enabled = false;
        }

        for (int i = 0; i < Math.Min(slot.Count, 6); i++)
        {
            Button slot_button = GameObject.Find($"slot{i + 1}").GetComponent<Button>();
            slot_button.GetComponent<Image>().sprite = slot[i].GetSprite();
            slot_button.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            slot_button.GetComponent<Image>().enabled = true;
            slot_button.enabled = true;
        }
    }

    /// <summary>
    /// <c>LoadTrade</c> finds 3 cards that are eligible for trade in.
    /// </summary>
    public void LoadTrade()
    {
        for (int i = 0; i < 3; i++)
        {
            Button trade = GameObject.Find($"trade{i + 1}").GetComponent<Button>();
            trade.enabled = false;
            trade.GetComponent<Image>().enabled = false;
            trade.GetComponent<Image>().sprite = null;
        }

        for (int i = 0; i < Math.Min(3, trade.Count); i++)
        {
            Button trade_slot = GameObject.Find($"trade{i + 1}").GetComponent<Button>();
            trade_slot.GetComponent<Image>().sprite = trade[i].GetSprite();
            trade_slot.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            trade_slot.GetComponent<Image>().enabled = true;
            trade_slot.enabled = true;
        }
    }

    /// <summary>
    /// <c>SelectForTrade</c> adds a card from the deck to the trade.
    /// </summary>
    /// <param name="slot_name">The name of the slot that you click .</param>
    public void SelectForTrade(string slot_name)
    {
        int index = int.Parse(slot_name[4].ToString()) - 1;
        Card card = slot[index];
        trade.Add(card);
        slot.RemoveAt(index);
        LoadSlot();
        LoadTrade();
    }

    /// <summary>
    /// <c>RemoveForTrade</c> removes a card from the trade section.
    /// </summary>
    /// <param name="slot_name">The name of the slot that you click .</param>
    public void RemoveForTrade(string slot_name)
    {
        int index = int.Parse(slot_name[5].ToString()) - 1;
        Card card = trade[index];
        slot.Add(card);
        trade.RemoveAt(index);
        LoadSlot();
        LoadTrade();
    }

    /// <summary>
    /// <c>Next</c> gets the first card in the slot and sends it to the back.
    /// </summary>
    public void Next()
    {
        Card card = slot[0];
        slot.RemoveAt(0);
        slot.Add(card);
        LoadSlot();
    }

    /// <summary>
    /// <c>Cancel</c> removes all cards from the trade deck, and back into the slot.
    /// </summary>
    public void Cancel()
    {
        foreach (Card card in trade) slot.Add(card);
        trade.Clear();
        LoadTrade();
    }

    /// <summary>
    /// <c>Trade</c> gets the 3 cards in the trade deck and trades them in for troops.
    /// </summary>
    public bool Trade()
    {
        if (trade.Count < 3) return false; // stops the player from being able to trade in less than 3 cards

        HashSet<string> types = new HashSet<string>();
        foreach (Card card in trade) types.Add(card.GetCardType());
        foreach (string s in types) Debug.Log(s);

        if (types.Count != 3 && types.Count != 1)
        {
            foreach (Card card in trade) slot.Add(card);
            trade.Clear();
            LoadTrade();
            LoadSlot();
            return false;
        }
        ChangeNumberOfTroops(6);
        Killfeed.Update($"{this.GetName()}: traded in for 6 troops");
        foreach (Card card in trade) ownedCards.Remove(card);
        trade.Clear();
        LoadTrade();
        LoadSlot();
        return true;
    }

    virtual public void TakeTurn() { } // only implemented in AIPlayer.cs
}