using System;
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
        TextMeshProUGUI killfeed = GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>();
        killfeed.text = $"({DateTime.Now.ToLongTimeString()}) {text}\n" + killfeed.text;
    }
}
