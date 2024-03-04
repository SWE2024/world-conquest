using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map1 : MonoBehaviour
{
    //number of players, this gets set before this scene loads by the previous scene
    public static int playerCount;

    //this is map to get the country instance that holds the button that is clicked
    Dictionary<Button, Country> country_map = new Dictionary<Button, Country>();

    //the scene holds a reference to the gamestate object
    GameState game;

    // list of neighbors
    static List<List<int>> list_of_neighbors = Map1Neighbours.list_of_neighbors;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"starting with {playerCount} players");

        //initializes the gamestate instance which is singleton
        game = GameState.New(Map1.playerCount);

        //this is the list of distributed colors which will be randomly picked
        List<Color> list_of_colors = game.generate_list_of_colors();

        //loop to create country instances set color randomly add them to the country list        
        for (int i = 1; i < 45; i++)
        {
            //gets the button
            Button button = GameObject.Find($"country{i}").GetComponent<Button>();

            //randomly selected index to pop a color
            int index = GameState.random.Next(list_of_colors.Count);
            UnityEngine.Color color = list_of_colors[index];
            list_of_colors.RemoveAt(index);

            // sets the button color and create a country instance with it
            button.GetComponent<Image>().color = color;
            Country country = new Country(button, color);

            // sets the number of troops above the country
            TextMeshProUGUI numberTroopsText = GameObject.Find($"country{i}").GetComponentInChildren<TextMeshProUGUI>();
            numberTroopsText.text = $"{country.get_troops()}";

            //adds it to hashmap and the gamestate's country list
            country_map.Add(button, country);
            game.list_of_countries.Add(country);
        }

        if (game.list_of_countries.Count != Map1Neighbours.list_of_neighbors.Count)
        {
            throw new Exception("list sizes do not match");
        }
        else
        {
            Debug.Log("list sizes do match");
        }

        // sets neighbors for each country 
        for (int i = 0; i < game.list_of_countries.Count; i++)
        {
            List<Country> neighbors = new List<Country>();

            foreach (int index in list_of_neighbors[i])
            {
                neighbors.Add(game.list_of_countries[index - 1]);
            }

            game.list_of_countries[i].set_neighbors(neighbors);
        }
        // game.set_countries(countries);


        // create a class to have methods to manipulate a few fields of gamestate:
        // 1. list of countries
        // 2. square to show turns
        // 3. turns_order to iterate the turns
        // 4. change the color field of the country instance 
        // 5. access the button through the country instance to actually change the color


        //after setup function executes the only difference is the the country buttons actual color the and the 
        // country instance's color field

        
        //the ui when you select the country is confirm and cancel 
        // you have to track of when a country is selected so switch between highlighted and not highlighted
        // keep track of which country instance is already taken, you can do this by its color field if its white or not


        

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

            if (selectedObj != null)
            {
                Button selectedBtn = GameObject.Find(selectedObj.name).GetComponent<Button>();

                if (selectedBtn != null)
                {
                    Country country = country_map[selectedBtn];

                    game.take_country_click(country);
                }
            }
        }
    }
}