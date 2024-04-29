using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    static List<List<int>> ListOfNeighbours;

    GameController game;

    [SerializeField] Canvas troopDistribution;
    [SerializeField] Canvas troopAttack;
    [SerializeField] Canvas troopDefend;
    [SerializeField] Canvas troopTransfer;
    [SerializeField] Canvas diceCanvas;

    /* unused
    void AutoPopulate()
    {
        //this is the list of distributed colors which will be Randomly picked
        List<Color> playerColors = game.GenerateListOfColors();

        for (int i = 0; i < playerColors.Count; i++)
        {
            //Randomly selected index to pop a color
            int index = GameController.Random.Next(playerColors.Count);
            Color color = playerColors[index];
            playerColors.RemoveAt(index);

            Country country = game.ListOfCountries[i];
            country.Pointer.GetComponent<Image>().color = color;

            // sets the number of Troops above the country
            TextMeshProUGUI numberTroopsText = GameObject.Find($"country{i + 1}map1").GetComponentInChildren<TextMeshProUGUI>();
            numberTroopsText.text = $"{country.GetTroops()}";
        }
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        int numberOfCountries = 0;
        int otherMap = 0;
        int otherCountries = 0;

        if (Preferences.MapNumber == 1) { numberOfCountries = 44; otherCountries = 27; otherMap = 2; ListOfNeighbours = Map1.ListOfNeighbours; }
        if (Preferences.MapNumber == 2) { numberOfCountries = 27; otherCountries = 44; otherMap = 1; ListOfNeighbours = Map2.ListOfNeighbours; }

        //initializes the gamestate instance which is singleton
        game = GameController.New(Preferences.PlayerCount, troopDistribution, troopAttack, troopDefend, troopTransfer, diceCanvas);

        //this is map to get the country instance that holds the button that is clicked
        Dictionary<Button, Country> countryMap = new Dictionary<Button, Country>();

        for (int i = 1; i <= numberOfCountries; i++)
        {
            // creates the country objects
            string name = "";
            if (Preferences.MapNumber == 1) name = (Map1.CountryNameMap[i]);
            else name = (Map2.CountryNameMap[i]);

            GameObject obj = GameObject.Find($"country{i}map{Preferences.MapNumber}");
            Button button = obj.GetComponent<Button>();
            Country country = new Country(button, name);

            // adds it to hashmap and the gamestate's country list
            countryMap.Add(button, country);
            game.ListOfCountries.Add(country);

            // makes the country clickable
            obj.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }

        for (int i = 1; i <= otherCountries; i++)
        {
            // disables the other map's elements
            GameObject obj = GameObject.Find($"country{i}map{otherMap}");

            obj.GetComponent<Button>().enabled = false;
            obj.GetComponent<Image>().enabled = false;
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
        }

        GameObject.Find($"connectionsmap{Preferences.MapNumber}").GetComponent<Image>().enabled = true;
        GameObject.Find($"connectionsmap{otherMap}").GetComponent<Image>().enabled = false;

        // sets Neighbors for each country 
        for (int i = 0; i < game.ListOfCountries.Count; i++)
        {
            List<Country> neighbors = new List<Country>();

            foreach (int index in ListOfNeighbours[i])
            {
                neighbors.Add(game.ListOfCountries[index - 1]);
            }

            game.ListOfCountries[i].SetNeighbors(neighbors);
        }

        game.SetCountryMap(countryMap);

        troopDistribution.enabled = false;
        troopAttack.enabled = false;
        troopTransfer.enabled = false;
        diceCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            game.HandleObjectClick(selectedObj);
        }
    }
}