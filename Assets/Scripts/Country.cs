using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Country</c> represents a single country that you can click.
/// </summary>
public class Country
{
    string Name;
    public Button Pointer; // reference to the pointer
    List<Country> Neighbors;
    Player Owner = null; // may not reflect the button's color
    int Troops = 0;

    public Country(Button button, string name)
    {
        this.Pointer = button;
        this.Name = name;
        this.Pointer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{this.Troops}";
    }

    /// <summary>
    /// <c>SetNeighbors</c> loads a list of neighbors and sets the contents as the country's neighbors.
    /// </summary>
    /// <param name="list">The list of neighbors.</param>
    public void SetNeighbors(List<Country> list)
    {
        if (Neighbors != null) return;
        Neighbors = list;
    }

    /// <summary>
    /// <c>GetNeighbors</c> returns the list of neighboring countries.
    /// </summary>
    /// <returns>
    /// List of neighboring Country objects.
    /// </returns>
    public List<Country> GetNeighbors() => Neighbors;

    /// <summary>
    /// <c>GetName</c> returns the name of the country.
    /// </summary>
    /// <returns>
    /// Name of the Country.
    /// </returns>
    public string GetName() => this.Name;

    /// <summary>
    /// <c>GetOwner</c> returns the owner of the country.
    /// </summary>
    /// <returns>
    /// Owner of the Country..
    /// </returns>
    public Player GetOwner() => this.Owner;

    /// <summary>
    /// <c>GetTroops</c> returns the number of troops on the country.
    /// </summary>
    /// <returns>
    /// Number of troops on the Country.
    /// </returns>
    public int GetTroops() => this.Troops;

    /// <summary>
    /// <c>SetName</c> changes the name of the country then updates the UI associated with it.
    /// </summary>
    public void SetName(string name)
    {
        this.Name = name;
        this.Pointer.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = this.Name;
    }

    /// <summary>
    /// <c>SetOwner</c> changes the owner of the country, 
    /// removing the country from the old owner's <c>ownedCountries</c> list
    /// and adding it to the new owner's <c>ownedCountries</c> list.
    /// </summary>
    /// <param name="player">The new owner.</param>
    public void SetOwner(Player player)
    {
        if (Owner != null) Owner.RemoveCountry(this);
        this.Owner = player;
        player.AddCountry(this);

        Pointer.GetComponent<Image>().color = Owner.GetColor();
    }

    /// <summary>
    /// <c>ChangeTroops</c> updates the amount of troops by <c>offset</c>.
    /// </summary>
    /// <param name="offset">Number of troops being sent / removed (negative offset to remove troops).</param>
    public void ChangeTroops(int offset)
    {
        this.Troops += offset;
        this.Pointer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{this.Troops}";
    }

    /// <summary>
    /// <c>TempColorChange</c> updates the color of the country UI, usually for highlighting.
    /// </summary>
    /// <param name="color">The mew temporary color.</param>
    public void TempColorChange(Color color)
    {
        Pointer.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// <c>ReverseColorChange</c> reverses the changes made by <c>TempColorChange</c>.
    /// </summary>
    public void ReverseColorChange()
    {
        Pointer.GetComponent<Image>().color = this.Owner.GetColor();
    }

    /// <summary>
    /// <c>HighlightEnemyNeighbors</c> looks at all neighbours and checks if they are an enemy.
    /// Also highlights the friendly country grey and the enemy countries as white.
    /// </summary>
    /// <returns>
    /// List of <c>Country</c> objects that are neighboring enemies.
    /// </returns>
    public List<Country> HighlightEnemyNeighbours()
    {
        TempColorChange(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in GetNeighbors())
        {
            if (neighbor.Owner == this.Owner) continue;

            neighbor.TempColorChange(Color.white);
            output.Add(neighbor);
        }
        return output;
    }

    /// <summary>
    /// <c>HighlightFriendlyNeighbours</c> recursively checks all neighbors and if they are friendly, adds them to a list.
    /// </summary>
    /// <returns>
    /// List of <c>Country</c> that you can fortify with.
    /// </returns>
    public List<Country> HighlightFriendlyNeighbours() // currently only finds 1 layer of neighbors
    {
        TempColorChange(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in GetNeighbors())
        {
            if (neighbor.Owner != this.Owner) continue;

            neighbor.TempColorChange(Color.white);
            output.Add(neighbor);
        }
        return output;
    }
}