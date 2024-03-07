using UnityEngine;

public enum Troop
{
    Infantry,
    Tanks,
}

public class Player
{
    public Color Color;
    public int NumberOfTroops;


    public Player(Color color)
    {
        this.Color = color;
        this.NumberOfTroops = 20;
    }
}