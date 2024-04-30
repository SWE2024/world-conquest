using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScriptHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 moveRight = Vector3.Lerp(btn.transform.position, new Vector3(btn.transform.position.x + 90, btn.transform.position.y), 5f * Time.deltaTime);
        btn.transform.position = moveRight;
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        Vector3 moveLeft = Vector3.Lerp(btn.transform.position, new Vector3(btn.transform.position.x - 90, btn.transform.position.y), 5f * Time.deltaTime);
        btn.transform.position = moveLeft;
    }
}
