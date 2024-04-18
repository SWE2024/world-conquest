using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController
{
    public static System.Random Random = new System.Random();
    static GameController instance = null;

    public delegate void DelegateVar(GameObject selectedObj);
    public DelegateVar HandleObjectClick;
    public Canvas DistributeCanvas;
    public Canvas AttackCanvas;
    public Canvas TransferCanvas;
    public Canvas DiceCanvas;
    public List<Country> ListOfCountries = new List<Country>();

    Country[] recentFight = new Country[2];

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

    private GameController(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas transferCanvas, Canvas diceCanvas)
    {
        this.turnIndex = 0;
        this.populatedCountries = 0;
        this.flagDistributionPhase = true;
        //this.flagGamePhase = false;

        this.playerCount = playerCount;
        this.DistributeCanvas = distributeCanvas;
        this.AttackCanvas = attackCanvas;
        this.TransferCanvas = transferCanvas;
        this.DiceCanvas = diceCanvas;

        // creates the turns order here
        this.turnsOrder = GameController.CreateTurns(this.playerCount);

        // set the first turn color
        this.turnPlayer = this.turnsOrder[0];
        this.square = GameObject.Find("CurrentColour").GetComponent<Image>();
        GameObject.Find("EndTurn").GetComponent<Image>().enabled = true;
        GameObject.Find("EndTurn").GetComponent<Button>().enabled = true;

        /*
         * USE THIS LINE TO CHANGE THE PROFILE PICTURE CIRCLE:
         * GameObject.Find("CurrentPlayer").GetComponent<Image>();
         */
        this.square.color = this.GetTurnsColor();
        this.HandleObjectClick = PopulatingCountryClick;
    }

    //singleton's constructor method access thru here
    public static GameController New(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas transferCanvas, Canvas diceCanvas)
    {
        if (instance != null) return GameController.instance;
        GameController.instance = new GameController(playerCount, distributeCanvas, attackCanvas, transferCanvas, diceCanvas);
        return GameController.instance;
    }

    public static GameController Get() => GameController.instance;

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
        int numberOfCountries = 0;
        int remainder = 0;

        switch (Preferences.MapNumber)
        {
            case 1:
                numberOfCountries = 44 / this.playerCount;
                remainder = 44 % this.playerCount;
                break;

            case 2:
                numberOfCountries = 27 / this.playerCount;
                remainder = 27 % this.playerCount;
                break;
        }

        List<Color> listOfColors = new List<Color>();
        List<Color> copyTurns = new List<Color>();

        foreach (Color color in this.GenerateTurnsOrderAsColors())
        {
            copyTurns.Add(color);
            for (int i = 0; i < numberOfCountries; i++) listOfColors.Add(color);
        }

        for (int i = 0; i < remainder; i++)
        {
            int index = GameController.Random.Next(copyTurns.Count);
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
            int index = GameController.Random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<Player> output = new List<Player>();

        foreach (int color_index in randomized)
        {
            output.Add(new Player(GameController.IntToColor(color_index)));
        }

        return output;
    }

    public void SetCountryMap(Dictionary<Button, Country> map)
    {
        this.countryMap = map;
    }

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

        if (populatedCountries >= countryMap.Count)
        {
            Debug.Log("EVENT: all territories owned");
            this.ResetTurn();

            flagDistributionPhase = false;
            //flagGamePhase = true;
            GameObject.Find("EndTurn").GetComponent<Image>().enabled = true;
            GameObject.Find("EndTurn").GetComponent<Button>().enabled = true;

            foreach (Player player in turnsOrder)
            {
                if (player.GetNumberOfTroops() > 0) flagDistributionPhase = true;
            }

            if (flagDistributionPhase) HandleObjectClick = DistributingTroopsCountryClick;
            else HandleObjectClick = CountryClick;
        }
    }

    public void DistributingTroopsCountryClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>();

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

                int num = Int32.Parse(numberOfTroops.GetComponent<TextMeshProUGUI>().text);
                this.attacker.ChangeTroops(num);
                this.turnPlayer.ChangeNumberOfTroops(-num);
                this.attacker = null;
                this.DistributeCanvas.enabled = false;
                numberOfTroops.text = "1";

                if (turnPlayer.GetNumberOfTroops() > 0) return;

                bool check = this.NextPlayerWithTroops();

                if (check) return;

                ResetTurn();
                HandleObjectClick = CountryClick;
                return;

            case "Cancel":
                CameraHandler.DisableMovement = false;

                this.attacker = null;
                numberOfTroops.text = "1";
                this.DistributeCanvas.enabled = false;
                return;

            case "ButtonPlus":
                int num1 = Int32.Parse(numberOfTroops.text);
                if (num1 == this.turnPlayer.GetNumberOfTroops())
                {
                    num1 = 1;
                    numberOfTroops.text = $"{num1}";
                    return;
                }
                num1++;
                numberOfTroops.text = $"{num1}";
                return;

            case "ButtonMinus":
                int num2 = Int32.Parse(numberOfTroops.text);
                if (num2 == 1)
                {
                    num2 = this.turnPlayer.GetNumberOfTroops();
                    numberOfTroops.text = $"{num2}";
                    return;
                }
                num2--;
                numberOfTroops.text = $"{num2}";
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
    public void CountryClick(GameObject selectedObj)
    {
        if (AttackCanvas.enabled)
        {
            HandleAttackClick(selectedObj);
            return;
        }

        if (TransferCanvas.enabled)
        {
            HandleTransferClick(selectedObj);
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
        CameraHandler.DisableMovement = true;

        GameObject.Find("RemainingAttack").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Attack: {attacker.GetTroops() - 1}\r\n(choose how many dice to roll)";

        this.defender = clickedCountry;
    }

    private void HandleAttackClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
        int num = Int32.Parse(numberOfTroops.text);

        switch (selectedObj.name)
        {
            case "Confirm":
                this.AttackCanvas.enabled = false;
                this.Attack(attacker, defender, num);

                if (attacker.GetOwner() == defender.GetOwner())
                {
                    recentFight[0] = attacker;
                    recentFight[1] = defender;
                    Wait.Start(4f, () => { 
                        this.TransferCanvas.enabled = true;
                        GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Transfer: {recentFight[0].GetTroops() - 1}"; ;
                    });
                }

                this.UnHighlight();
                numberOfTroops.text = "1";
                CameraHandler.DisableMovement = false;
                return;

            case "Cancel":
                this.AttackCanvas.enabled = false;
                this.UnHighlight();
                numberOfTroops.text = "1";
                CameraHandler.DisableMovement = false;
                return;

            case "ButtonPlus":
                if (num == 3 || num == attacker.GetTroops() - 1) return;

                num++;
                numberOfTroops.text = "" + num;
                return;

            case "ButtonMinus":
                if (num == 1) return;

                num--;
                numberOfTroops.text = "" + num;
                return;

            default: return;
        }
    }

    private void HandleTransferClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        int available = recentFight[0].GetTroops() - 1;

        TextMeshProUGUI troopsLeft = GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>();
        troopsLeft.text = $"Troops Left To Transfer: {available}";

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToTransfer").GetComponent<TextMeshProUGUI>();
        int num = Int32.Parse(numberOfTroops.text);

        switch (selectedObj.name)
        {
            case "Confirm":
                this.TransferCanvas.enabled = false;

                this.Transfer(recentFight[0], recentFight[1], num);

                numberOfTroops.text = "1";
                return;

            case "Cancel":
                this.TransferCanvas.enabled = false;
                numberOfTroops.text = "1";
                return;

            case "ButtonPlus":
                if (num == available) return;

                num++;
                numberOfTroops.text = "" + num;
                return;

            case "ButtonMinus":
                if (num == 1)
                {
                    num = available;
                    numberOfTroops.text = "" + num;
                    return;
                }

                num--;
                numberOfTroops.text = "" + num;
                return;

            default: return;
        }
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

    public void Attack(Country attacker, Country defender, int num)
    {
        List<int> atkRolls = new List<int>();
        List<int> defRolls = new List<int>();

        for (int i = 1; i <= num; i++)
        {
            int roll = Dice.Roll();
            atkRolls.Add(roll);
            GameObject.Find($"AttackerDiceRoll{i}").GetComponent<TextMeshProUGUI>().text = $"Attacker Rolls - {roll}";
        }

        for (int i = 1; i <= Mathf.Min(Mathf.Min(defender.GetTroops(), 3), num); i++)
        {
            int roll = Dice.Roll();
            defRolls.Add(roll);
            GameObject.Find($"DefenderDiceRoll{i}").GetComponent<TextMeshProUGUI>().text = $"Defender Rolls - {roll}";
        }

        int atkWins = 0;
        int atkLosses = 0;

        if (atkRolls.Count > defRolls.Count)
        {
            // handle if there are less defender rolls
            // take the best rolls from attacker
            // puts them in a new list of size defender rolls

            atkRolls.Sort();
            atkRolls.Reverse();
            List<int> newAtkRolls = new List<int>();

            for (int i = 0; i < defRolls.Count; i++)
            {
                newAtkRolls.Add(atkRolls[i]);
            }

            atkRolls = newAtkRolls;
        }

        for (int i = 0; i < atkRolls.Count; i++)
        {
            GameObject.Find($"AttackerDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().text = $"Attacker Rolls - {atkRolls[i]}";
            GameObject.Find($"DefenderDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().text = $"Defender Rolls - {defRolls[i]}";

            if (atkRolls[i] > defRolls[i])
            {
                // attacker wins the battle
                atkWins++;
                defender.ChangeTroops(-1);

                GameObject.Find($"AttackerDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().color = Color.green;
                GameObject.Find($"DefenderDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                // defender wins the battle
                atkLosses++;
                attacker.ChangeTroops(-1);

                GameObject.Find($"AttackerDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().color = Color.red;
                GameObject.Find($"DefenderDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().color = Color.green;
            }

            // fights have ended

            string s = $"Attacker Lost {atkLosses} Troop(s)!\nDefender Lost {atkWins} Troop(s)!";
            GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = s;

            if (defender.GetTroops() == 0)
            {
                GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = $"You Successfully Invaded!";

                defender.SetOwner(attacker.GetOwner());
            }
        }

        this.DiceCanvas.enabled = true;

        Wait.Start(4f, () =>
        {
            this.DiceCanvas.enabled = false;

            // reset the UI for the next attack

            for (int i = 1; i <= 3; i++)
            {
                GameObject.Find($"AttackerDiceRoll{i}").GetComponent<TextMeshProUGUI>().text = "";
                GameObject.Find($"DefenderDiceRoll{i}").GetComponent<TextMeshProUGUI>().text = "";
                GameObject.Find($"AttackerDiceRoll{i}").GetComponent<TextMeshProUGUI>().color = Color.white;
                GameObject.Find($"DefenderDiceRoll{i}").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            GameObject.Find($"WinnerText").GetComponent<TextMeshProUGUI>().text = "";
        });
    }

    public void Transfer(Country from, Country to, int num)
    {
        from.ChangeTroops(-num);
        to.ChangeTroops(num);
    }

    public void NextTurn()
    {
        this.turnIndex++;

        if (this.turnIndex > (this.turnsOrder.Count - 1)) this.turnIndex = 0;

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