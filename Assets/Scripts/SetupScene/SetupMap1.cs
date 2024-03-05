using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupMap1 : MonoBehaviour
{
    // number of players, this gets set before this scene loads by the previous scene
    public static int playerCount;

    // number of countries that have been claimed
    public static int numberOfClaims = 0;

    // the scene holds a reference to the gamestate object
    GameState game;

    // the list of colors chosen in setup phase
    List<Color> list;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"starting with {playerCount} players");

        //initializes the gamestate instance which is singleton
        game = GameState.New(SetupMap1.playerCount);

        list = new List<Color>(44);

        for (int i = 0; i < 44; i++)
        {
            list.Add(Color.white);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfClaims == 44)
        {
            game.reset_turn(); // ensures the game starts with the first player
            Map1.playerCount = playerCount;
            Map1.list_of_colors = list;
            SceneManager.LoadScene("assets/scenes/scenegame.unity");
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            
            if (selectedObj != null)
            {
                Button selectedCountry = selectedObj.GetComponent<Button>();
                if (selectedCountry.GetComponent<Image>().color == Color.white)
                {
                    // take the unowned country
                    numberOfClaims++;
                    print(numberOfClaims);
                    selectedCountry.GetComponent<Image>().color = game.turn_color;
                    int countryNumber = Int32.Parse(selectedCountry.name.Substring(7));
                    list[countryNumber - 1] = game.turn_color;
                    game.next_turn();
                }
            }
        }
    }
}