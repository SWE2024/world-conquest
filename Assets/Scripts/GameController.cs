using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// <c>GameController</c> handles the logic of the local version of the game.
/// </summary>
public class GameController
{
    static GameController instance = null;

    public delegate void DelegateVar(GameObject selectedObj);
    public DelegateVar HandleObjectClick;
    public Canvas DistributeCanvas;
    public Canvas AttackCanvas;
    public Canvas DefendCanvas;
    public Canvas TransferCanvas;
    public Canvas DiceCanvas;
    public Canvas CardInventory;
    public List<Country> ListOfCountries = new List<Country>();
    public static List<Card> ListOfCards = new List<Card>();

    Country[] recentFight = new Country[2];

    int playerCount;

    //this is map to get the country instance that holds the button that is clicked
    public Dictionary<Button, Country> countryMap = new Dictionary<Button, Country>();
    public List<Sprite> cardList = new List<Sprite>();

    // these are related to the turns
    public TextMeshProUGUI currentPhase;
    public TextMeshProUGUI currentPlayerName;
    public Image currentPlayerColor;
    public Player turnPlayer;

    // this holds the order of turn represented by color
    public List<Player> turnsOrder;
    public List<Player> eliminatedPlayers = new List<Player>();

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
    public int populatedCountries;

    private GameController(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas defendCanvas, Canvas transferCanvas, Canvas diceCanvas, Canvas cardInventory)
    {

        this.turnIndex = 0;
        this.populatedCountries = 0;

        this.playerCount = playerCount;
        this.DistributeCanvas = distributeCanvas;
        this.AttackCanvas = attackCanvas;
        this.DefendCanvas = defendCanvas;
        this.TransferCanvas = transferCanvas;
        this.DiceCanvas = diceCanvas;
        this.CardInventory = cardInventory;

        this.DistributeCanvas.enabled = false;
        this.AttackCanvas.enabled = false;
        this.DefendCanvas.enabled = false;
        this.TransferCanvas.enabled = false;
        this.DiceCanvas.enabled = false;
        this.CardInventory.enabled = false;
        GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = false;
        GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = false;
        GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled = false;
        GameObject.Find("PlayerEliminated").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CardNotification").GetComponent<Image>().enabled = false;
        GameObject.Find("TradeResult").GetComponent<Canvas>().enabled = false;

        // creates the turns order here
        this.turnsOrder = GameController.CreateTurns();

        // set the first turn color
        this.turnPlayer = this.turnsOrder[0];
        this.currentPhase = GameObject.Find("GamePhase").GetComponent<TextMeshProUGUI>();
        this.currentPlayerName = GameObject.Find("CurrentPlayer").GetComponent<TextMeshProUGUI>();
        this.currentPlayerColor = GameObject.Find("CurrentColour").GetComponent<Image>();

        this.currentPlayerName.text = "playing:\n" + this.GetTurnsName();
        this.currentPlayerColor.color = this.GetTurnsColor();
        this.currentPhase.text = "setup phase";
        this.HandleObjectClick = SetupPhase;
    }

    /// <summary>
    /// <c>New</c> creates and starts a new game.
    /// </summary>
    /// <param name="playerCount">The number of players the game will start with (players + agents).</param>
    /// <returns>
    /// New GameController instance.
    /// </returns> 
    public static GameController New(int playerCount, Canvas distributeCanvas, Canvas attackCanvas, Canvas defendCanvas, Canvas transferCanvas, Canvas diceCanvas, Canvas cardInventory)
    {
        //if (instance != null) return GameController.instance; // find a better way of joining a game that already exists, this does not work
        GameController.instance = new GameController(playerCount, distributeCanvas, attackCanvas, defendCanvas, transferCanvas, diceCanvas, cardInventory);
        return GameController.instance;
    }

    /// <summary>
    /// <c>Get</c> retrieves the instance of the currently open game.
    /// </summary>
    /// <returns>
    /// Current GameController instance (including current game state).
    /// </returns> 
    public static GameController Get() => GameController.instance;

