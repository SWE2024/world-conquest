
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum Country_State {
    none,
    highlighted,
    considered,
}
public class GameState 
{   
    //holds the reference to the singleton instance
    private static GameState instance = null;

    // list of countries
    public List<Country> list_of_countries = new List<Country>();

    // random generator
    public static System.Random random = new System.Random();

    // represent a state, if it holds a country that country is highlighted
    // if not highlighted, holds null
    Country highlighted = null;

    // represent the same state as above, if it holds a list of country a country is highlighted (can be empty but still means highlighted)
    // if not highlighted, holds null
    // country held in the list are the attackable countries when a country is selected. so not all neighboring countries will be here
    // only the attckable neighboring country
    List<Country> considered = null;

    //turn's color
    UnityEngine.Color turn_color;

    // the square sprite that shows the color of the turn
    SpriteRenderer square;

    // this holds the order of turn represented by color
    public List<UnityEngine.Color> turns_order;

    // it's the index to the turns order to know aht is next
    int turn_index = 0;

    //player count
    int playerCount;

    

    private GameState(int playerCount) {
        // this.list_of_countries = list;
        this.playerCount = playerCount;

        // creates the turns order here
        this.turns_order = GameState.create_turns(this.playerCount);       

        // set the first turn color
        this.turn_color = this.turns_order[0];
        this.square = GameObject.Find("CurrentColour").GetComponent<SpriteRenderer>();
        this.square.color = turn_color;
    }

    
    //singleton's constructor method access thru here
    public static GameState New(int playerCount) {
        if (instance != null) return GameState.instance;
        GameState.instance = new GameState(playerCount);
        return GameState.instance;
    }

    // this generates a of colors that represents the distributed number of countries, 
    // if 5 reds are here red holds five countries
    public List<Color> generate_list_of_colors() {
        int num_of_countries = 44 / this.playerCount;
        int remainder = 44 % this.playerCount;

        List<Color> list_of_colors = new List<Color>();
        List<Color> copy_turns = new List<Color>();

        foreach (Color color in this.turns_order) {
            copy_turns.Add(color);
            for (int i = 0; i < num_of_countries; i ++) list_of_colors.Add(color);
        }


        for (int i = 0; i < remainder; i++) {
            int index = GameState.random.Next(copy_turns.Count);
            list_of_colors.Add(copy_turns[index]);
            copy_turns.RemoveAt(index);
        }

        return list_of_colors;
    }


    // generate the randomized a color list to track turns
    private static List<Color> create_turns(int playerCount) {
        List<int> list = new List<int>();

        for (int i = 0; i < playerCount; i++) {
            list.Add(i);
        }


        List<int> randomized = new List<int>();

        while (list.Count > 0) {
            int index = GameState.random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<Color> output = new List<Color>();

        foreach (int color_index in randomized) {
            output.Add(GameState.int_to_color(color_index));
        }

        return output;
    }



    // returns a color from a int for randomizing color
    public static Color int_to_color(int num) 
    {
        switch (num) 
        {
            case 0:
                return Color.green;
            case 1:
                return Color.blue;
            case 2:
                return Color.red;
            case 3:
                return Color.cyan;
            case 4: 
                return Color.magenta;
            case 5: 
                return Color.yellow;
            case 6:
                return new Color(1.0F, 0.5F, 0.0F, 1.0F);
            case 7: 
                return new Color(0.5F, 0.1F, 1F, 1.0F);
            default:
                Debug.Log("default clause hit");
                throw new System.Exception("wtf");

        }
    }

    // deal with country click
    // top level general method
    public void take_country_click(Country country) {
        //this handles highlighting (if nothing is highlighted)
        if (this.highlighted == null) {
            // handles the case  where this turn's player clicked a country not owned by this player
            if (this.turn_color != country.color) return;
            this.highlight(country);
            return;
        }  

        //from here onwards is when smth is highlighted and it about to handle a country click 
        //check if the country being clicked is attackable, positive index is true, else is false
        int index = this.considered.IndexOf(country);

        //if clicked country is unattackable, unhighlights and returns
        if (index < 0) {
            this.unhighlight();
            return;
        }

        //from here is attacking

        //changes the country's owner and color
        this.attack(index);

        //changes turns
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