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
    public Canvas AttackCanvas;
    public Canvas DiceCanvas;
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
    Country attacker = null;
    Country defender = null;

    // represent the same state as above, if it holds a list of country a country is highlighted (can be empty but still means highlighted)
    // if not highlighted, holds null
    // country held in the list are the Attackable countries when a country is selected. so not all neighboring countries will be here
    // only the attckable neighboring country
    List<Country> considered = null;

    int turnIndex; // indicates who is playing
    int populatedCountries;
    bool flagDistributionPhase;
    // bool flagGamePhase; // not required yet

    private GameState(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas diceCanvas)
    {
        this.turnIndex = 0;
        this.populatedCountries = 0;
        this.flagDistributionPhase = true;
        //this.flagGamePhase = false;

        this.playerCount = playerCount;
        this.DistributeCanvas = distributeCanvas;
        this.AttackCanvas = attackCanvas;
        this.DiceCanvas = diceCanvas;

        // creates the turns order here
        this.turnsOrder = GameState.CreateTurns(this.playerCount);

        // set the first turn color
        this.turnPlayer = this.turnsOrder[0];
        this.square = GameObject.Find("CurrentColour").GetComponent<Image>();
        GameObject.Find("EndTurn").GetComponent<Button>().enabled = false;

        /*
         * USE THIS LINE TO CHANGE THE PROFILE PICTURE CIRCLE:
         * GameObject.Find("CurrentPlayer").GetComponent<Image>();
         */
        this.square.color = this.GetTurnsColor();
        this.HandleCountryClick = PopulatingCountryClick;

        Debug.Log("EVENT: entering setup phase");
    }

    //singleton's constructor method access thru here
    public static GameState New(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas diceCanvas)
    {
        if (instance != null) return GameState.instance;
        GameState.instance = new GameState(playerCount, distributeCanvas, attackCanvas, diceCanvas);
        return GameState.instance;
    }

    public static GameState Get() => GameState.instance;

    // returns a color from a random int
    public static Color IntToColor(int num) => num switch
    {
        0 => new Color(0.95f, 0.30f, 0.30f),
        1 => new Color(0.25f, 0.25f, 0.50f),
        2 => new Color(0.35f, 0.70f, 0.30f),
        3 => new Color(0.50f, 0.30f, 0.50f),
        4 => new Color(0.00f, 0.75f, 0.70f),
        5 => new Color(0.80f, 0.80f, 0.00f),
        _ => throw new Exception("color not found"),
    };

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
            output.Add(player.GetColor());
        }
        return output;
    }

    // generate the Randomized a color list to track turns
    private static List<Player> CreateTurns(int playerCount)
    {
        List<int> list = new List<int>();
        List<int> randomized = new List<int>();

        for (int i = 0; i < playerCount; i++) list.Add(i);

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

    public void SetCountryMap(Dictionary<Button, Country> map)
    {
        this.countryMap = map;
    }

    public static int DiceRoll() => Random.Next(1, 7);

    public void PopulatingCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null || !selectedObj.name.StartsWith("country")) return;

        Country country = this.countryMap[selectedObj.GetComponent<Button>()];
        if (country == null) return;
        if (country.GetOwner() != null) return;

        country.SetOwner(turnPlayer);
        country.ChangeTroops(1);
        turnPlayer.ChangeNumberOfTroops(-1);
        populatedCountries++;

        NextTurn();

        if (populatedCountries >= 44)
        {
            Debug.Log("EVENT: starting game");
            this.ResetTurn();

            flagDistributionPhase = false;
            //flagGamePhase = true;
            GameObject.Find("EndTurn").GetComponent<Button>().enabled = true;

            foreach (Player player in turnsOrder)
            {
                if (player.GetNumberOfTroops() > 0) flagDistributionPhase = true;
            }

            if (flagDistributionPhase) HandleCountryClick = DistributingTroopsCountryClick;
            else HandleCountryClick = AttackCountryClick;
        }
    }

    public void DistributingTroopsCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        switch (selectedObj.name)
        {
            case string s when s.StartsWith("country"):
                CameraHandler.DisableMovement = true;
                Country country = countryMap[selectedObj.GetComponent<Button>()];

                if (country.GetOwner() != turnPlayer)
                {
                    defender = country;
                    return;
                }

                attacker = country;

                GameObject.Find("RemainingDistribution").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Deploy: {this.turnPlayer.GetNumberOfTroops()}";
                DistributeCanvas.enabled = true;
                return;

            case "Confirm":
                CameraHandler.DisableMovement = false;

                int num = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
                this.attacker.ChangeTroops(num);
                this.turnPlayer.ChangeNumberOfTroops(-num);
                this.attacker = null;
                this.DistributeCanvas.enabled = false;
                GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";

                if (turnPlayer.GetNumberOfTroops() > 0) return;

                bool check = this.NextPlayerWithTroops();

                if (check) return;

                ResetTurn();
                HandleCountryClick = AttackCountryClick;
                return;

            case "Cancel":
                CameraHandler.DisableMovement = false;

                this.attacker = null;
                GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = "1";
                this.DistributeCanvas.enabled = false;
                return;

            case "ButtonPlus":
                int num1 = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
                if (num1 == this.turnPlayer.GetNumberOfTroops())
                {
                    num1 = 1;
                    GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num1}";
                    return;
                }
                num1++;
                GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num1}";
                return;

            case "ButtonMinus":
                int num2 = Int32.Parse(GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text);
                if (num2 == 1)
                {
                    num2 = this.turnPlayer.GetNumberOfTroops();
                    GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num2}";
                    return;
                }
                num2--;
                GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>().text = $"{num2}";
                return;

            default: return;
        }
    }

    public bool NextPlayerWithTroops()
    {
        Player player = null;
        while (true)
        {
            if (turnPlayer.GetNumberOfTroops() > 0)
            {
                player = turnPlayer;
                break;
            }

            if (turnIndex == turnsOrder.Count - 1) break;
            NextTurn();
        }

        if (player != null) return true;
        return false;
    }

    public void ReinforcementCountryClick(GameObject selectedObj)
    {
        if (selectedObj) return;
        return;
    }

    // deal with country click
    // top level general method
    public void AttackCountryClick(GameObject selectedObj)
    {
        if (AttackCanvas.enabled)
        {
            TroopAttackEnabled(selectedObj);
            return;
        }

        if (selectedObj == null)
        {
            this.UnHighlight();
            return;
        }

        // ends the players turn if they press the exit button
        if (selectedObj.name == "EndTurn")
        {
            this.UnHighlight();
            NextTurn();
            return;
        }

        // this handles Highlighting (if nothing is highlighted)
        if (attacker == null)
        {
            if (!selectedObj.name.StartsWith("country")) return;
            Country countrySelected = countryMap[selectedObj.GetComponent<Button>()];

            if (countrySelected.GetTroops() < 2) return;

            // handles the case where the current player clicks a different player's country
            if (turnPlayer != countrySelected.GetOwner()) return;
            Highlight(countrySelected);
            return;
        }

        // from here onwards is when smth is highlighted and it about to handle a country click 
        // check if the country being clicked is Attackable, positive index is true, else is false
        if (!selectedObj.name.StartsWith("country"))
        {
            this.UnHighlight();
            return;
        }

        Country clickedCountry = countryMap[selectedObj.GetComponent<Button>()];
        int index = considered.IndexOf(clickedCountry);

        //if clicked country is unAttackable, UnHighlights and returns
        if (index < 0)
        {
            UnHighlight();
            return;
        }

        this.AttackCanvas.enabled = true;
        GameObject.Find("RemainingAttack").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Attack: {attacker.GetTroops() - 1}\r\n(choose how many dice to roll)";

        this.defender = clickedCountry;
    }

    private void TroopAttackEnabled(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        switch (selectedObj.name)
        {
            case "Confirm":
                this.AttackCanvas.enabled = false;
                this.Attack(attacker, defender);
                this.UnHighlight();
                return;

            case "Cancel":
                this.AttackCanvas.enabled = false;
                this.UnHighlight();
                GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>().text = "1";
                return;

            case "ButtonPlus":
                TextMeshProUGUI numberOfTroops1 = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
                int num1 = Int32.Parse(numberOfTroops1.text);

                if (num1 == 3 || num1 == this.attacker.GetTroops() - 1 || num1 == this.defender.GetTroops()) return;

                numberOfTroops1.text = "" + (num1 + 1);
                return;

            case "ButtonMinus":
                TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
                int num = Int32.Parse(numberOfTroops.text);

                if (num == 1) return;

                numberOfTroops.text = "" + (num - 1);
                return;

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
        this.attacker = country;
        this.considered = country.HighlightNeighbours();
        return;
    }

    public void UnHighlight()
    {
        if (attacker != null)
        {
            this.attacker.ReverseColorChange();

            foreach (Country country in this.considered) country.ReverseColorChange();

            this.attacker = null;
            this.considered = null;
        }
    }

    public void Attack(Country attacker, Country defender)
    {
        // this method only currently handles ONE dice roll

        int attackerDiceRoll = GameState.DiceRoll();
        int defenderDiceRoll = GameState.DiceRoll();

        GameObject.Find("AttackerDiceRoll").GetComponent<TextMeshProUGUI>().text = $"Attacker Rolls - {attackerDiceRoll}";
        GameObject.Find("DefenderDiceRoll").GetComponent<TextMeshProUGUI>().text = $"Defender Rolls - {defenderDiceRoll}";

        this.DiceCanvas.enabled = true;

        if (attackerDiceRoll > defenderDiceRoll && defender.GetTroops() == 1) // attacker successfully invades
        {
            GameObject.Find("AttackerDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.green;
            GameObject.Find("DefenderDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = $"Attacker Won Invasion!";

            // int transferredTroops = gameobject get numberoftransferredtroops etc etc etc
            // defender.ChangeTroops(-1);
            // attacker.ChangeTroops(-transferredTroops);
            // defender.ChangeTroops(transferredTroops);

            defender.SetOwner(turnPlayer);
            attacker.ChangeTroops(-1);
            Debug.Log($"EVENT: attacker successfully invaded the country");
        }

        else if (attackerDiceRoll > defenderDiceRoll) // attacker takes troops
        {
            GameObject.Find("AttackerDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.green;
            GameObject.Find("DefenderDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = $"Defender Lost 1 Troop(s)";

            defender.ChangeTroops(-1);
            Debug.Log($"EVENT: defender loses one troop");
        }

        else // defender wins
        {
            GameObject.Find("AttackerDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("DefenderDiceRoll").GetComponent<TextMeshProUGUI>().color = Color.green;
            GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = $"Attacker Lost 1 Troop(s)";

            attacker.ChangeTroops(-1);
            Debug.Log($"EVENT: attacker loses one troop");
        }

        Wait.Start(4f, () =>
        {
            this.DiceCanvas.enabled = false;
        });
    }

    public void NextTurn()
    {
        this.turnIndex++;

        if (this.turnIndex > (this.turnsOrder.Count - 1)) this.turnIndex = 0;

        // Debug.Log($"current player: {this.turnIndex}"); // uncomment for debug

        turnPlayer = turnsOrder[turnIndex];
        if (turnPlayer.GetNumberOfOwnedCountries() == 0 && !flagDistributionPhase)
        {
            Debug.Log($"EVENT: skipped player {turnIndex - 1}");
            NextTurn(); // ignores players who lost
        }

        square.GetComponent<Image>().color = GetTurnsColor();
    }

    public void ResetTurn()
    {
        turnIndex = 0;
        turnPlayer = turnsOrder[0];
        square.GetComponent<Image>().color = GetTurnsColor();
    }

    public Color GetTurnsColor() => turnPlayer.GetColor();
}