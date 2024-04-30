using System.Collections.Generic;

/// <summary>
/// <c>Map3</c> contains the list of country names and neighbouring countries for map 3.
/// </summary>
public class Map3
{
    public static Dictionary<int, string> CountryNameMap = new Dictionary<int, string>()
    {
        { 1, "Pefrounia"},
        { 2, "Uthoegro"},
        { 3, "Quscea"},
        { 4, "Priuyare"},
        { 5, "Ospela"},
        { 6, "Usnal"},
    };

    public static List<List<int>> ListOfNeighbours = new List<List<int>> {
        new List<int> {
            2, 3, 4, 5
        },
        new List<int> {
            1, 3, 6
        },
        new List<int> {
            1, 2, 4, 6
        },
        new List<int> {
            1, 3, 5, 6
        },
        new List<int> {
            1, 4, 6
        },
        new List<int> {
            2, 3, 4, 5
        },
    };
}