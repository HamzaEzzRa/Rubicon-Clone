using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HTTPLink : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D linkCursor;

    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(linkCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Application.OpenURL(textMesh.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
