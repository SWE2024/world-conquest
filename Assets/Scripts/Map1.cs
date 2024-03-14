using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map1 : MonoBehaviour
{
    public static int PlayerCount; // set before this scene loads
    public static bool AutoPopulateFlag = false; // sees if the setup phase should be skipped
    private static List<List<int>> ListOfNeighbours = Map1Neighbours.ListOfNeighbours;

    private GameState game;
    [SerializeField] Canvas troopDistribution;
    [SerializeField] Canvas troopAttack;
    [SerializeField] Canvas diceCanvas;

    void AutoPopulate()
    {
        //this is the list of distributed colors which will be Randomly picked
        List<Color> listOfColors = game.GenerateListOfColors();

        for (int i = 0; i < listOfColors.Count; i++)
        {
            //Randomly selected index to pop a color
            int index = GameState.Random.Next(listOfColors.Count);
            Color color = listOfColors[index];
            listOfColors.RemoveAt(index);

            Country country = game.ListOfCountries[i];
            country.Pointer.GetComponent<Image>().color = color;

            // sets the number of Troops above the country
            TextMeshProUGUI numberTroopsText = GameObject.Find($"country{i + 1}map1").GetComponentInChildren<TextMeshProUGUI>();
            numberTroopsText.text = $"{country.GetTroops()}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        troopDistribution.enabled = false;
        troopAttack.enabled = false;
        diceCanvas.enabled = false;

        Debug.Log($"EVENT: STARTING GAME: there are {PlayerCount} players");

        //initializes the gamestate instance which is singleton
        game = GameState.New(Map1.PlayerCount, troopDistribution, troopAttack, diceCanvas);

        //this is map to get the country instance that holds the button that is clicked
        Dictionary<Button, Country> countryMap = new Dictionary<Button, Country>();

        for (int i = 1; i < 45; i++)
        {
            //gets the button
            Button button = GameObject.Find($"country{i}map1").GetComponent<Button>();
            Country country = new Country(button);

            //adds it to hashmap and the gamestate's country list
            countryMap.Add(button, country);
            game.ListOfCountries.Add(country);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            game.HandleCountryClick(selectedObj);
        }
    }
}