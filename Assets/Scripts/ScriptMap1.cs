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



    // Dictionary<Button, List<Button>> dictOfNeighbours;
    // [SerializeField] Button country1;
    // [SerializeField] Button country2;
    // [SerializeField] Button country3;
    // [SerializeField] Button country4;
    // [SerializeField] Button country5;
    // [SerializeField] Button country6;
    // [SerializeField] Button country7;
    // [SerializeField] Button country8;
    // [SerializeField] Button country9;
    // [SerializeField] Button country10;
    // [SerializeField] Button country11;
    // [SerializeField] Button country12;
    // [SerializeField] Button country13;
    // [SerializeField] Button country14;
    // [SerializeField] Button country15;
    // [SerializeField] Button country16;
    // [SerializeField] Button country17;
    // [SerializeField] Button country18;
    // [SerializeField] Button country19;
    // [SerializeField] Button country20;
    // [SerializeField] Button country21;
    // [SerializeField] Button country22;
    // [SerializeField] Button country23;
    // [SerializeField] Button country24;
    // [SerializeField] Button country25;
    // [SerializeField] Button country26;
    // [SerializeField] Button country27;
    // [SerializeField] Button country28;
    // [SerializeField] Button country29;
    // [SerializeField] Button country30;
    // [SerializeField] Button country31;
    // [SerializeField] Button country32;
    // [SerializeField] Button country33;
    // [SerializeField] Button country34;
    // [SerializeField] Button country35;
    // [SerializeField] Button country36;
    // [SerializeField] Button country37;
    // [SerializeField] Button country38;
    // [SerializeField] Button country39;
    // [SerializeField] Button country40;
    // [SerializeField] Button country41;
    // [SerializeField] Button country42;
    // [SerializeField] Button country43;
    // [SerializeField] Button country44;

    // Start is called before the first frame update
    void Start()
    {
        
        // dictOfNeighbours = new Dictionary<Button, List<Button>>();
        // dictOfNeighbours.Add(country1, new List<Button>() { country2, country3, country37 });
        // dictOfNeighbours.Add(country2, new List<Button>() { country1, country3, country4, country9 });
        // dictOfNeighbours.Add(country3, new List<Button>() { country1, country2, country4, country6 });
        // dictOfNeighbours.Add(country4, new List<Button>() { country2, country3, country5, country6, country7, country9 });
        // dictOfNeighbours.Add(country5, new List<Button>() { country4, country7, country9 });
        // dictOfNeighbours.Add(country6, new List<Button>() { country3, country4, country7, country8 });
        // dictOfNeighbours.Add(country7, new List<Button>() { country4, country5, country6, country8 });
        // dictOfNeighbours.Add(country8, new List<Button>() { country6, country7, country10 });
        // dictOfNeighbours.Add(country9, new List<Button>() { country2, country4, country5, country14 });
        // dictOfNeighbours.Add(country10, new List<Button>() { country8, country11, country12 });
        // dictOfNeighbours.Add(country11, new List<Button>() { country10, country12, country21 });
        // dictOfNeighbours.Add(country12, new List<Button>() { country10, country11, country13 });
        // dictOfNeighbours.Add(country13, new List<Button>() { country11, country12 });
        // dictOfNeighbours.Add(country14, new List<Button>() { country9, country15, country16 });
        // dictOfNeighbours.Add(country15, new List<Button>() { country14, country16, country18, country19 });
        // dictOfNeighbours.Add(country16, new List<Button>() { country14, country15, country17, country18 });
        // dictOfNeighbours.Add(country17, new List<Button>() { country16, country18, country20, country27, country28, country29 });
        // dictOfNeighbours.Add(country18, new List<Button>() { country15, country16, country17, country19, country20 });
        // dictOfNeighbours.Add(country19, new List<Button>() { country15, country18, country20, country21 });
        // dictOfNeighbours.Add(country20, new List<Button>() { country17, country18, country19, country22, country29 });
        // dictOfNeighbours.Add(country21, new List<Button>() { country11, country19, country22, country23, country24 });
        // dictOfNeighbours.Add(country22, new List<Button>() { country20, country21, country23, country29 });
        // dictOfNeighbours.Add(country23, new List<Button>() { country21, country22, country24, country26, country29 });
        // dictOfNeighbours.Add(country24, new List<Button>() { country21, country23, country25 });
        // dictOfNeighbours.Add(country25, new List<Button>() { country23, country24, country26 });
        // dictOfNeighbours.Add(country26, new List<Button>() { country23, country25 });
        // dictOfNeighbours.Add(country27, new List<Button>() { country17, country28, country32, country34 });
        // dictOfNeighbours.Add(country28, new List<Button>() { country17, country27, country29, country30, country32 });
        // dictOfNeighbours.Add(country29, new List<Button>() { country17, country20, country22, country23, country28, country29 });
        // dictOfNeighbours.Add(country30, new List<Button>() { country28, country29, country31, country32 });
        // dictOfNeighbours.Add(country31, new List<Button>() { country30, country32, country41 });
        // dictOfNeighbours.Add(country32, new List<Button>() { country27, country28, country30, country31, country33, country34 });
        // dictOfNeighbours.Add(country33, new List<Button>() { country32, country34, country35, country39, country40 });
        // dictOfNeighbours.Add(country34, new List<Button>() { country27, country32, country33, country35, country36 });
        // dictOfNeighbours.Add(country35, new List<Button>() { country33, country34, country36, country38, country39 });
        // dictOfNeighbours.Add(country36, new List<Button>() { country34, country35, country37, country38 });
        // dictOfNeighbours.Add(country37, new List<Button>() { country1, country36, country38 });
        // dictOfNeighbours.Add(country38, new List<Button>() { country35, country36, country39 });
        // dictOfNeighbours.Add(country39, new List<Button>() { country33, country35, country38, country40 });
        // dictOfNeighbours.Add(country40, new List<Button>() { country33, country39 });
        // dictOfNeighbours.Add(country41, new List<Button>() { country31, country42 });
        // dictOfNeighbours.Add(country42, new List<Button>() { country41, country43, country44 });
        // dictOfNeighbours.Add(country43, new List<Button>() { country42, country44 });
        // dictOfNeighbours.Add(country44, new List<Button>() { country42, country43 });



        

        // initializes the country instances and stores them in a hashmap to access the country map with the button
        // the colors of countries are randomly set here
        // also gets added to the countries list

        
        List<Country> countries = new List<Country>();
        for (int i = 1; i < 45; i++) {
            Button button = GameObject.Find($"country{i}").GetComponent<Button>();
            UnityEngine.Color color = GameState.int_to_color(GameState.random.Next(0, 3));
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


        //initializes the gamestate instance
        this.game = new GameState(countries);
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