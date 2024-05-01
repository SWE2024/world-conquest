using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    [SerializeField] Canvas cardInventory;

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

        switch (Preferences.MapNumber)
        {
            case 1: numberOfCountries = 44; ListOfNeighbours = Map1.ListOfNeighbours; break;
            case 2: numberOfCountries = 27; ListOfNeighbours = Map2.ListOfNeighbours; break;
            case 3: numberOfCountries = 6; ListOfNeighbours = Map3.ListOfNeighbours; break;
        }

        //initializes the gamestate instance which is singleton
        game = GameController.New(Preferences.PlayerCount, troopDistribution, troopAttack, troopDefend, troopTransfer, diceCanvas, cardInventory);
        

        //this is map to get the country instance that holds the button that is clicked
        Dictionary<Button, Country> countryMap = new Dictionary<Button, Country>();

        for (int i = 1; i <= numberOfCountries; i++)
        {
            // creates the country objects
            string name = "";
            switch (Preferences.MapNumber)
            {
                case 1: name = Map1.CountryNameMap[i]; break;
                case 2: name = Map2.CountryNameMap[i]; break;
                case 3: name = Map3.CountryNameMap[i]; break;
            }

            GameObject obj = GameObject.Find($"country{i}map{Preferences.MapNumber}");
            Button button = obj.GetComponent<Button>();
            Country country = new Country(button, name);

            // adds it to hashmap and the gamestate's country list
            countryMap.Add(button, country);
            game.ListOfCountries.Add(country);

            // makes the country clickable
            obj.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

            // show the connections for the map
            GameObject.Find($"connectionsmap{Preferences.MapNumber}").GetComponent<Image>().enabled = true;
        }

        if (Preferences.MapNumber != 1)
        {
            for (int i = 1; i <= 44; i++)
            {
                GameObject go = GameObject.Find($"country{i}map1");
                go.GetComponent<Image>().enabled = false;
                go.GetComponent<Button>().enabled = false;
                foreach (Transform child in go.transform)
                {
                    child.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            GameObject.Find($"connectionsmap1").GetComponent<Image>().enabled = false;
        }
        if (Preferences.MapNumber != 2)
        {
            for (int i = 1; i <= 27; i++)
            {
                GameObject go = GameObject.Find($"country{i}map2");
                go.GetComponent<Image>().enabled = false;
                go.GetComponent<Button>().enabled = false;
                foreach (Transform child in go.transform)
                {
                    child.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            GameObject.Find($"connectionsmap2").GetComponent<Image>().enabled = false;
        }
        if (Preferences.MapNumber != 3)
        {
            for (int i = 1; i <= 6; i++)
            {
                GameObject go = GameObject.Find($"country{i}map3");
                go.GetComponent<Image>().enabled = false;
                go.GetComponent<Button>().enabled = false;
                foreach (Transform child in go.transform)
                {
                    child.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            GameObject.Find($"connectionsmap3").GetComponent<Image>().enabled = false;
        }

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

        string path = $"cards/map{Preferences.MapNumber}";

        Sprite[] allSpriteAssets = Resources.LoadAll<Sprite>(path);
        foreach (Sprite s in allSpriteAssets)
        {
            string name;
            string card_type;
            if (s.name == "wildcard")
            {
                name = null;
                card_type = "wildcard";
            }
            else
            {
                name = s.name.Split("_")[0];
                card_type = s.name.Split("_")[1];
            }

            Country country = null;
            foreach (Country country_ins in game.ListOfCountries)
            {
                if (country_ins.GetName() == name)
                {
                    country = country_ins;
                    break;
                }
            }
            Card c = new Card(country, card_type, s);
            GameController.ListOfCards.Add(c);
        }

        for (int i = 1; i <= 3; i++)
        {
            GameObject.Find($"trade{i}").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }

        for (int i = 1; i <= 6; i++)
        {
            GameObject.Find($"slot{i}").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
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