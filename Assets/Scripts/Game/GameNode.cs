using UnityEngine;
using UnityEngine.EventSystems;

using System;

[Flags]
public enum LinkMask
{
    NORTH = 1 << 1,
    NORTH_EAST = 1 << 2,
    EAST = 1 << 3,
    SOUTH_EAST = 1 << 4,
}

public enum NodeType
{
    NORMAL = 0,
    YELLOW,
    CYAN,
    GREEN,
    BLUE,
    RED,
    ORANGE,
}

public enum NodeSide
{
    NORTH,
    SOUTH,
}

[RequireComponent(typeof(SpriteRenderer))]
public class GameNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public int Id => id;

    public NodeSide NodeSide
    {
        get => nodeSide;
        set
        {
            nodeSide = value;
        }
    }

    public LinkMask LinkMask => linkMask;
    public NodeType NodeType => nodeType;

    public int ActivePlayer => activePlayer;

    [SerializeField] private LinkMask linkMask;
    [SerializeField] private NodeType nodeType;

    [SerializeField, HideInInspector] private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector] private Vector3 originalScale;
    [SerializeField, HideInInspector] private float zoomFactor;

    public GameNode
        NorthConnection,
        NorthEastConnection,
        EastConnection,
        SouthEastConnection,
        SouthConnection,
        SouthWestConnection,
        WestConnection,
        NorthWestConnection;

    private int activePlayer = 0;

    [SerializeField] private int id;

    [SerializeField] private NodeSide nodeSide;

    public bool CurrentPlayerIsActive => activePlayer == GameManager.Instance.CurrentPlayer || NodeType == NodeType.YELLOW;

    private bool IsPlayable => !GameManager.Instance.IsGameOver && NodeType != NodeType.YELLOW &&
        (GameManager.Instance.IsFirstRound ||
        (NorthConnection != null && NorthConnection.CurrentPlayerIsActive) ||
        (NorthEastConnection != null && NorthEastConnection.CurrentPlayerIsActive) ||
        (EastConnection != null && EastConnection.CurrentPlayerIsActive) ||
        (SouthEastConnection != null && SouthEastConnection.CurrentPlayerIsActive) ||
        (SouthConnection != null && SouthConnection.CurrentPlayerIsActive) ||
        (SouthWestConnection != null && SouthWestConnection.CurrentPlayerIsActive) ||
        (WestConnection != null && WestConnection.CurrentPlayerIsActive) ||
        (NorthWestConnection != null && NorthWestConnection.CurrentPlayerIsActive));

    private SpriteRenderer childSpriteRenderer;

    public void Setup(int id, Vector2 position, Vector3 scale, Sprite sprite, Transform parentTransform, float hoverZoomFactor)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        spriteRenderer = renderers[0];
        if (renderers.Length > 1)
        {
            childSpriteRenderer = renderers[1];
        }

        UpdateNode(id, scale, sprite, hoverZoomFactor);

        transform.localPosition = position;
        if (parentTransform != null)
        {
            transform.parent = parentTransform;
        }

        gameObject.AddComponent<CircleCollider2D>();
    }
    
    public void UpdateNode(int id, Vector3 scale, Sprite sprite, float hoverZoomFactor)
    {
        this.id = id;

        originalScale = scale;
        zoomFactor = hoverZoomFactor;
        transform.localScale = scale;

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = MapTypeToColor(NodeType);
        spriteRenderer.sortingOrder = 1;

        if (childSpriteRenderer == null)
        {
            childSpriteRenderer = new GameObject("Inner").AddComponent<SpriteRenderer>();
        }

        childSpriteRenderer.transform.SetParent(transform);
        childSpriteRenderer.transform.localPosition = Vector3.zero;
        childSpriteRenderer.transform.localScale = Vector3.one * 0.65f;

        childSpriteRenderer.sprite = sprite;
        childSpriteRenderer.color = Color.black;
        childSpriteRenderer.sortingOrder = 2;

        if (NodeType == NodeType.YELLOW)
        {
            // Correctly initializes the yellow/golden node
            childSpriteRenderer.color = Color.yellow;
            GameManager.Instance.PlayerMove(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activePlayer == 0 && IsPlayable)
        {
            transform.localScale = originalScale * zoomFactor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePlayer == 0 && IsPlayable)
        {
            activePlayer = GameManager.Instance.CurrentPlayer;

            childSpriteRenderer.color = GameManager.Instance.CurrentColor;
            transform.localScale = originalScale;

            GameManager.Instance.PlayerMove(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void SetOuterColor(Color color)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = color;
    }

    public static Color MapTypeToColor(NodeType type)
    {
        Color color = Color.white;
        switch (type)
        {
            case NodeType.NORMAL:
            {
                color = Color.white;
                break;
            }
            case NodeType.YELLOW:
            {
                color = Color.white;
                break;
            }
            case NodeType.CYAN:
            {
                color = Color.cyan;
                break;
            }
            case NodeType.GREEN:
            {
                color = Color.green;
                break;
            }
            case NodeType.BLUE:
            {
                color = Color.blue;
                break;
            }
            case NodeType.RED:
            {
                color = Color.red;
                break;
            }
            case NodeType.ORANGE:
            {
                color = new Color(1f, 0.647f, 0f);
                break;
            }
        }

        return color;
    }

    public static int MapTypeToValue(NodeType type)
    {
        int value = 0;
        switch (type)
        {
            case NodeType.NORMAL:
            {
                value = 0;
                break;
            }
            case NodeType.YELLOW:
            {
                value = 7;
                break;
            }
            case NodeType.CYAN:
            {
                value = 0;
                break;
            }
            case NodeType.GREEN:
            {
                value = 9;
                break;
            }
            case NodeType.BLUE:
            {
                value = 5;
                break;
            }
            case NodeType.RED:
            {
                value = 3;
                break;
            }
            case NodeType.ORANGE:
            {
                value = 1;
                break;
            }
        }

        return value;
    }
}
