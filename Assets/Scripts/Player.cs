using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Troop {
    infantry,
    tanks,
}

public class Player {
    public Color color;
    public int num_of_troops;


    public Player(Color color) {
        this.color = color;
        this.num_of_troops = 20;
    }
}