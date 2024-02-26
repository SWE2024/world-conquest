using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




// public class Player {};




public class Country
{
    Button pointer;
    UnityEngine.Color color;
    bool highlighted = false;
    int troops = 0;
    public Country(Button button, UnityEngine.Color color) {
        this.pointer = button;
        this.color = color;
    }
    
}

// public enum colors : UnityEngine.Color {
//     green = Color.green,
//     blue = Color.blue,
//     red = Color.red,
//     white = Color.white,
// }



public class ScriptMap1 : MonoBehaviour
{
    Dictionary<Button, Country> country_state = new Dictionary<Button, Country>();
    public System.Random random = new System.Random();


    static UnityEngine.Color int_to_color(int num) 
    {
        switch (num) 
        {
            case 0:
                return UnityEngine.Color.green;
            case 1:
                return UnityEngine.Color.blue;
            case 2:
                return UnityEngine.Color.red;
            default:
                return UnityEngine.Color.white;
        }
    }



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
        for (int i = 1; i < 45; i++) {
            Button button = GameObject.Find($"country{i}").GetComponent<Button>();
            UnityEngine.Color color = ScriptMap1.int_to_color(this.random.Next(0, 3));
            button.GetComponent<Image>().color = color;
            Country country = new Country(button, color);

            this.country_state.Add(button, country);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

            // if (selectedObj != null)
            // {
            //     Button selectedBtn = GameObject.Find(selectedObj.name).GetComponent<Button>();

            //     if (selectedBtn != null) 
            //     {
                    

            //     }



            //     foreach (Button neighbour in dictOfNeighbours[selectedBtn])
            //     {
            //         neighbour.GetComponent<Image>().color = Color.green;
            //     }
            // }
        }
    }
}