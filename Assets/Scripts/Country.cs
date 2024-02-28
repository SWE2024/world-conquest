using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Country
{
    //reference to the pointer
    public Button pointer = null;

    // the country's neighbors
    public List<Country> neighbors = null;

    // country's current game logic color
    // might not reflect the button's color, because of highlighting
    public Color color;

    // number of troops
    public int troops = 0;

    public Country(Button button, Color color)
    {
        this.pointer = button;
        this.color = color;
    }

    public void set_neighbors(List<Country> list)
    {
        if (neighbors != null) return;
        neighbors = list;
    }

    public Color get_color() { return color; }

    public int get_troops() { return troops; }

    public void set_troops(int new_troops)
    {
        troops += new_troops;
        pointer.GetComponentInChildren<TextMeshProUGUI>().text = $"{troops}";
    }

    // this is when a country is taken by order player
    public void change_country_color(Color color)
    {
        this.color = color;
        pointer.GetComponent<Image>().color = color;
    }

    // this is for changing button color for highlighting either to black or white
    private void button_temp_color_change(Color color)
    {
        pointer.GetComponent<Image>().color = color;
    }


    // this is to undo the highlighting so change to the owner color from either black or white
    public void reverse_color_change()
    {
        pointer.GetComponent<Image>().color = color;
    }

    // highlights the color to grey 
    // also highlights the attackable countries which are neighboring countries that do not have the same color as itself
    // returns this attackable countries in a list for the gameobject instance to handle states
    public List<Country> highlight()
    {
        button_temp_color_change(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in neighbors)
        {
            if (neighbor.color == color) continue;

            neighbor.button_temp_color_change(Color.white);
            output.Add(neighbor);
        }
        return output;
    }
}