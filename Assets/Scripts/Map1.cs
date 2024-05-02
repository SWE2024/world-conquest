using System.Collections.Generic;

/// <summary>
/// <c>Map1</c> contains the list of country names and neighbouring countries for map 1.
/// </summary>
public class Map1
{
    public static Dictionary<int, string> CountryNameMap = new Dictionary<int, string>()
    {
        { 1, "Alaska"},
        { 2, "Northwest"},
        { 3, "Alberta"},
        { 4, "Ontario"},
        { 5, "Eastern"},
        { 6, "West US"},
        { 7, "East US"},
        { 8, "C. America"},
        { 9, "Greenland"},
        { 10, "Venezuela"},
        { 11, "Brazil"},
        { 12, "Peru"},
        { 13, "Argentina"},
        { 14, "Iceland"},
        { 15, "Britain"},
        { 16, "Scandinavia"},
        { 17, "Russia"},
        { 18, "North EU"},
        { 19, "West EU"},
        { 20, "South EU"},
        { 21, "N. Africa"},
        { 22, "Egypt"},
        { 23, "E. Africa"},
        { 24, "C. Africa"},
        { 25, "S. Africa"},
        { 26, "Madagascar"},
        { 27, "China"},
        { 28, "Ural" },
        { 29, "Afghanistan" },
        { 30, "India" },
        { 31, "SE. Asia" },
        { 32, "Mongolia" },
        { 33, "Middle East" },
        { 34, "Siberia" },
        { 35, "Irkutsk" },
        { 36, "Yakutsk" },
        { 37, "Kamchatka" },
        { 38, "Yelizovo" },
        { 39, "Vilyuchinsk" },
        { 40, "Japan" },
        { 41, "Indonesia" },
        { 42, "New Guinea" },
        { 43, "W. Australia" },
        { 44, "E. Australia" },
    };

    public static List<List<int>> ListOfNeighbors = new List<List<int>> {
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
            10, 12, 13, 21
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
            21, 22, 24, 25, 26, 29
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
            17, 20, 22, 23, 28, 30
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
            35, 36, 37, 39
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
}