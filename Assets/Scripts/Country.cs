using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Country
{
    public Button pointer = null;
    public List<Country> neighbors = null;
    public Color color;
    public Country_State state = Country_State.none;
    public int troops = 0;
    public Country(Button button, Color color)
    {
        this.pointer = button;
        this.color = color;
    }
    public void set_neighbors(List<Country> list)
    {
        if (neighbors != null) return;
        this.neighbors = list;
    }

    public void change_country_color(Color color)
    {
        this.color = color;
        this.pointer.GetComponent<Image>().color = color;
    }

    private void button_temp_color_change(Color color)
    {
        this.pointer.GetComponent<Image>().color = color;
    }

    public void reverse_color_change()
    {
        this.pointer.GetComponent<Image>().color = this.color;
    }

    public void country_clicked()
    {
        switch (this.state)
        {
            case Country_State.none:
                this.highlight();
                return;
            case Country_State.highlighted:
                this.unhighlight();
                return;
            default:
                return;
        }
    }

    public List<Country> highlight()
    {
        this.button_temp_color_change(Color.black);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in this.neighbors)
        {
            if (neighbor.color == this.color) continue;

            neighbor.button_temp_color_change(Color.white);
            output.Add(neighbor);
        }
        return output;
    }

    public void unhighlight()
    {
        this.reverse_color_change();
        this.state = Country_State.none;

        foreach (Country neighbor in this.neighbors)
        {
            neighbor.reverse_color_change();
            neighbor.state = Country_State.none;
        }
    }


}