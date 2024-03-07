using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Country
{
    public Button Pointer; //reference to the pointer
    public List<Country> Neighbors;
    public Player Owner = null; // may not reflect the button's color
    public int Troops = 0;

    public Country(Button button)
    {
        this.Pointer = button;
    }

    public void SetNeighbors(List<Country> list)
    {
        if (Neighbors != null) return;
        Neighbors = list;
    }

    public void SetOwner(Player player)
    {
        this.Owner = player;
        Pointer.GetComponent<Image>().color = player.Color;
    }

    public Player GetOwner() => this.Owner;

    public Color GetColor() => Owner.Color;

    public int GetTroops() => this.Troops;

    public void SetTroops(int newTroops)
    {
        this.Troops = newTroops;
        this.Pointer.GetComponentInChildren<TextMeshProUGUI>().text = $"{Troops}";
    }

    public void IncreaseTroops(int increment)
    {
        this.Troops += increment;
        this.Pointer.GetComponentInChildren<TextMeshProUGUI>().text = $"{Troops}";
    }

    // this is when a country is taken by order player
    // public void change_country_color(Color color)
    // {
    //     this.color = color;
    //     pointer.GetComponent<Image>().color = color;
    // }

    // this is for changing button color for Highlighting either to black or white
    private void TempColorChange(Color color)
    {
        Pointer.GetComponent<Image>().color = color;
    }


    // this is to undo the Highlighting so change to the Owner color from either black or white
    public void ReverseColorChange()
    {
        Pointer.GetComponent<Image>().color = this.Owner.Color;
    }

    // Highlights the color to grey 
    // also Highlights the Attackable countries which are neighboring countries that do not have the same color as itself
    // returns this Attackable countries in a list for the gameobject instance to handle states
    public List<Country> Highlight()
    {
        TempColorChange(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in Neighbors)
        {
            if (neighbor.Owner == this.Owner) continue;

            neighbor.TempColorChange(Color.white);
            output.Add(neighbor);
        }
        return output;
    }
}