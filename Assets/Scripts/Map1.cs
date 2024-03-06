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

    //the scene holds a reference to the gamestate object
    GameState game;

    // list of neighbors
    static List<List<int>> list_of_neighbors = Map1Neighbours.list_of_neighbors;

    public static bool auto_populate_flag = false;

    [SerializeField] Canvas user_input;

    void auto_populate(){
        //this is the list of distributed colors which will be randomly picked
        List<Color> list_of_colors = game.generate_list_of_colors();

        for(int i = 0; i < list_of_colors.Count; i++) 
        {
            //randomly selected index to pop a color
            int index = GameState.random.Next(list_of_colors.Count);
            UnityEngine.Color color = list_of_colors[index];
            list_of_colors.RemoveAt(index);

            Country country = game.list_of_countries[i];
            country.pointer.GetComponent<Image>().color = color;
            // country.color = color;

            // sets the number of troops above the country
            TextMeshProUGUI numberTroopsText = GameObject.Find($"country{i + 1}").GetComponentInChildren<TextMeshProUGUI>();
            numberTroopsText.text = $"{country.get_troops()}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {   
        user_input.enabled = false;

        Debug.Log($"starting with {playerCount} players");

        //initializes the gamestate instance which is singleton
        game = GameState.New(Map1.playerCount, user_input);

        //this is map to get the country instance that holds the button that is clicked
        Dictionary<Button, Country> country_map = new Dictionary<Button, Country>();

        for (int i = 1; i < 45; i++)
        {
            //gets the button
            Button button = GameObject.Find($"country{i}").GetComponent<Button>();
            Country country = new Country(button);
            country.set_troops(0);

            //adds it to hashmap and the gamestate's country list
            country_map.Add(button, country);
            game.list_of_countries.Add(country);
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
        
        game.set_hashmap(country_map);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

            // Debug.Log($"{selectedObj.name}");

            game.Handle_Country_Click(selectedObj);

            // if (selectedObj == null) game.Handle_Country_Click(null);
            // else if (selectedObj.name.StartsWith("country"))


            // if (selectedObj != null)
            // {
            //     Button selectedBtn = GameObject.Find(selectedObj.name).GetComponent<Button>();

            //     if (selectedBtn != null)
            //     {
            //         Country country = country_map[selectedBtn];

            //         game.Handle_Country_Click(country);
            //     }
            // }
        }
    }
}