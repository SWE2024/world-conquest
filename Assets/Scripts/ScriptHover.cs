using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScriptHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button btn;
    float duration = 0.1f; // duration of the animation
    Vector2 originalPos;

    void Start()
    {
        originalPos = btn.transform.localPosition;
    }

    /// <summary>
    /// <c>OnPointerEnter</c> moves the button to the right when the mouse enters.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 moveRight = originalPos + new Vector2(+100, 0);
        StartCoroutine(MoveButton(btn.transform.localPosition, moveRight));
    }

    /// <summary>
    /// <c>OnPointerExit</c> moves the button to the left when the mouse exits.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(MoveButton(btn.transform.localPosition, originalPos));
    }

    /// <summary>
    /// <c>MoveButton</c> smoothly slides a button from <c>startPos</c> to <c>endPos</c>.
    /// </summary>
    /// <param name="startPos">The starting 2D location of the GameObject.</param>
    /// <param name="endPos">The ending 2D location of the GameObject.</param>
    IEnumerator MoveButton(Vector2 startPos, Vector2 endPos)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            btn.transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        btn.transform.localPosition = endPos; // force it to the end position
    }
}
