using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameState
{
    public static System.Random Random = new System.Random();
    static GameState instance = null;

    public delegate void DelegateVar(GameObject selectedObj);
    public DelegateVar HandleCountryClick;
    public Canvas DistributeCanvas;
    public List<Country> ListOfCountries = new List<Country>();

    int playerCount;

    //this is map to get the country instance that holds the button that is clicked
    Dictionary<Button, Country> countryMap = new Dictionary<Button, Country>();

    // these are related to the turns
    Image square;
    Player turnPlayer;

    // this holds the order of turn represented by color
    List<Player> turnsOrder;

    // represent a state, if it holds a country that country is highlighted
    // if not highlighted, holds null
    Country highlighted = null;

    // represent the same state as above, if it holds a list of country a country is highlighted (can be empty but still means highlighted)
    // if not highlighted, holds null
    // country held in the list are the Attackable countries when a country is selected. so not all neighboring countries will be here
    // only the attckable neighboring country
    List<Country> considered = null;

    int turnIndex = 0; // indicates who is playing
    int populatedCountries = 0;

    public void SetHashmap(Dictionary<Button, Country> map)
    {
        this.countryMap = map;
    }

    private GameState(int playerCount, Canvas distributeCanvas)
    {
        this.DistributeCanvas = distributeCanvas;
        this.playerCount = playerCount;

        // creates the turns order here
        this.turnsOrder = GameState.CreateTurns(this.playerCount);

        // set the first turn color
        this.turnPlayer = this.turnsOrder[0];
        this.square = GameObject.Find("CurrentColour").GetComponent<Image>();
        /*
         * USE THIS LINE TO CHANGE THE PROFILE PICTURE CIRCLE:
         * GameObject.Find("CurrentPlayer").GetComponent<Image>();
         */
        this.square.color = this.GetTurnsColor();

        this.HandleCountryClick = PopulatingTakeCountryClick;
    }

    //singleton's constructor method access thru here
    public static GameState New(int playerCount, Canvas distributeCanvas)
    {
        if (instance != null) return GameState.instance;
        GameState.instance = new GameState(playerCount, distributeCanvas);
        return GameState.instance;
    }

    public static GameState Get() => GameState.instance;

    // this generates a of colors that represents the distributed number of countries, 
    // if 5 reds are here red holds five countries
    public List<Color> GenerateListOfColors()
    {
        int numberOfCountries = 44 / this.playerCount;
        int remainder = 44 % this.playerCount;

        List<Color> listOfColors = new List<Color>();
        List<Color> copyTurns = new List<Color>();

        foreach (Color color in this.GenerateTurnsOrderAsColors())
        {
            copyTurns.Add(color);
            for (int i = 0; i < numberOfCountries; i++) listOfColors.Add(color);
        }

        for (int i = 0; i < remainder; i++)
        {
            int index = GameState.Random.Next(copyTurns.Count);
            listOfColors.Add(copyTurns[index]);
            copyTurns.RemoveAt(index);
        }

        return listOfColors;
    }

    public List<Color> GenerateTurnsOrderAsColors()
    {
        List<Color> output = new List<Color>();

        foreach (Player player in this.turnsOrder)
        {
            output.Add(player.Color);
        }
        return output;
    }

    // generate the Randomized a color list to track turns
    private static List<Player> CreateTurns(int playerCount)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < playerCount; i++)
        {
            list.Add(i);
        }

        List<int> randomized = new List<int>();

        while (list.Count > 0)
        {
            int index = GameState.Random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<Player> output = new List<Player>();

        foreach (int color_index in randomized)
        {
            output.Add(new Player(GameState.IntToColor(color_index)));
        }

        return output;
    }

    // returns a color from a int for Randomizing color
    public static Color IntToColor(int num)
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
                return new Color(0.00f, 0.75f, 0.70f);
            case 5:
                return new Color(0.80f, 0.80f, 0.00f);
            default:
                throw new System.Exception("color not found");
        }
    }

    public void PopulatingTakeCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null || !selectedObj.name.StartsWith("country")) return;

        Country country = this.countryMap[selectedObj.GetComponent<Button>()];
        if (country == null) return;
        if (country.Owner != null) return;

        // DistributeCanvas.enabled = true;

        country.SetOwner(this.turnPlayer);
        country.SetTroops(1);
        this.turnPlayer.NumberOfTroops--;
        this.NextTurn();
        this.populatedCountries++;


        if (populatedCountries == 44)
        {
            Debug.Log("all countries are populated");
            this.ResetTurn();

            bool flag = false;

            foreach (Player player in this.turnsOrder)
            {
                if (player.NumberOfTroops > 0) flag = true;
                if (player.NumberOfTroops < 0) player.NumberOfTroops = 0;
            }

            if (flag) HandleCountryClick = distributing_Troops_take_country_click;
            else HandleCountryClick = Attack_take_country_click;
            return;
        }
    }

    public bool next_player_with_Troops()
    {
        Player player = null;
        while (true)
        {
            if (this.turnPlayer.NumberOfTroops > 0)
            {
                player = this.turnPlayer;
                break;
            }

            if (this.turnIndex == this.turnsOrder.Count - 1) break;
            this.NextTurn();
        }

        if (player != null) return true;
        return false;
    }

    public void distributing_Troops_take_country_click(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        String objName = selectedObj.name;

        if (objName.StartsWith("country"))
        {
            CameraHandler.InDistributionPhase = true;
            Country country = this.countryMap[selectedObj.GetComponent<Button>()];
            if (country.Owner != this.turnPlayer) return;

            this.highlighted = country;
            GameObject.Find("Remaining").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Deploy: {this.turnPlayer.NumberOfTroops}";

            this.DistributeCanvas.enabled = true;
            return;
        }

        if (objName == "Confirm")
        {
            CameraHandler.InDistributionPhase = false;

            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            this.highlighted.IncreaseTroops(num);
            this.turnPlayer.NumberOfTroops -= num;
            this.highlighted = null;
            this.DistributeCanvas.enabled = false;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";

            if (this.turnPlayer.NumberOfTroops > 0) return;

            bool check = this.next_player_with_Troops();

            if (check) return;

            this.ResetTurn();
            this.HandleCountryClick = Attack_take_country_click;
            return;
        }
        else if (objName == "Cancel")
        {
            CameraHandler.InDistributionPhase = false;

            this.highlighted = null;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";
            this.DistributeCanvas.enabled = false;
            return;
        }
        else if (objName == "ButtonPlus")
        {
            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            if (num == this.turnPlayer.NumberOfTroops) return;
            num++;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num}";
            return;
        }
        else if (objName == "ButtonMinus")
        {
            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            if (num == 1) return;
            num--;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num}";
            return;
        }
    }

    // deal with country click
    // top level general method
    public void Attack_take_country_click(GameObject selectedObj)
    {
        //this handles Highlighting (if nothing is highlighted)
        if (this.highlighted == null)
        {
            if (selectedObj == null) return;
            Country country_selected = this.countryMap[selectedObj.GetComponent<Button>()];

            // handles the case  where this turn's player clicked a country not owned by this player
            if (this.turnPlayer != country_selected.Owner) return;
            this.Highlight(country_selected);
            return;
        }

        //from here onwards is when smth is highlighted and it about to handle a country click 
        //check if the country being clicked is Attackable, positive index is true, else is false

        if (selectedObj == null || !selectedObj.name.StartsWith("country"))
        {
            this.unHighlight();
            return;
        }

        Country country = this.countryMap[selectedObj.GetComponent<Button>()];
        int index = this.considered.IndexOf(country);

        //if clicked country is unAttackable, unHighlights and returns
        if (index < 0)
        {
            this.unHighlight();
            return;
        }

        //from here is Attacking

        //changes the country's Owner and color
        this.Attack(index);

        //changes turns
        this.NextTurn();

        return;
    }

    public void Highlight(Country country)
    {
        this.highlighted = country;
        this.considered = country.Highlight();
        return;
    }

    public void unHighlight()
    {
        this.highlighted.ReverseColorChange();

        foreach (Country country in this.considered)
        {
            country.ReverseColorChange();
        }

        this.highlighted = null;
        this.considered = null;
    }

    public void Attack(int index)
    {
        Country attacked = this.considered[index];
        //remove the Attacked country from considered 
        this.considered.RemoveAt(index);

        //change the Attacked country color
        attacked.SetOwner(this.turnPlayer);

        //unHighlight the rest
        this.unHighlight();
    }

    public void NextTurn()
    {
        this.turnIndex++;

        if (this.turnIndex > (this.turnsOrder.Count - 1))
        {
            this.turnIndex = 0;
        }

        // Debug.Log($"current player: {this.turnIndex}"); // uncomment for debug

        this.turnPlayer = this.turnsOrder[this.turnIndex];
        this.square.GetComponent<Image>().color = this.GetTurnsColor();
    }

    public void ResetTurn()
    {
        this.turnIndex = 0;
        this.turnPlayer = this.turnsOrder[0];
        this.square.GetComponent<Image>().color = this.GetTurnsColor();
    }

    public Color GetTurnsColor() => this.turnPlayer.Color;
}