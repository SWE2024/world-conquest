using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public Canvas AttackCanvas;
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
    Country target = null;

    // represent the same state as above, if it holds a list of country a country is highlighted (can be empty but still means highlighted)
    // if not highlighted, holds null
    // country held in the list are the Attackable countries when a country is selected. so not all neighboring countries will be here
    // only the attckable neighboring country
    List<Country> considered = null;

    int turnIndex = 0; // indicates who is playing
    int populatedCountries = 0;

    public static int DiceRoll() => Random.Next(1, 7);

    public void SetHashmap(Dictionary<Button, Country> map)
    {
        this.countryMap = map;
    }

    private GameState(int playerCount, Canvas distributeCanvas, Canvas attackCanvas)
    {
        this.playerCount = playerCount;
        this.DistributeCanvas = distributeCanvas;
        this.AttackCanvas = attackCanvas;

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
    public static GameState New(int playerCount, Canvas distributeCanvas, Canvas attackCanvas)
    {
        if (instance != null) return GameState.instance;
        GameState.instance = new GameState(playerCount, distributeCanvas, attackCanvas);
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

    private List<Color> GenerateTurnsOrderAsColors()
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
            case 0: return new Color(0.95f, 0.30f, 0.30f);
            case 1: return new Color(0.25f, 0.25f, 0.50f);
            case 2: return new Color(0.35f, 0.70f, 0.30f);
            case 3: return new Color(0.50f, 0.30f, 0.50f);
            case 4: return new Color(0.00f, 0.75f, 0.70f);
            case 5: return new Color(0.80f, 0.80f, 0.00f);
            default: throw new Exception("color not found");
        }
    }

    public void PopulatingTakeCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null || !selectedObj.name.StartsWith("country")) return;

        Country country = this.countryMap[selectedObj.GetComponent<Button>()];
        if (country == null) return;
        if (country.Owner != null) return;

        turnPlayer.AddCountry(country);
        country.SetOwner(turnPlayer);
        country.SetTroops(1);
        turnPlayer.NumberOfTroops--;
        populatedCountries++;

        NextTurn();

        if (populatedCountries == 44)
        {
            Debug.Log("all countries are populated");
            this.ResetTurn();

            bool flag = false;

            foreach (Player player in turnsOrder)
            {
                if (player.NumberOfTroops > 0) flag = true;
                if (player.NumberOfTroops < 0) player.NumberOfTroops = 0;
            }

            if (flag) HandleCountryClick = DistributingTroopsTakeCountryClick;
            else HandleCountryClick = AttackTakeCountryClick;
            return;
        }
    }

    public void DistributingTroopsTakeCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        String objName = selectedObj.name;

        if (objName.StartsWith("country"))
        {
            CameraHandler.DisableMovement = true;
            Country country = countryMap[selectedObj.GetComponent<Button>()];
            if (country.Owner != turnPlayer) return;

            highlighted = country;
            GameObject.Find("Remaining").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Deploy: {this.turnPlayer.NumberOfTroops}";

            DistributeCanvas.enabled = true;
            return;
        }

        /*
         * 
         * 
         * 
         * 
         *  PLEASE REWRITE THIS AS A SWITCH CASE
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */

        if (objName == "Confirm")
        {
            CameraHandler.DisableMovement = false;

            int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
            this.highlighted.IncreaseTroops(num);
            this.turnPlayer.NumberOfTroops -= num;
            this.highlighted = null;
            this.DistributeCanvas.enabled = false;
            GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";

            if (turnPlayer.NumberOfTroops > 0) return;

            bool check = this.NextPlayerWithTroops();

            if (check) return;

            ResetTurn();
            HandleCountryClick = AttackTakeCountryClick;
            return;
        }
        else if (objName == "Cancel")
        {
            CameraHandler.DisableMovement = false;

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

    public bool NextPlayerWithTroops()
    {
        Player player = null;
        while (true)
        {
            if (turnPlayer.NumberOfTroops>0)
            {
                player=turnPlayer;
                break;
            }

            if (turnIndex==turnsOrder.Count-1) break;
            NextTurn();
        }

        if (player!=null) return true;
        return false;
    }

    public void ReinforcementTakeClick(GameObject selectedObj)
    {
        if (selectedObj) return;
        return;
    }

    // deal with country click
    // top level general method
    public void AttackTakeCountryClick(GameObject selectedObj)
    {
        if (AttackCanvas.enabled)
        {
            TroopAttackEnabled(selectedObj);
            return;
        }

        //this handles Highlighting (if nothing is highlighted)
        if (highlighted == null)
        {
            if (selectedObj == null ||!selectedObj.name.StartsWith("country")) return;
            Country countrySelected = countryMap[selectedObj.GetComponent<Button>()];

            if (countrySelected.GetTroops() == 1)
            {
                Debug.Log("cannot attack with only 1 troop");
                return;
            }

            // handles the case  where this turn's player clicked a country not owned by this player
            if (turnPlayer != countrySelected.Owner) return;
            Highlight(countrySelected);
            return;
        }

        //from here onwards is when smth is highlighted and it about to handle a country click 
        //check if the country being clicked is Attackable, positive index is true, else is false
        if (selectedObj == null || !selectedObj.name.StartsWith("country"))
        {
            this.UnHighlight();
            return;
        }

        Country country = countryMap[selectedObj.GetComponent<Button>()];
        int index = considered.IndexOf(country);

        //if clicked country is unAttackable, UnHighlights and returns
        if (index < 0)
        {
            UnHighlight();
            return;
        }

        this.AttackCanvas.enabled = true;
        this.target = country;

        return;
    }

    private void TroopAttackEnabled(GameObject selectedObj)
    {
        if (selectedObj == null) return;
        
        switch (selectedObj.name)
        {
            case "ButtonMinus":
                TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
                int num = Int32.Parse(numberOfTroops.text);
                if (num == 1) return;
                numberOfTroops.text = "" + (num - 1);
                return;
            case "ButtonPlus":
                TextMeshProUGUI numberOfTroops1 = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
                int num1 = Int32.Parse(numberOfTroops1.text);
                if (num1 == 3)
                {
                    Debug.Log("max 3 reached");
                    return;
                }

                if (num1 == this.highlighted.GetTroops() - 1)
                {
                    Debug.Log("not enough troops");
                    return;
                }


                //if (num1 == 3 || num1 == target.GetTroops() - 1) return;
                numberOfTroops1.text = "" + (num1 + 1);
                return;
            case "Cancel":
                GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>().text = "1";
                this.UnHighlight();
                this.AttackCanvas.enabled = false;
                return;
            //case "Confirm":
                //return;
            default: return;
        }
    }

    public void FortificationTakeClick(GameObject selectedObj) 
    {
        if (selectedObj == null) return;
        return;
    }

    public void Highlight(Country country)
    {
        this.highlighted = country;
        this.considered = country.Highlight();
        return;
    }

    public void UnHighlight()
    {
        this.highlighted.ReverseColorChange();

        foreach (Country country in this.considered)
        {
            country.ReverseColorChange();
        }

        this.highlighted = null;
        this.considered = null;
    }

    public void Attack(int defenderIndex)
    {
        Country attacker = this.highlighted;
        Country defender = this.considered[defenderIndex];

        // remove the Attacked country from considered 
        this.considered.RemoveAt(defenderIndex);

        this.AttackCanvas.enabled = true;

        // UnHighlight the rest
        this.UnHighlight();
    }

    public void NextTurn()
    {
        this.turnIndex++;

        if (this.turnIndex > (this.turnsOrder.Count - 1))
        {
            this.turnIndex = 0;
        }

        // Debug.Log($"current player: {this.turnIndex}"); // uncomment for debug

        turnPlayer = turnsOrder[turnIndex];
        square.GetComponent<Image>().color = GetTurnsColor();
    }

    public void ResetTurn()
    {
        turnIndex = 0;
        turnPlayer = turnsOrder[0];
        square.GetComponent<Image>().color = GetTurnsColor();
    }

    public Color GetTurnsColor() => turnPlayer.Color;
}