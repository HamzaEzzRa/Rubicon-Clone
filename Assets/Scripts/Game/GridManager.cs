using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Node Settings")]
    [SerializeField] private int maxDepth = 11;
    [SerializeField] private Vector2 spacing = Vector2.zero;
    [SerializeField] private Vector3 nodeScale = Vector3.one;

    [SerializeField] private float hoverZoomFactor = 2f;

    [SerializeField] private Sprite nodeSprite = default;

    [Header("Link Settings")]
    [SerializeField] private float linkWidth = 0.1f;
    [SerializeField] private Material linkMaterial = default;

    [SerializeField, HideInInspector] private GameObject grid;

    [SerializeField, HideInInspector] private GameNode[] gameNodes;

    private void Start()
    {
        Link();
    }

    public void Generate()
    {
        SafeDestroy(grid);

        int rowCount = maxDepth * 2,
            columnCount = maxDepth * 2 + 1;

        gameNodes = new GameNode[rowCount * columnCount];

        grid = new GameObject("Grid");

        for (int i = 0; i < maxDepth; i++)
        {
            int nodeCount = i * 2 + 1;
            float yPosition = maxDepth - i - 0.5f;

            for (int j = 0; j <= nodeCount / 2; j++)
            {
                GameNode leftTopNode = new GameObject("Node").AddComponent<GameNode>();
                leftTopNode.Setup(i * columnCount + columnCount / 2 - j, new Vector2(-j * (nodeScale.x + spacing.x), yPosition * (nodeScale.y + spacing.y)), nodeScale, nodeSprite, grid.transform, hoverZoomFactor);
                gameNodes[i * columnCount + columnCount / 2 - j] = leftTopNode;

                GameNode leftBottomNode = new GameObject("Node").AddComponent<GameNode>();
                leftBottomNode.Setup((rowCount - i - 1) * columnCount + columnCount / 2 - j, new Vector2(-j * (nodeScale.x + spacing.x), -yPosition * (nodeScale.y + spacing.y)), nodeScale, nodeSprite, grid.transform, hoverZoomFactor);
                gameNodes[(rowCount - i - 1) * columnCount + columnCount / 2 - j] = leftBottomNode;

                if (j != 0)
                {
                    GameNode rightTopNode = new GameObject("Node").AddComponent<GameNode>();
                    rightTopNode.Setup(i * columnCount + columnCount / 2 + j, new Vector2(j * (nodeScale.x + spacing.x), yPosition * (nodeScale.y + spacing.y)), nodeScale, nodeSprite, grid.transform, hoverZoomFactor);
                    gameNodes[i * columnCount + columnCount / 2 + j] = rightTopNode;

                    GameNode rightBottomNode = new GameObject("Node").AddComponent<GameNode>();
                    rightBottomNode.Setup((rowCount - i - 1) * columnCount + columnCount / 2 + j, new Vector2(j * (nodeScale.x + spacing.x), -yPosition * (nodeScale.y + spacing.y)), nodeScale, nodeSprite, grid.transform, hoverZoomFactor);
                    gameNodes[(rowCount - i - 1) * columnCount + columnCount / 2 + j] = rightBottomNode;
                }
            }
        }

        //string board = "";
        //for (int i = 0; i < maxDepth * 2; i++)
        //{
        //    for (int j = 0; j < maxDepth * 2 + 1; j++)
        //    {
        //        board += gameNodes[i, j] ? gameNodes[i, j].name + " " : "null ";
        //    }
        //    board += "\n";
        //}
        //Debug.Log(board);
    }

    public void Link()
    {
        int rowCount = maxDepth * 2,
            columnCount = maxDepth * 2 + 1;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                int idx = i * columnCount + j;
                GameNode node = gameNodes[idx];

                if (node != null)
                {
                    if (idx >= 2 * maxDepth * (maxDepth + 0.5f))
                    {
                        node.NodeSide = NodeSide.SOUTH;
                    }
                    else
                    {
                        node.NodeSide = NodeSide.NORTH;
                    }

                    Transform[] nodeLinks = node.GetComponentsInChildren<Transform>();
                    for (int t = 1; t < nodeLinks.Length; t++)
                    {
                        SafeDestroy(nodeLinks[t].gameObject);
                    }

                    if (i > 0 && gameNodes[(i - 1) * columnCount + j] != null)
                    {
                        if ((node.LinkMask & LinkMask.NORTH) != 0)
                        {
                            // Visual part of the link
                            Vector3 nodePosition = node.transform.position;
                            Vector3 nodeScale = node.transform.localScale;

                            Vector2 startPosition = new Vector2(nodePosition.x, nodePosition.y + nodeScale.y / 2);
                            Vector2 endPosition = new Vector2(nodePosition.x, nodePosition.y + nodeScale.y / 2 + spacing.y);

                            DrawLine(
                                "North Link",
                                Color.white,
                                linkWidth,
                                new Vector3(startPosition.x, startPosition.y, nodePosition.z),
                                new Vector3(endPosition.x, endPosition.y, nodePosition.z)
                            ).transform.parent = node.transform;

                            // Logical part of the link
                            node.NorthConnection = gameNodes[(i - 1) * columnCount + j];
                            gameNodes[(i - 1) * columnCount + j].SouthConnection = node;
                        }
                        else
                        {
                            node.NorthConnection = null;
                            gameNodes[(i - 1) * columnCount + j].SouthConnection = null;
                        }
                    }

                    if (j < columnCount - 1 && gameNodes[i * columnCount + j + 1] != null)
                    {
                        if ((node.LinkMask & LinkMask.EAST) != 0)
                        {
                            // Visual part of the link
                            Vector3 nodePosition = node.transform.position;
                            Vector3 nodeScale = node.transform.localScale;

                            Vector2 startPosition = new Vector2(nodePosition.x + nodeScale.x / 2, nodePosition.y);
                            Vector2 endPosition = new Vector2(nodePosition.x + nodeScale.x / 2 + spacing.x, nodePosition.y);

                            DrawLine(
                                "East Link",
                                Color.white,
                                linkWidth,
                                new Vector3(startPosition.x, startPosition.y, nodePosition.z),
                                new Vector3(endPosition.x, endPosition.y, nodePosition.z)
                            ).transform.parent = node.transform;

                            // Logical part of the link
                            node.EastConnection = gameNodes[i * columnCount + j + 1];
                            gameNodes[i * columnCount + j + 1].WestConnection = node;
                        }
                        else
                        {
                            node.EastConnection = null;
                            gameNodes[i * columnCount + j + 1].WestConnection = null;
                        }
                    }

                    if (i > 0 && j < columnCount - 1 && gameNodes[(i - 1) * columnCount + j + 1] != null)
                    {
                        if ((node.LinkMask & LinkMask.NORTH_EAST) != 0)
                        {
                            // Visual part of the link
                            Vector3 nodePosition = node.transform.position;
                            Vector3 nodeScale = node.transform.localScale;

                            Vector2 startPosition = new Vector2(
                                nodePosition.x + nodeScale.x / 2 * Mathf.Cos(Mathf.PI / 4),
                                nodePosition.y + nodeScale.y / 2 * Mathf.Sin(Mathf.PI / 4)
                            );
                            Vector2 endPosition = new Vector2(
                                nodePosition.x + nodeScale.x / 2 * (2 - Mathf.Cos(Mathf.PI / 4)) + spacing.x,
                                nodePosition.y + nodeScale.y / 2 * (2 - Mathf.Sin(Mathf.PI / 4)) + spacing.y
                            );

                            DrawLine(
                                "North East Link",
                                Color.white,
                                linkWidth,
                                new Vector3(startPosition.x, startPosition.y, nodePosition.z),
                                new Vector3(endPosition.x, endPosition.y, nodePosition.z)
                            ).transform.parent = node.transform;

                            // Logical part of the link
                            node.NorthEastConnection = gameNodes[(i - 1) * columnCount + j + 1];
                            gameNodes[(i - 1) * columnCount + j + 1].SouthWestConnection = node;
                        }
                        else
                        {
                            node.NorthEastConnection = null;
                            gameNodes[(i - 1) * columnCount + j + 1].SouthWestConnection = null;
                        }
                    }

                    if (i < rowCount - 1 && j < columnCount - 1 && gameNodes[(i + 1) * columnCount + j + 1] != null)
                    {
                        if ((node.LinkMask & LinkMask.SOUTH_EAST) != 0)
                        {
                            // Visual part of the link
                            Vector3 nodePosition = node.transform.position;
                            Vector3 nodeScale = node.transform.localScale;

                            Vector2 startPosition = new Vector2(
                                nodePosition.x + nodeScale.x / 2 * Mathf.Cos(-Mathf.PI / 4),
                                nodePosition.y + nodeScale.y / 2 * Mathf.Sin(-Mathf.PI / 4)
                            );
                            Vector2 endPosition = new Vector2(
                                nodePosition.x + nodeScale.x - nodeScale.x / 2 * Mathf.Cos(-Mathf.PI / 4) + spacing.x,
                                nodePosition.y - nodeScale.y - nodeScale.y / 2 * Mathf.Sin(-Mathf.PI / 4) - spacing.y
                            );

                            DrawLine(
                                "South East Link",
                                Color.white,
                                linkWidth,
                                new Vector3(startPosition.x, startPosition.y, nodePosition.z),
                                new Vector3(endPosition.x, endPosition.y, nodePosition.z)
                            ).transform.parent = node.transform;

                            // Logical part of the link
                            node.SouthEastConnection = gameNodes[(i + 1) * columnCount + j + 1];
                            gameNodes[(i + 1) * columnCount + j + 1].NorthWestConnection = node;
                        }
                        else
                        {
                            node.SouthEastConnection = null;
                            gameNodes[(i + 1) * columnCount + j + 1].NorthWestConnection = null;
                        }
                    }

                    // Update the visuals of the node
                    node.UpdateNode(idx, nodeScale, nodeSprite, hoverZoomFactor);
                }
            }
        }
    }

    public static bool CheckConnected(GameNode start, GameNode end)
    {
        // Uses a queue structure for a breadth-first search to check if a player has connected two nodes
        // Can be speedup with bi-directional search

        Queue<GameNode> nodeQueue = new Queue<GameNode>();
        nodeQueue.Enqueue(start);

        Dictionary<int, bool> checkedNodes = new Dictionary<int, bool>();

        while (nodeQueue.Count > 0)
        {
            //Debug.Log("Loop");

            GameNode currentNode = nodeQueue.Dequeue();
            if (!checkedNodes.TryAdd(currentNode.Id, true))
            {
                checkedNodes[currentNode.Id] = true;
            }

            if (currentNode.NorthConnection != null && currentNode.NorthConnection.CurrentPlayerIsActive)
            {
                if (currentNode.NorthConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.NorthConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.NorthConnection);
                }
            }
            if (currentNode.NorthEastConnection != null && currentNode.NorthEastConnection.CurrentPlayerIsActive)
            {
                if (currentNode.NorthEastConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.NorthEastConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.NorthEastConnection);
                }
            }
            if (currentNode.EastConnection != null && currentNode.EastConnection.CurrentPlayerIsActive)
            {
                if (currentNode.EastConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.EastConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.EastConnection);
                }
            }
            if (currentNode.SouthEastConnection != null && currentNode.SouthEastConnection.CurrentPlayerIsActive)
            {
                if (currentNode.SouthEastConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.SouthEastConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.SouthEastConnection);
                }
            }
            if (currentNode.SouthConnection != null && currentNode.SouthConnection.CurrentPlayerIsActive)
            {
                if (currentNode.SouthConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.SouthConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.SouthConnection);
                }
            }
            if (currentNode.SouthWestConnection != null && currentNode.SouthWestConnection.CurrentPlayerIsActive)
            {
                if (currentNode.SouthWestConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.SouthWestConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.SouthWestConnection);
                }
            }
            if (currentNode.WestConnection != null && currentNode.WestConnection.CurrentPlayerIsActive)
            {
                if (currentNode.WestConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.WestConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.WestConnection);
                }
            }
            if (currentNode.NorthWestConnection != null && currentNode.NorthWestConnection.CurrentPlayerIsActive)
            {
                if (currentNode.NorthWestConnection.Id == end.Id)
                {
                    return true;
                }
                else if (!checkedNodes.TryGetValue(currentNode.NorthWestConnection.Id, out bool check))
                {
                    nodeQueue.Enqueue(currentNode.NorthWestConnection);
                }
            }
        }

        return false;
    }

    private LineRenderer DrawLine(string name, Color color, float width, Vector3 startPosition, Vector3 endPosition)
    {
        // Quick utility function to draw links between nodes

        LineRenderer lineRenderer = new GameObject(name).AddComponent<LineRenderer>();
        lineRenderer.material = linkMaterial;

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        lineRenderer.sortingOrder = 0;

        return lineRenderer;
    }

    private void SafeDestroy(GameObject gameObject)
    {
        if (gameObject != null)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
