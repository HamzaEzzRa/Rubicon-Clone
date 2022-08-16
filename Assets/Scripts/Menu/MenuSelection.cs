using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections.Generic;

public class MenuSelection : MonoBehaviour, IPointerDownHandler
{
    public static List<MenuSelection> menuSelectionList = new List<MenuSelection>();

    public int Id => id;

    [SerializeField] private int id;

    [SerializeField] private Image innerImage;
    [SerializeField] private Sprite selectionSprite;

    [SerializeField] private UnityEvent onClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (MenuSelection selection in menuSelectionList)
        {
            Transform[] children = selection.innerImage.GetComponentsInChildren<Transform>();
            if (children.Length > 0)
            {
                for (int i = children.Length - 1; i > 0; i--)
                {
                    Destroy(children[i].gameObject);
                }
            }
        }

        Image selectionImage = new GameObject("Selection").AddComponent<Image>();
        selectionImage.sprite = selectionSprite;
        selectionImage.color = Color.white;

        selectionImage.transform.SetParent(innerImage.transform);
        selectionImage.transform.localPosition = Vector3.zero;
        selectionImage.rectTransform.sizeDelta = innerImage.rectTransform.sizeDelta * 0.75f;

        onClick?.Invoke();
    }

    private void OnEnable()
    {
        menuSelectionList.Add(this);
    }

    private void OnDisable()
    {
        menuSelectionList.Remove(this);
    }
}
