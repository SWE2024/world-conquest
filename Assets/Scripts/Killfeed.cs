using TMPro;
using UnityEngine;

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
        GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text += text + "\n";
        if (GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text.Split('\n').Length > 5)
        {
            RemoveFirstLine();
        }
        else
        {
            Wait.Start(6f, () => RemoveFirstLine()); // clear item from killfeed after 6 seconds
        }
    }

    /// <summary>
    /// <c>RemoveFirstLine</c> gets the top line of the killfeed and removes it.
    /// </summary>
    static void RemoveFirstLine()
    {
        string s = GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text;
        string removeFirstLine = s.Substring(s.IndexOf('\n') + 1);
        GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text = removeFirstLine;
    }
}
