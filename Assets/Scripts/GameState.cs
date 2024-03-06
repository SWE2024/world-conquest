using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameState
{
    // random generator
    public static System.Random random = new System.Random();
  
    // holds the reference to the singleton instance
    private static GameState instance = null;
    int playerCount;
    Canvas user_input;
    public delegate void Delegate_Var(GameObject selectedObj);

    public Delegate_Var Handle_Country_Click;

    // list of countries
    public List<Country> list_of_countries = new List<Country>();

    //this is map to get the country instance that holds the button that is clicked
    Dictionary<Button, Country> country_map = new Dictionary<Button, Country>();

    // these are related to the turns
    Image square;
  
    // current turn's color
    Player turn_player;

    // this holds the order of turn represented by color
    List<Player> turns_order;

    // it's the index to the turns order to know aht is next
    int turn_index = 0;

    // represent a state, if it holds a country that country is highlighted
    // if not highlighted, holds null
    Country highlighted = null;

    // represent the same state as above, if it holds a list of country a country is highlighted (can be empty but still means highlighted)
    // if not highlighted, holds null
    // country held in the list are the attackable countries when a country is selected. so not all neighboring countries will be here
    // only the attckable neighboring country
    List<Country> considered = null;

    int populated_country_count = 0;

    public void set_hashmap(Dictionary<Button, Country> map)    
    {
        this.country_map = map;
    }

    private GameState(int playerCount, Canvas user_input)
    {
        this.user_input = user_input;
        // this.list_of_countries = list;
        this.playerCount = playerCount;

        // creates the turns order here
        this.turns_order = GameState.create_turns(this.playerCount);

        // set the first turn color
        this.turn_player = this.turns_order[0];
        this.square = GameObject.Find("CurrentColour").GetComponent<Image>();
        /*
         * USE THIS LINE TO CHANGE THE PROFILE PICTURE CIRCLE:
         * GameObject.Find("CurrentPlayer").GetComponent<Image>();
         */
        this.square.color = this.get_turns_color();

        this.Handle_Country_Click = populating_take_country_click;
    }

    //singleton's constructor method access thru here
    public static GameState New(int playerCount, Canvas user_input)
    {
        if (instance != null) return GameState.instance;
        GameState.instance = new GameState(playerCount, user_input);
        return GameState.instance;
    }

    public void reset_turn()
    {
        this.turn_color = this.turns_order[0];
        this.turn_index = 0;
    }

    public static GameState Get()
    {
        return GameState.instance;
    }

    // this generates a of colors that represents the distributed number of countries, 
    // if 5 reds are here red holds five countries
    public List<Color> generate_list_of_colors()
    {
        int num_of_countries = 44 / this.playerCount;
        int remainder = 44 % this.playerCount;

        List<Color> list_of_colors = new List<Color>();
        List<Color> copy_turns = new List<Color>();

        foreach (Color color in this.generate_turns_order_as_colors())
        {
            copy_turns.Add(color);
            for (int i = 0; i < num_of_countries; i++) list_of_colors.Add(color);
        }

        for (int i = 0; i < remainder; i++)
        {
            int index = GameState.random.Next(copy_turns.Count);
            list_of_colors.Add(copy_turns[index]);
            copy_turns.RemoveAt(index);
        }
        return list_of_colors;
    }

    public List<Color> generate_turns_order_as_colors() 
    {
        List<Color> output = new List<Color>();

        foreach(Player player in this.turns_order) 
        {
            output.Add(player.color);
        }
        return output;
    }

    // generate the randomized a color list to track turns
    private static List<Player> create_turns(int playerCount)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < playerCount; i++)
        {
            list.Add(i);
        }

        List<int> randomized = new List<int>();

        while (list.Count > 0)
        {
            int index = GameState.random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<Player> output = new List<Player>();

        foreach (int color_index in randomized)
        {
            output.Add(new Player(GameState.int_to_color(color_index)));
        }

        return output;
    }

    // returns a color from a int for randomizing color
    public static Color int_to_color(int num)
    {
        switch (num)
        {
            case 0:
                return new Color(0.95f, 0.30f, 0.30f);
            case 1:
                return new Color(0.25f, 0.25f, 0.50f);
            case 2:
                return new Color(0.35f, 0.70f, 0.30f);
            case 3:
                return new Color(0.50f, 0.30f, 0.50f);
            case 4:
                return new Color(0.40f, 0.25f, 0.10f);
            case 5:
                return new Color(0.80f, 0.80f, 0.00f);
            default:
                throw new System.Exception("color not found");
        }
    }

    public void populating_take_country_click(GameObject selectedObj) 
    {
        if (selectedObj == null || !selectedObj.name.StartsWith("country")) return;

        Country country = this.country_map[selectedObj.GetComponent<Button>()];
        if (country == null || country.owner != null) return;

        // user_input.enabled = true;

        country.set_owner(this.turn_player);
        country.set_troops(1);
        this.turn_player.num_of_troops--;
        this.next_turn();
        this.populated_country_count++;

        if (populated_country_count == 44) {
            Debug.Log("hit this clause");
            this.reset_turn();

            bool flag = false;

            foreach(Player player in this.turns_order)
            {
                if (player.num_of_troops > 0) flag = true;
                if (player.num_of_troops < 0) player.num_of_troops = 0;
            }

            if (flag) Handle_Country_Click = distributing_troops_take_country_click;
            else Handle_Country_Click = attack_take_country_click;
            return;
        }
    }

    public bool next_player_with_troops()
    {
        Player player = null;
        while (true) {
            if (this.turn_player.num_of_troops > 0) {
                player = this.turn_player;
                break;
            }

            if (this.turn_index == this.turns_order.Count - 1) break;
            this.next_turn();
        }

        if (player != null) return true;
        return false;
    }

    public void distributing_troops_take_country_click(GameObject selectedObj)
    {
        if (selectedObj == null) return;
        
        String objName = selectedObj.name;

        if (objName.StartsWith("country")) {
            Country country = this.country_map[selectedObj.GetComponent<Button>()];
            if (country.owner != this.turn_player) return;

            this.highlighted = country;
            GameObject.Find("Remaining").GetComponent<TextMeshProUGUI>().text = $"Remaining : {this.turn_player.num_of_troops}";

            this.user_input.enabled = true;
            return;
        }

        if (objName == "Confirm") {
            Debug.Log("confirm pressed");

            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            this.highlighted.increase_troops(num);
            this.turn_player.num_of_troops -= num;
            this.highlighted = null;
            this.user_input.enabled = false;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";

            if (this.turn_player.num_of_troops > 0) return;

            bool check = this.next_player_with_troops();

            if (check) return;

            this.reset_turn();
            this.Handle_Country_Click = attack_take_country_click;
            return;
        } else if (objName == "Cancel") {
            this.highlighted = null;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";
            this.user_input.enabled = false;
            return;
        } else if (objName == "ButtonPlus") {
            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            if (num == this.turn_player.num_of_troops) return;
            num++;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num}";
            return;
        } else if (objName == "ButtonMinus") {
            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            if (num == 1) return;
            num--;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num}";
            return;
        }
    }

    // deal with country click
    // top level general method
    public void attack_take_country_click(GameObject selectedObj)
    {
        //this handles highlighting (if nothing is highlighted)
        if (this.highlighted == null)
        {
            if (selectedObj == null) return;
            Country country_selected = this.country_map[selectedObj.GetComponent<Button>()];

            // handles the case  where this turn's player clicked a country not owned by this player
            if (this.turn_player != country_selected.owner) return;
            this.highlight(country_selected);
            return;
        }

        //from here onwards is when smth is highlighted and it about to handle a country click 
        //check if the country being clicked is attackable, positive index is true, else is false

        if (selectedObj == null || !selectedObj.name.StartsWith("country")) 
        {
            this.unhighlight();
            return;
        }

        Country country = this.country_map[selectedObj.GetComponent<Button>()];
        int index = this.considered.IndexOf(country);

        //if clicked country is unattackable, unhighlights and returns
        if (index < 0)
        {
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

    public void highlight(Country country)
    {
        this.highlighted = country;
        this.considered = country.highlight();
        return;
    }

    public void unhighlight()
    {
        this.highlighted.reverse_color_change();

        foreach (Country country in this.considered)
        {
            country.reverse_color_change();
        }

        this.highlighted = null;
        this.considered = null;
    }

    public void attack(int index)
    {
        Country attacked = this.considered[index];
        //remove the attacked country from considered 
        this.considered.RemoveAt(index);

        //change the attacked country color
        attacked.set_owner(this.turn_player);

        //unhighlight the rest
        this.unhighlight();
    }

    public void next_turn()
    {
        this.turn_index++;

        if (this.turn_index > (this.turns_order.Count - 1))
        {
            this.turn_index = 0;
        }

        Debug.Log($"the turn now is {this.turn_index}");

        this.turn_player = this.turns_order[this.turn_index];
        this.square.GetComponent<Image>().color = this.get_turns_color();
    }

    public void reset_turn()
    {
        this.turn_index = 0;
        this.turn_player = this.turns_order[0];
        this.square.GetComponent<Image>().color = this.get_turns_color();
    }

    public Color get_turns_color()
    {
        return this.turn_player.color;
    }
}