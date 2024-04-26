using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Killfeed</c> controls the bottom right section of the screen (killfeed).
/// </summary>
public class Killfeed : MonoBehaviour
{
    /// <summary>
    /// <c>Update</c> adds a string to the killfeed and then removes it after 6 seconds
    /// </summary>
    /// <param name="text">The string you would like to add to the killfeed.</param>
    public static void Update(string text)
    {
        TextMeshProUGUI killfeed = GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>();
        killfeed.text = $"({DateTime.Now.ToLongTimeString()}) {text}\n" + killfeed.text;
        //Wait.Start(6f, () => RemoveFirstLine()); // clear item from killfeed after 6 seconds
    }

    /* unused
    /// <summary>
    /// <c>RemoveFirstLine</c> gets the top line of the killfeed and removes it.
    /// </summary>
    static void RemoveFirstLine()
    {
        string s = GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text;
        string removeFirstLine = s.Substring(s.IndexOf('\n') + 1);
        GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text = removeFirstLine;
    }
    */
}