    /// <summary>
    /// <c>IntToColor</c> converts a number into a color. Use when setting up the game to get player colors.
    /// </summary>
    /// <param name="num">The number of players the game will start with (players + agents).</param>
    /// <returns>
    /// Unique Color for each number input.
    /// </returns>
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

    /* unused
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
            int index = UnityEngine.Random.Range(0, copyTurns.Count);
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
    */

    /// <summary>
    /// <c>CreateTurns</c> gets the number of players required, creates the <c>Player</c> and <c>AIPlayer</c> objects required.
    /// Then, these objects are added to a list.
    /// </summary>
    /// <returns>
    /// List of players and AI agents in the order of their turns.
    /// </returns>
    private static List<Player> CreateTurns()
    {
        List<int> listOfPlayers = new List<int>();
        List<int> listOfAgents = new List<int>();

        for (int i = 0; i < Preferences.PlayerCount; i++) listOfPlayers.Add(i);
        for (int i = Preferences.PlayerCount; i < Preferences.PlayerCount + Preferences.AgentCount; i++) listOfAgents.Add(i);

        List<int> listOfPlayersAndAgents = new List<int>();
        listOfPlayersAndAgents.AddRange(listOfPlayers);
        listOfPlayersAndAgents.AddRange(listOfAgents);

        List<Player> output = new List<Player>();

        foreach (int color_index in listOfPlayersAndAgents) // used to be color_index in randomized
        {
            if (color_index >= Preferences.PlayerCount) output.Add(new AIPlayer("(AI) Agent" + (color_index + 1), GameController.IntToColor(color_index)));
            else output.Add(new Player("Player" + (color_index + 1), GameController.IntToColor(color_index)));
        }

        return output;
    }

    /// <summary>
    /// <c>SetCountryMap</c> sets the map of buttons to Country objects to handle clicks correctly.
    /// </summary>
    /// <param name="map">The map of buttons to their <c>Country</c> object.</param>
    public void SetCountryMap(Dictionary<Button, Country> map)
    {
        this.countryMap = map;
    }

    /// <summary>
    /// <c>SetupPhase</c> handles clicks in the phase where not all countries are owned.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    public void SetupPhase(GameObject selectedObj)
    {
        if (selectedObj == null || (!selectedObj.name.StartsWith("country") && !selectedObj.name.StartsWith("Rename"))) return;

        if (selectedObj.name.StartsWith("Rename"))
        {
            HandleRenameClick(selectedObj);
            return;
        }

        Country country = this.countryMap[selectedObj.GetComponent<Button>()];
        if (country == null) return;
        if (country.GetOwner() != null) return;

        country.SetOwner(turnPlayer);
        country.ChangeTroops(1);
        turnPlayer.ChangeNumberOfTroops(-1);
        populatedCountries++;

        GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
        Killfeed.Update($"{turnPlayer.GetName()}: now owns {country.GetName()}");

        if (populatedCountries < countryMap.Count) NextTurn();
        else
        {
            this.ResetTurn();

            this.currentPhase.text = "deploy phase";
            HandleObjectClick = SetupDeployPhase;
        }
    }

    /// <summary>
    /// <c>SetupDeployPhase</c> handles clicks in the phase where all countries are owned;
    /// but some players have troops left to deploy.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    public void SetupDeployPhase(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        if (selectedObj.name.StartsWith("Rename"))
        {
            HandleRenameClick(selectedObj);
            return;
        }

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>();

