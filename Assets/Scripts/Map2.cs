using System.Collections.Generic;

/// <summary>
/// <c>Map2</c> contains the list of country names and neighbouring countries for map 2.
/// </summary>
public class Map2
{
    public static Dictionary<int, string> CountryNameMap = new Dictionary<int, string>()
    {
        { 1, "Pefrounia"},
        { 2, "Uthoegro"},
        { 3, "Quscea"},
        { 4, "Priuyare"},
        { 5, "Ospela"},
        { 6, "Usnal"},
        { 7, "Wheok Pla"},
        { 8, "Cebraunia"},
        { 9, "Modra"},
        { 10, "Udreau"},
        { 11, "Ospitan"},
        { 12, "Vothaostan"},
        { 13, "Xocroesal"},
        { 14, "Astrus"},
        { 15, "Straulia"},
        { 16, "Ublya"},
        { 17, "Nosturg"},
        { 18, "Ethein"},
        { 19, "Spiedan"},
        { 20, "Jasmad"},
        { 21, "Ascurg"},
        { 22, "Uskaevania"},
        { 23, "Xesnana"},
        { 24, "Noplain"},
        { 25, "Ashai"},
        { 26, "Oshurg"},
        { 27, "Swuca"},
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
            1, 4, 6, 18
        },
        new List<int> {
            2, 3, 4, 5, 7, 8, 9, 10
        },
        new List<int> {
            6, 8
        },
        new List<int> {
            6, 7, 9, 21
        },
        new List<int> {
            6, 8, 10, 11
        },
        new List<int> {
            6, 9, 11
        },
        new List<int> {
            9, 10, 25
        },
        new List<int> {
            13
        },
        new List<int> {
            12, 14
        },
        new List<int> {
            13, 15, 16
        },
        new List<int> {
            14, 16, 17
        },
        new List<int> {
            14, 15, 17, 18, 19
        },
        new List<int> {
            15, 16, 19, 20
        },
        new List<int> {
            5, 16, 19
        },
        new List<int> {
            16, 17, 18, 20, 25
        },
        new List<int> {
            17, 19, 27
        },
        new List<int> {
            8, 22
        },
        new List<int> {
            21, 23
        },
        new List<int> {
            22, 24, 25
        },
        new List<int> {
            23, 25, 27
        },
        new List<int> {
            11, 19, 23, 24
        },
        new List<int> {
            27
        },
        new List<int> {
            20, 24, 26
        },
    };
}