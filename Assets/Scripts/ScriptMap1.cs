using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScriptMap1 : MonoBehaviour
{
    public static int playerCount;
    Dictionary<Button, Country> country_map = new Dictionary<Button, Country>();

    GameState game;
    List<Country> countries = new List<Country>();
    List<List<int>> list_of_neighbors = new List<List<int>> {
        new List<int> {
            2, 3, 37
        },
        new List<int> {
            1, 3, 4, 9
        },
        new List<int> {
            1, 2, 4, 6
        },
        new List<int> {
            2, 3, 5, 6, 7, 9
        },
        new List<int> {
            4, 7, 9
        },
        new List<int> {
            3, 4, 7, 8
        },
        new List<int> {
            4, 5, 6, 8
        },
        new List<int> {
            6, 7, 10
        },
        new List<int> {
            2, 4, 5, 14
        },
        new List<int> {
            8, 11, 12
        },
        new List<int> {
            10, 12, 21
        },
        new List<int> {
            10, 11, 13
        },
        new List<int> {
            11, 12
        },
        new List<int> {
            9, 15, 16
        },
        new List<int> {
            14, 16, 18, 19
        },
        new List<int> {
            14, 15, 17, 18
        },
        new List<int> {
            16, 18, 20, 27, 28, 29
        },
        new List<int> {
            15, 16, 17, 19, 20
        },
        new List<int> {
            15, 18, 20, 21
        },
        new List<int> {
            17, 18,19, 22, 29
        },
        new List<int> {
            11, 19, 22, 23, 24
        },
        new List<int> {
            20, 21, 23, 29
        },
        new List<int> {
            21, 22, 24, 26, 29
        },
        new List<int> {
            21, 23, 25
        },
        new List<int> {
            23, 24, 26
        },
        new List<int> {
            23, 25
        },
        new List<int> {
            17, 28, 32, 34
        },
        new List<int> {
            17, 27, 29, 30, 32
        },
        new List<int> {
            17, 20, 22, 23, 28, 29
        },
        new List<int> {
            28, 29, 31, 32
        },
        new List<int> {
            30, 32, 41
        },
        new List<int> {
            27, 28, 30, 31, 33, 34
        },
        new List<int> {
            32, 34, 35, 39, 40
        },
        new List<int> {
            27, 32, 33, 35, 36
        },
        new List<int> {
            33, 34, 36, 38, 39
        },
        new List<int> {
            34, 35, 37, 38
        },
        new List<int> {
            1, 36, 38
        },
        new List<int> {
            35, 36, 39
        },
        new List<int> {
            33, 35, 38, 40
        },
        new List<int> {
            33, 39
        },
        new List<int> {
            31, 42
        },
        new List<int> {
            41, 43, 44
        },
        new List<int> {
            42, 44
        },
        new List<int> {
            42, 43
        },
    };

    // Start is called before the first frame update
    void Start()
    {
        // initializes the country instances and stores them in a hashmap to access the country map with the button
        // the colors of countries are randomly set here
        // also gets added to the countries list
        Debug.Log($"starting with {playerCount} players");

                //initializes the gamestate instance which is singleton
        this.game = GameState.New(ScriptMap1.playerCount);
        
        int num_of_countries = 44 / ScriptMap1.playerCount;
        int remainder = 44 % ScriptMap1.playerCount;

        List<Color> list_of_colors = new List<Color>();
        List<Color> copy_turns = new List<Color>();

        foreach (Color color in this.game.turns_order) {
            copy_turns.Add(color);
            for (int i = 0; i < num_of_countries; i ++) list_of_colors.Add(color);
        }


        for (int i = 0; i < remainder; i++) {
            int index = GameState.random.Next(copy_turns.Count);
            list_of_colors.Add(copy_turns[index]);
            copy_turns.RemoveAt(index);
        }

        List<Country> countries = new List<Country>();
        for (int i = 1; i < 45; i++) {
            Button button = GameObject.Find($"country{i}").GetComponent<Button>();


            int index = GameState.random.Next(list_of_colors.Count);
            
            UnityEngine.Color color = list_of_colors[index];
            list_of_colors.RemoveAt(index);

            button.GetComponent<Image>().color = color;
            Country country = new Country(button, color);

            this.country_map.Add(button, country);
            countries.Add(country);
        }

        if (countries.Count != this.list_of_neighbors.Count) {
            throw new Exception("list sizes do not match");
        } else {
            Debug.Log("list sizes do match");
        }

        // sets neighbors for each country 
        for(int i = 0; i < countries.Count; i++) {
            List<Country> neighbors = new List<Country>();

            foreach(int index in list_of_neighbors[i]) {
                neighbors.Add(countries[index - 1]);
            }

            countries[i].set_neighbors(neighbors);
        }


    }

    private List<T> ArrayListList<T>()
    {
        throw new NotImplementedException();
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
                    Country country = this.country_map[selectedBtn];

                    game.take_country_click(country);

                    // foreach (Button neighbour in dictOfNeighbours[selectedBtn])
                    // {
                    //     neighbour.GetComponent<Image>().color = Color.green;
                    // }
                    

                }



            }
        }
    }
}