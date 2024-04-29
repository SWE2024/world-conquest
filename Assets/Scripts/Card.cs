using UnityEngine;

public enum Type
{
    // wildcard
    wildcard,

    // map 1
    infantry,
    artillery,
    cavalry,

    // map 2
    jet,
    soldier,
    tank,
}

public class Card
{
    Country parent;
    Type type;
    Sprite sprite;

    public Card(Country c, Type t, Sprite s)
    {
        parent = c;
        type = t;
        sprite = s;
    }

    public string GetImage()
    {
        return $"{parent.GetName().ToLower()}_{type}";
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}
