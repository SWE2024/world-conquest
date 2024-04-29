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

    public Card(Country c, Type t)
    {
        parent = c;
        type = t;
    }

    public string GetImage()
    {
        return $"{parent.GetName().ToLower()}_{type}";
    }
}
