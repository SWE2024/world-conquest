using TMPro;
using UnityEngine;

public class Killfeed : MonoBehaviour
{
    public static void Update(string text)
    {
        GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text += text + "\n";
        if (GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text.Split('\n').Length > 5)
        {
            RemoveFirstLine();
        }
        else
        {
            Wait.Start(8f, () => RemoveFirstLine()); // clear item from killfeed after 8s
        }
    }

    static void RemoveFirstLine()
    {
        string s = GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text;
        string removeFirstLine = s.Substring(s.IndexOf('\n') + 1);
        GameObject.Find("KillfeedText").GetComponent<TextMeshProUGUI>().text = removeFirstLine;
    }
}
