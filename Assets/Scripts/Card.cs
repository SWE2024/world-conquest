using UnityEngine;
public class Card
{
    Country parent;
    string type;
    Sprite sprite;

    public Card(Country c, string card_type, Sprite s)
    {
        parent = c;
        type = card_type;
        sprite = s;
    }

    /// <summary>
    /// <c>GetCardType</c> returns the type of a card.
    /// </summary>
    /// <returns>
    /// A string showing the type of the card.
    /// </returns>
    public string GetCardType() => type;

    /// <summary>
    /// <c>GetSprite</c> returns the sprite associated with a card.
    /// </summary>
    /// <returns>
    /// A sprite of the card image.
    /// </returns>
    public Sprite GetSprite() => sprite;
}