        switch (selectedObj.name)
        {
            case string s when s.StartsWith("country"):
                CameraHandler.DisableMovement = true;
                Country country = countryMap[selectedObj.GetComponent<Button>()];

                if (country.GetOwner() != turnPlayer) return;

                attacker = country;

                GameObject.Find("RemainingDistribution").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Deploy: {this.turnPlayer.GetNumberOfTroops()}";
                DistributeCanvas.enabled = true;
                return;

            case "Confirm":
                CameraHandler.DisableMovement = false;

                int num = Int32.Parse(numberOfTroops.GetComponent<TextMeshProUGUI>().text);
                this.attacker.ChangeTroops(num);
                this.turnPlayer.ChangeNumberOfTroops(-num);

                GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                Killfeed.Update($"{turnPlayer.GetName()}: sent {num} troop(s) to {attacker.GetName()}");

                this.attacker = null;
                this.DistributeCanvas.enabled = false;
                numberOfTroops.text = "1";

                if (turnPlayer.GetNumberOfTroops() > 0) return;
                NextTurn();


                if (turnPlayer.GetNumberOfTroops() <= 0) // if the next player still has 0
                {
                    ResetTurn();
                    this.turnPlayer.GetNewTroopsAndCards();
                    // phase changing to draft
                    this.currentPhase.text = "draft phase";

                    HandleObjectClick = DraftPhase;
                    // this.turnPlayer.FillCards();
                    this.turnPlayer.InitializeSlot();

                    GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = true;
                    GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = true;
                    GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                    GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
                    return;
                }

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

    /// <summary>
    /// <c>DraftPhase</c> handles clicks in the first phase of each player's turn, troops are deployed.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    public void DraftPhase(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        if (CardInventory.enabled)
        {
            HandleCardClick(selectedObj);
            return;
        }

        if (selectedObj.name.StartsWith("Rename"))
        {
            HandleRenameClick(selectedObj);
            return;
        }

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroops").GetComponent<TextMeshProUGUI>();

        switch (selectedObj.name)
        {
            case string s when s.StartsWith("country"):
                CameraHandler.DisableMovement = true;
                Country country = countryMap[selectedObj.GetComponent<Button>()];

                if (turnPlayer != country.GetOwner()) return;
                attacker = country;

                GameObject.Find("RemainingDistribution").GetComponent<TextMeshProUGUI>().text = $"Troops Left To Deploy: {this.turnPlayer.GetNumberOfTroops()}";
                DistributeCanvas.enabled = true;
                return;

            case "CardInventoryButton":
                this.turnPlayer.LoadSlot();
                this.turnPlayer.LoadTrade();
                this.CardInventory.enabled = true;
                return;

            case "Confirm":
                CameraHandler.DisableMovement = false;

                int num = Int32.Parse(numberOfTroops.GetComponent<TextMeshProUGUI>().text);
                this.attacker.ChangeTroops(num);
                this.turnPlayer.ChangeNumberOfTroops(-num);

                GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                Killfeed.Update($"{this.turnPlayer.GetName()}: sent {num} troop(s) to {attacker.GetName()}");

                this.attacker = null;
                this.DistributeCanvas.enabled = false;
                numberOfTroops.text = "1";

                if (turnPlayer.GetNumberOfTroops() > 0) return;

                // phase changing to attack
                this.currentPhase.text = "attack phase";
                HandleObjectClick = AttackPhase;
                GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = false;
                GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = false;
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

    /// <summary>
    /// <c>AttackPhase</c> handles clicks in the attacking phase of the turn.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    public void AttackPhase(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        if (AttackCanvas.enabled)
        {
            Debug.Log("clause attack");
            HandleAttackClick(selectedObj);
            return;
        }

        if (DefendCanvas.enabled)
        {
            Debug.Log("clause defend");
            HandleDefendClick(selectedObj);
            return;
        }

        if (TransferCanvas.enabled)
        {
            Debug.Log("clause transfer");
            HandleTransferClick(selectedObj);
            return;
        }

        if (selectedObj.name.StartsWith("Rename"))
        {
            HandleRenameClick(selectedObj);
            return;
        }

        // ends the players attack phase
        if (selectedObj.name == "EndPhase")
        {
            this.UnHighlight();
            this.currentPhase.text = "fortify phase";
            HandleObjectClick = FortifyPhase;
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
            HighlightEnemy(countrySelected);
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

        if (clickedCountry == attacker)
        {
            this.UnHighlight();
            return;
        }

        //if clicked country is unattackable, unhighlights and returns
        if (considered.Contains(clickedCountry))
        {
            this.AttackCanvas.enabled = true;
            CameraHandler.DisableMovement = true;
            GameObject.Find("RemainingAttack").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Attack: {attacker.GetTroops() - 1}\r\n(choose how many dice to roll)";
            this.defender = clickedCountry;
        }
        return;
    }

    /// <summary>
    /// <c>FortifyPhase</c> handles clicks in the phase after attack where players can fortify a country.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    public void FortifyPhase(GameObject selectedObj)
    {
        if (TransferCanvas.enabled)
        {
            HandleFortifyClick(selectedObj);
            return;
        }

        if (selectedObj.name.StartsWith("Rename"))
        {
            HandleRenameClick(selectedObj);
            return;
        }

        if (selectedObj == null)
        {
            this.UnHighlight();
            return;
        }

        // ends the players fortify phase
        if (selectedObj.name == "EndPhase")
        {
            this.UnHighlight();
            this.currentPhase.text = "draft phase";
            HandleObjectClick = DraftPhase;
            GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = true;
            GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = true;
            NextTurn();
            this.turnPlayer.GetNewTroopsAndCards();
            this.turnPlayer.InitializeSlot();
            return;
        }

        // this handles Highlighting (if nothing is highlighted)
        if (attacker == null)
        {
            if (!selectedObj.name.StartsWith("country")) return;
            Country countrySelected = countryMap[selectedObj.GetComponent<Button>()];

            // handles the case where the current player clicks a different player's country
            if (turnPlayer != countrySelected.GetOwner()) return;

            // handles the case where you have no troops to transfer
            if (countrySelected.GetTroops() < 2) return;

            attacker = countrySelected;
            Debug.Log("about to run");
            HighlightConnectedCountries();
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

        if (clickedCountry == attacker)
        {
            this.UnHighlight();
            return;
        }

        if (considered.Contains(clickedCountry))
        {
            this.defender = clickedCountry;
            this.TransferCanvas.enabled = true;
            GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Transfer: {attacker.GetTroops() - 1}"; ;
            CameraHandler.DisableMovement = true;
        }
        return;
    }

    /// <summary>
    /// <c>HandleCardClick</c> handles clicks in the card inventory screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleCardClick(GameObject selectedObj)
    {
        switch (selectedObj.name)
        {
            case "Trade":
                if (this.turnPlayer.Trade())
                {
                    GameObject.Find("TradeResultText").GetComponent<TextMeshProUGUI>().text = "Trade was successful. You gained 6 troops.";
                    GameObject.Find("TradeResultText").GetComponent<TextMeshProUGUI>().color = Color.green;
                }
                else
                {
                    GameObject.Find("TradeResultText").GetComponent<TextMeshProUGUI>().text = "Trade was unsuccessful. You have to trade one of each type or three of a type.";
                    GameObject.Find("TradeResultText").GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                GameObject.Find("TradeResult").GetComponent<Canvas>().enabled = true;

                Wait.Start(2f, () =>
                {
                    GameObject.Find("TradeResult").GetComponent<Canvas>().enabled = false;
                });
                return;
            case "CardInventoryButtonClose":
                this.CardInventory.enabled = false;
                this.turnPlayer.Cancel();
                GameObject.Find("CardNotification").GetComponent<Image>().enabled = false;
                return;

            case "slot1":
            case "slot2":
            case "slot3":
            case "slot4":
            case "slot5":
            case "slot6":
                Debug.Log("came slot");
                this.turnPlayer.SelectForTrade(selectedObj.name);
                return;

            case "trade1":
            case "trade2":
            case "trade3":
                Debug.Log("came trade");
                this.turnPlayer.RemoveForTrade(selectedObj.name);
                return;
            case "NextCard":
                this.turnPlayer.Next();
                return;
            default: return;
        }
    }

    /// <summary>
    /// <c>HandleRenameClick</c> handles clicks in the rename country screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleRenameClick(GameObject selectedObj)
    {
        switch (selectedObj.name)
        {
            case "RenameCountryButton":
                GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled = true;
                break;

            case "RenameConfirm":
                string from = GameObject.Find("RenameCountryFrom").GetComponent<TMP_InputField>().text.FirstCharacterToUpper();
                string to = GameObject.Find("RenameCountryTo").GetComponent<TMP_InputField>().text.FirstCharacterToUpper();

                Country foundCountry = null;
                string[] names = new string[countryMap.Count];
                int i = 0;

                foreach (var kvp in countryMap)
                {
                    string name = kvp.Value.GetName();
                    names[i] = name;
                    i++;
                    if (to.ToLower().Equals(name.ToLower()))
                    {
                        Killfeed.Update($"Country '{to}' already exists");
                        GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled = false;
                        GameObject.Find("RenameCountryFrom").GetComponent<TMP_InputField>().text = "";
                        GameObject.Find("RenameCountryTo").GetComponent<TMP_InputField>().text = "";
                        return;
                    }

                    if (name.ToLower().Equals(from.ToLower())) foundCountry = kvp.Value;
                }

                if (foundCountry != null)
                {
                    foundCountry.SetName(to);
                    Killfeed.Update($"'{from}' was renamed to '{to}'");
                }
                else
                {
                    Killfeed.Update($"Country '{from}' does not exist");
                }

                GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled = false;
                GameObject.Find("RenameCountryFrom").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("RenameCountryTo").GetComponent<TMP_InputField>().text = "";
                return;

            case "RenameCancel":
                GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled = false;
                return;

            default: return;
        }
    }

    /// <summary>
    /// <c>HandleAttackClick</c> handles clicks in the attacking screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleAttackClick(GameObject selectedObj)
    {
        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToSend").GetComponent<TextMeshProUGUI>();
        int attacker_num = Int32.Parse(numberOfTroops.text);

        switch (selectedObj.name)
        {
            case "Confirm":
                this.AttackCanvas.enabled = false;

                if (this.defender.GetTroops() > 1)
                {
                    numberOfTroops.text = "1";
                    AttackCanvas.enabled = false;
                    DefendCanvas.enabled = true;
                    DefendCanvas.transform.Find("RemainingDefend").GetComponent<TextMeshProUGUI>().text = $"Attacker deployed: {attacker_num} \nTroops Available For Defense: 2\n(choose how many dice to roll)";
                    return;
                }

                if (!this.Attack(attacker, defender, attacker_num, 1)) return;

                if (attacker.GetOwner() == defender.GetOwner())
                {
                    if (attacker.GetTroops() > 1)
                    {
                        Wait.Start(3f, () =>
                        {
                            this.TransferCanvas.enabled = true;
                            GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Transfer: {recentFight[0].GetTroops() - 1}"; ;
                        });
                    }
                    else recentFight = new Country[] { null, null };
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
                if (attacker_num == 3 || attacker_num == attacker.GetTroops() - 1) return;

                attacker_num++;
                numberOfTroops.text = "" + attacker_num;
                return;

            case "ButtonMinus":
                if (attacker_num == 1) return;

                attacker_num--;
                numberOfTroops.text = "" + attacker_num;
                return;

            default: return;
        }
    }

    /// <summary>
    /// <c>HandleAttackClick</c> handles clicks in the troop defend screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleDefendClick(GameObject selectedObj)
    {
        int attacker_num = Int32.Parse(GameObject.Find("RemainingDefend").GetComponent<TextMeshProUGUI>().text.Split("\n")[0].Substring(18));
        TextMeshProUGUI defender_text = GameObject.Find("NumberOfTroopsToDefend").GetComponent<TextMeshProUGUI>();

        switch (selectedObj.name)
        {
            case "Confirm":
                int defender_num = Int32.Parse(defender_text.text);
                this.DefendCanvas.enabled = false;
                defender_text.text = "1";
                if (!this.Attack(attacker, defender, attacker_num, defender_num)) return;

                if (attacker.GetOwner() == defender.GetOwner())
                {
                    if (attacker.GetTroops() > 1)
                    {
                        Wait.Start(3f, () =>
                        {
                            this.TransferCanvas.enabled = true;
                            GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>().text = $"Troops Available For Transfer: {recentFight[0].GetTroops() - 1}"; ;
                        });
                    }
                    else recentFight = new Country[] { null, null };
                }

                this.UnHighlight();
                CameraHandler.DisableMovement = false;
                return;

            case "ButtonPlus":
            case "ButtonMinus":
                if (Int32.Parse(defender_text.text) == 1) defender_text.text = "2";
                else if (Int32.Parse(defender_text.text) == 2) defender_text.text = "1";
                return;

            default: return;
        }
    }

    /// <summary>
    /// <c>HandleTransferClick</c> handles clicks in the transferring of troops screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleTransferClick(GameObject selectedObj)
    {
        int available = recentFight[0].GetTroops() - 1;

        TextMeshProUGUI troopsLeft = GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>();
        troopsLeft.text = $"Troops Available For Transfer: {available}";

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToTransfer").GetComponent<TextMeshProUGUI>();
        int num = Int32.Parse(numberOfTroops.text);

        switch (selectedObj.name)
        {
            case "Confirm":
                this.TransferCanvas.enabled = false;

                this.Transfer(recentFight[0], recentFight[1], num); // transfer num troops to new country

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

    /// <summary>
    /// <c>HandleFortifyClick</c> handles clicks in the country fortification screen.
    /// </summary>
    /// <param name="selectedObj">The object the user has clicked.</param>
    private void HandleFortifyClick(GameObject selectedObj)
    {
        if (selectedObj == null) return;

        int available = attacker.GetTroops() - 1;

        TextMeshProUGUI troopsLeft = GameObject.Find("AvailableForTransfer").GetComponent<TextMeshProUGUI>();
        troopsLeft.text = $"Troops Available For Transfer: {available}";

        TextMeshProUGUI numberOfTroops = GameObject.Find("NumberOfTroopsToTransfer").GetComponent<TextMeshProUGUI>();
        int num = Int32.Parse(numberOfTroops.text);

        switch (selectedObj.name)
        {
            case "Confirm":
                this.TransferCanvas.enabled = false;
                this.Transfer(this.attacker, this.defender, num); // transfer num troops to new country
                this.UnHighlight();
                NextTurn();
                this.turnPlayer.GetNewTroopsAndCards();
                this.turnPlayer.InitializeSlot();
                this.currentPhase.text = "draft phase";
                HandleObjectClick = DraftPhase;
                GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = true;
                GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = true;
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
            case "EndPhase":
                this.TransferCanvas.enabled = false;
                this.UnHighlight();
                NextTurn();
                this.turnPlayer.GetNewTroopsAndCards();
                this.turnPlayer.InitializeSlot();
                this.currentPhase.text = "draft phase";
                HandleObjectClick = DraftPhase;
                GameObject.Find("CardInventoryButton").GetComponent<Image>().enabled = true;
                GameObject.Find("CardInventoryButton").GetComponent<Button>().enabled = true;
                numberOfTroops.text = "1";
                return;

            default: return;
        }
    }

    /// <summary>
    /// <c>HighlightEnemy</c> changes the color of all enemies that you can attack.
    /// </summary>
    /// <param name="country">The country that has enemies you want to highlight.</param>
    public void HighlightEnemy(Country country)
    {
        this.attacker = country;
        this.considered = country.HighlightEnemyNeighbors();
        return;
    }

    /// <summary>
    /// <c>HighlightConnectedCountries</c> recurses through all connected countries to find which ones can be fortified.
    /// </summary>
    private void HighlightConnectedCountries()
    {
        List<Country> visited = new List<Country>();
        Action<List<Country>, Country> recurse = null;
        recurse = (visited, country) =>
        {
            visited.Add(country);
            country.TempColorChange(Color.white);

            foreach (Country neighbor in country.GetNeighbors())
            {
                if (neighbor.GetOwner() != attacker.GetOwner() || visited.Contains(neighbor)) continue;
                recurse(visited, neighbor);
            }
        };

        recurse(visited, this.attacker);
        this.attacker.TempColorChange(Color.grey);
        this.considered = visited;
    }

    /// <summary>
    /// <c>UnHighlight</c> reverses any color changes that happened during attack or fortify.
    /// </summary>
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

    /// <summary>
    /// <c>Attack</c> handles the whole attack after the users have chosen countries and troops. 
    /// Rolls dice and shows outcome of fight.
    /// </summary>
    /// <param name="attacker">The country that is going to attack.</param>
    /// <param name="defender">The country that is going to defend.</param>
    /// <param name="num">The number of troops that are going to attack.</param>
    /// <param name="defender_num">The number of troops that are going to defend.</param>
    public bool Attack(Country attacker, Country defender, int num, int defender_num)
    {

        List<int> atkRolls = new List<int>();
        List<int> defRolls = new List<int>();

        for (int i = 1; i <= num; i++)
        {
            int roll = Dice.Roll();
            atkRolls.Add(roll);
        }

        for (int i = 1; i <= defender_num; i++)
        {
            int roll = Dice.Roll();
            defRolls.Add(roll);
        }

        atkRolls.Sort();
        defRolls.Sort();

        atkRolls.Reverse();
        defRolls.Reverse();

        for (int i = 0; i < atkRolls.Count; i++) GameObject.Find($"AttackerDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().text = $"Attacker Rolls - {atkRolls[i]}";
        for (int i = 0; i < defRolls.Count; i++) GameObject.Find($"DefenderDiceRoll{i + 1}").GetComponent<TextMeshProUGUI>().text = $"Defender Rolls - {defRolls[i]}";

        if (atkRolls.Count > defRolls.Count) atkRolls.RemoveRange(defRolls.Count, atkRolls.Count - defRolls.Count);
        else defRolls.RemoveRange(atkRolls.Count, defRolls.Count - atkRolls.Count);

        int atkWins = 0;
        int atkLosses = 0;

        for (int i = 0; i < Math.Min(atkRolls.Count, defRolls.Count); i++)
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
        }
        // fights have ended

        string s = $"Attacker Lost {atkLosses} Troop(s)!\nDefender Lost {atkWins} Troop(s)!";
        GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = s;
        Killfeed.Update($"{turnPlayer.GetName()}: attacking {defender.GetName()} (↓{atkWins})");

        if (defender.GetTroops() == 0)
        {
            Player defending_player = defender.GetOwner();
            if (attacker.GetOwner().GetCountries().Count + 1 == ListOfCountries.Count)
            {
                Debug.Log("came to game finished");
                eliminatedPlayers.Add(defending_player);
                ShowRanking();
                return false;
            }

            Debug.Log("came after if block");






            Killfeed.Update($"{turnPlayer.GetName()}: now owns {defender.GetName()}");
            GameObject.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = $"You Successfully Invaded!";
            defending_player.RemoveCountry(defender);
            defender.SetOwner(attacker.GetOwner());
            defender.ChangeTroops(num);
            attacker.ChangeTroops(-num);
            this.turnPlayer.gain_card = true;

            recentFight[0] = attacker;
            recentFight[1] = defender;

            if (defending_player.GetCountries().Count == 0)
            {
                eliminatedPlayers.Add(defending_player);
                EliminatePlayer();
            }
        }
        GameObject.Find("SoundConquer").GetComponent<AudioSource>().Play();


        if (turnPlayer is not AIPlayer) this.DiceCanvas.enabled = true;

        Wait.Start(3f, () =>
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

        return true;
    }

    /// <summary>
    /// <c>Transfer</c> moves <c>num</c> troops from a country to another country.
    /// </summary>
    /// <param name="from">The country that is going to send the troops.</param>
    /// <param name="to">The country that is going to receive the troops.</param>
    /// <param name="num">The number of troops that are going to be sent.</param>
    public void Transfer(Country from, Country to, int num)
    {
        from.ChangeTroops(-num);
        to.ChangeTroops(num);

        GameObject.Find("SoundTransfer").GetComponent<AudioSource>().Play();
        Killfeed.Update($"{turnPlayer.GetName()}: sent {num} troop(s) from {from.GetName()} to {to.GetName()}");
    }

    /// <summary>
    /// <c>NextTurn</c> finds the next player that can take a turn and switches to them.
    /// </summary>
    public void NextTurn()
    {
        if (this.turnPlayer.GetNumberOfOwnedCountries() == this.countryMap.Count)
        {
            Debug.Log("winner winner chicken dinner!!!");
        }

        this.turnIndex++;

        if (this.turnIndex > (this.turnsOrder.Count - 1)) this.turnIndex = 0;

        turnPlayer = turnsOrder[turnIndex];
        if (turnPlayer.GetNumberOfOwnedCountries() == 0
            && !HandleObjectClick.GetMethodInfo().Name.Equals("SetupPhase")
            && !HandleObjectClick.GetMethodInfo().Name.Equals("SetupDeployPhase"))
        {
            Killfeed.Update($"{turnPlayer.GetName()}: skipped as they are no longer in the game");
            NextTurn(); // ignores players who have lost
        }
        currentPlayerName.GetComponent<TextMeshProUGUI>().text = "playing:\n" + this.GetTurnsName();
        currentPlayerColor.GetComponent<Image>().color = GetTurnsColor();

        if (turnPlayer is AIPlayer)
        {
            DisableButtons();
            turnPlayer.TakeTurn();
        }
        else EnableButtons();
    }

    /// <summary>
    /// <c>ResetTurn</c> goes back to the first player's turn.
    /// </summary>
    public void ResetTurn()
    {
        turnIndex = 0;
        turnPlayer = turnsOrder[0];
        currentPlayerName.GetComponent<TextMeshProUGUI>().text = "playing:\n" + this.GetTurnsName();
        currentPlayerColor.GetComponent<Image>().color = GetTurnsColor();
        EnableButtons();
    }

    /// <summary>
    /// <c>GetTurnsName</c> finds the name of the current player.
    /// </summary>
    /// <returns>
    /// The name of the currently playing player.
    /// </returns>
    public string GetTurnsName() => turnPlayer.GetName();

    /// <summary>
    /// <c>GetTurnsColor</c> finds the color of the current player.
    /// </summary>
    /// <returns>
    /// The color of the currently playing player.
    /// </returns>
    public Color GetTurnsColor() => turnPlayer.GetColor();

    /// <summary>
    /// <c>EnableButtons</c> allows the player to click all buttons on the screen when it is their turn.
    /// </summary>
    private void EnableButtons()
    {
        foreach (var kvp in countryMap)
        {
            kvp.Key.enabled = true; // make buttons clickable again
        }
        GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
        GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
    }

    /// <summary>
    /// <c>DisableButtons</c> stops the player from clicking buttons when it is not their turn.
    /// </summary>
    private void DisableButtons()
    {
        foreach (var kvp in countryMap)
        {
            kvp.Key.enabled = false; // make buttons unclickable so the AI is not bugged
        }
        GameObject.Find("EndPhase").GetComponent<Image>().enabled = false;
        GameObject.Find("EndPhase").GetComponent<Button>().enabled = false;
    }

    /// <summary>
    /// <c>EliminatePlayer</c> shows a screen with information of the recently eliminated player.
    /// </summary>
    private void EliminatePlayer()
    {
        GameObject.Find("EliminatedColour").GetComponent<Image>().color = eliminatedPlayers[eliminatedPlayers.Count - 1].GetColor();
        GameObject.Find("EliminatedUsername").GetComponent<TextMeshProUGUI>().text = eliminatedPlayers[eliminatedPlayers.Count - 1].GetName();
        GameObject.Find("PlayerEliminated").GetComponent<Canvas>().enabled = true;
        Wait.Start(3f, () =>
        {
            GameObject.Find("PlayerEliminated").GetComponent<Canvas>().enabled = false;
        });
    }

    /// <summary>
    /// <c>ShowRanking</c> shows the order of which all players were eliminated for the win screen of the game.
    /// </summary>
    private void ShowRanking()
    {
        eliminatedPlayers.Reverse();
        List<string> places = new List<string>() { "2nd", "3rd", "4th", "5th", "6th" };
        string ranking = "";
        for (int i = 0; i < eliminatedPlayers.Count; i++) ranking += $"{places[i]}: {eliminatedPlayers[i].GetName()}\n";
        Win.eliminatedList = ranking;
        Win.username = turnPlayer.GetName();
        Win.winnerColor = turnPlayer.GetColor();
        SceneManager.LoadScene("assets/scenes/scenewin.unity");
    }
}