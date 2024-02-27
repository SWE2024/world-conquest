
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;


public enum Country_State {
    none,
    highlighted,
    considered,
}
public class GameState 
{
    List<Country> list_of_countries;
    public static System.Random random = new System.Random();
    Country highlighted = null;
    List<Country> considered = null;
    UnityEngine.Color turn_color;
    SpriteRenderer square;
    List<UnityEngine.Color> turns_order;
    int turn_index = 0;

    public GameState(List<Country> list) {
        this.list_of_countries = list;
        this.turns_order = GameState.create_turns();        
        this.turn_color = this.turns_order[0];
        this.square = GameObject.Find("Square").GetComponent<SpriteRenderer>();
        this.square.color = turn_color;
    }

    private static List<UnityEngine.Color> create_turns() {
        List<int> list = new List<int> {0 , 1, 2};

        List<int> randomized = new List<int>();

        while (list.Count > 0) {
            int index = GameState.random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<UnityEngine.Color> output = new List<UnityEngine.Color>();

        foreach (int index in randomized) {
            output.Add(GameState.int_to_color(index));
        }

        return output;
    }




    public static UnityEngine.Color int_to_color(int num) 
    {
        switch (num) 
        {
            case 0:
                return UnityEngine.Color.green;
            case 1:
                return UnityEngine.Color.blue;
            case 2:
                return UnityEngine.Color.red;
            default:
                return UnityEngine.Color.white;
        }
    }



    public void take_country_click(Country country) {
        //this handles highlighting (if nothing is highlighted)
        if (this.highlighted == null) {
            // this handles the case that the clicked country's owner doesn't much turn's player
            if (this.turn_color != country.color) return;
            this.highlight(country);
            return;
        }  

        //from here onwards is when smth is highlighted

        //check if the country being clicked is attackable, positive index is true, else is false
        int index = this.considered.IndexOf(country);

        if (index < 0) {
            this.unhighlight();
            return;
        }


        this.attack(index);

        this.next_turn();

        return;
    }

    public void highlight(Country country) {
        this.highlighted = country;
        this.considered = country.highlight();
        return;
    }

    public void unhighlight() {
        this.highlighted.reverse_color_change();

        foreach (Country country in this.considered) {
            country.reverse_color_change();
        }

        this.highlighted = null;
        this.considered = null;
    }

    public void attack(int index) {
        Country attacked = this.considered[index];
        //remove the attacked country from considered 
        this.considered.RemoveAt(index);
        
        //change the attacked country color
        attacked.change_country_color(this.turn_color);

        //unhighlight the rest
        this.unhighlight();
    }

    public void next_turn() {

        this.turn_index++;

        if (this.turn_index > (this.turns_order.Count - 1)) {
            this.turn_index = 0;
        }

        Debug.Log($"the turn now is {this.turn_index}");

        this.turn_color = this.turns_order[this.turn_index];

        this.square.GetComponent<SpriteRenderer>().color = this.turn_color;
    }




}






public class Country
{
    public Button pointer = null;
    public List<Country> neighbors = null;
    public UnityEngine.Color color;
    public Country_State state = Country_State.none;
    public int troops = 0;
    public Country(Button button, UnityEngine.Color color) {
        this.pointer = button;
        this.color = color;
    }
    public void set_neighbors(List<Country> list) {
        if (neighbors != null) return;
        this.neighbors = list;
    }

    public void change_country_color(UnityEngine.Color color) {
        this.color = color;
        this.pointer.GetComponent<Image>().color = color;
    }

    private void button_temp_color_change(UnityEngine.Color color) {
        this.pointer.GetComponent<Image>().color = color;
    }

    public void reverse_color_change() {
        this.pointer.GetComponent<Image>().color = this.color;
    }

    public void country_clicked() {
        switch(this.state) {
            case Country_State.none:
                this.highlight();
                return ;
            case Country_State.highlighted:
                this.unhighlight();
                return ;
            default:
                return ;
        }
    }

    public List<Country> highlight() {
        this.button_temp_color_change(UnityEngine.Color.black);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in this.neighbors ) {
            if (neighbor.color == this.color) continue;

            neighbor.button_temp_color_change(UnityEngine.Color.white);
            output.Add(neighbor);
        }
        return output;
    }

    public void unhighlight() {
        this.reverse_color_change();
        this.state = Country_State.none;

        foreach (Country neighbor in this.neighbors ) {
            neighbor.reverse_color_change();
            neighbor.state = Country_State.none;
        }
    }


}