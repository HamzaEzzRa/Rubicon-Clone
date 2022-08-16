using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentPlayer { get; private set; } = 1;

    public int CurrentTurn { get; private set; } = 0;

    public bool IsFirstRound { get; private set; } = true;

    public Color CurrentColor { get; private set; }

    public bool IsGameOver { get; private set; }

    public int PlayersCount { get; set; } = 2;

    [SerializeField] private int[] moveSequence = { 1, 2, 3 };

    [Header("UI")]
    [SerializeField] private GameObject inGameUI = default;
    [SerializeField] private Image moveImage = default;

    [Header("Colors")]
    [SerializeField] private Color[] playerColors = { Color.magenta, Color.cyan, new Color(0.737f, 0.424f, 0.145f), new Color(0.145f, 0.737f, 0.424f) };

    private List<TextMeshProUGUI> activeTextMeshes = new List<TextMeshProUGUI>();
    private List<LayoutGroup> activeMovesUI = new List<LayoutGroup>();
    private List<Image> remainingMoveImages = new List<Image>();

    private int moveSequenceIdx = 0;
    private int remainingMoves;

    private Dictionary<NodeType, List<GameNode>> northernColoredNodes = new Dictionary<NodeType, List<GameNode>>();
    private Dictionary<NodeType, List<GameNode>> southernColoredNodes = new Dictionary<NodeType, List<GameNode>>();
    private Dictionary<NodeType, bool> nodeConnectionChecks = new Dictionary<NodeType, bool>();

    private List<int> playersScores = new List<int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void NewGame()
    {
        CurrentPlayer = 1;
        CurrentTurn = 0;
        IsFirstRound = true;
        moveSequenceIdx = 0;
        remainingMoves = moveSequence[moveSequenceIdx];
        CurrentColor = playerColors[CurrentPlayer - 1];
        IsGameOver = false;

        for (int i = 0; i < PlayersCount; i++)
        {
            playersScores.Add(0);
        }

        // Initialize players UI
        if (inGameUI != null)
        {
            int i = 0;
            foreach (Transform t in inGameUI.transform)
            {
                t.gameObject.SetActive(true);
                activeTextMeshes.Add(t.GetComponentInChildren<TextMeshProUGUI>());
                activeTextMeshes[i].SetText(GetUpdatedScore(activeTextMeshes[i], playersScores[i]));

                activeMovesUI.Add(t.GetComponentInChildren<LayoutGroup>());

                i++;
                if (i >= PlayersCount)
                {
                    break;
                }
            }

            activeTextMeshes[0].fontStyle = FontStyles.Bold | FontStyles.Underline;
            for (i = 0; i < remainingMoves; i++)
            {
                Image img = Instantiate(moveImage, activeMovesUI[0].transform);
                img.color = CurrentColor;

                remainingMoveImages.Add(img);
            }
        }
    }

    private string GetUpdatedScore(TextMeshProUGUI textMesh, int score)
    {
        string[] tmp = textMesh.text.Split(":");
        tmp[1] = score.ToString();
        
        return tmp[0] + ": " + tmp[1];
    }

    public void PlayerMove(GameNode node)
    {
        if (node.NodeType != NodeType.NORMAL)
        {
            if (node.NodeSide == NodeSide.NORTH)
            {
                if (!northernColoredNodes.TryGetValue(node.NodeType, out List<GameNode> _))
                {
                    northernColoredNodes.Add(node.NodeType, new List<GameNode>());
                }
                northernColoredNodes[node.NodeType].Add(node);

                if (southernColoredNodes.TryGetValue(node.NodeType, out List<GameNode> coloredNodes))
                {
                    if (coloredNodes.Count > 0)
                    {
                        if (!nodeConnectionChecks.TryAdd(node.NodeType, true))
                        {
                            nodeConnectionChecks[node.NodeType] = true;
                        }
                    }
                }
            }
            else if (node.NodeSide == NodeSide.SOUTH)
            {
                if (!southernColoredNodes.TryGetValue(node.NodeType, out List<GameNode> _))
                {
                    southernColoredNodes.Add(node.NodeType, new List<GameNode>());
                }
                southernColoredNodes[node.NodeType].Add(node);

                if (northernColoredNodes.TryGetValue(node.NodeType, out List<GameNode> coloredNodes))
                {
                    if (coloredNodes.Count > 0)
                    {
                        if (!nodeConnectionChecks.TryAdd(node.NodeType, true))
                        {
                            nodeConnectionChecks[node.NodeType] = true;
                        }
                    }
                }
            }
        }

        if (node.NodeType == NodeType.YELLOW)
        {
            // OnPlayerMove gets called at the start to setup the yellow/golden nodes as active but any first-to-connect player can win them
            return;
        }

        remainingMoves--;
        Destroy(remainingMoveImages[remainingMoves].gameObject);

        List<NodeType> nodeConnectionChanges = new List<NodeType>();
        foreach (KeyValuePair<NodeType, bool> pair in nodeConnectionChecks)
        {
            //Debug.Log("Checking for " + pair.Key.ToString() + " connection: " + pair.Value);

            if (pair.Value)
            {
                List<GameNode> northernNodes = northernColoredNodes[pair.Key];
                List<GameNode> southernNodes = southernColoredNodes[pair.Key];

                for (int i = 0; i < northernNodes.Count; i++)
                {
                    for (int j = 0; j < southernNodes.Count; j++)
                    {
                        GameNode northernNode = northernNodes[i];
                        GameNode southernNode = southernNodes[j];

                        if (!northernNode.CurrentPlayerIsActive || !southernNode.CurrentPlayerIsActive)
                        {
                            continue;
                        }

                        if (GridManager.CheckConnected(northernNode, southernNode))
                        {
                            northernColoredNodes[pair.Key].RemoveAt(i);
                            southernColoredNodes[pair.Key].RemoveAt(j);

                            if (northernColoredNodes[pair.Key].Count == 0 || southernColoredNodes[pair.Key].Count == 0)
                            {
                                nodeConnectionChanges.Add(pair.Key);
                            }

                            playersScores[CurrentPlayer - 1] += GameNode.MapTypeToValue(pair.Key);
                            activeTextMeshes[CurrentPlayer - 1].SetText(GetUpdatedScore(activeTextMeshes[CurrentPlayer - 1], playersScores[CurrentPlayer - 1]));

                            if (northernNode.NodeType == NodeType.YELLOW && southernNode.NodeType == NodeType.YELLOW)
                            {
                                northernNode.SetOuterColor(CurrentColor);
                                southernNode.SetOuterColor(CurrentColor);
                            }

                            if (northernNode.NodeType == NodeType.CYAN && southernNode.NodeType == NodeType.CYAN)
                            {
                                IsGameOver = true;

                                int maxScore = Mathf.Max(playersScores.ToArray());
                                List<int> maxScoreIndices = new List<int>();
                                for (int x = 0; x < PlayersCount; x++)
                                {
                                    if (playersScores[x] == maxScore)
                                    {
                                        maxScoreIndices.Add(x);
                                    }
                                }

                                GameEvents.GameOverInvoke(maxScoreIndices.ToArray());
                            }
                        }
                    }
                }
            }
        }
        foreach (NodeType type in nodeConnectionChanges)
        {
            nodeConnectionChecks[type] = false;
        }

        if (remainingMoves <= 0)
        {
            activeTextMeshes[CurrentPlayer - 1].fontStyle = FontStyles.Bold;

            CurrentTurn++;
            if (CurrentTurn == PlayersCount)
            {
                IsFirstRound = false;
            }

            if (CurrentTurn % PlayersCount == 0)
            {
                moveSequenceIdx = (moveSequenceIdx + 1) % moveSequence.Length;
            }
            remainingMoves = moveSequence[moveSequenceIdx];

            CurrentPlayer = (CurrentPlayer % PlayersCount) + 1;
            CurrentColor = playerColors[CurrentPlayer - 1];

            // Update UI visuals to reflect current player and remaining moves
            activeTextMeshes[CurrentPlayer - 1].fontStyle = FontStyles.Bold | FontStyles.Underline;
            
            remainingMoveImages.Clear();
            for (int i = 0; i < remainingMoves; i++)
            {
                Image img = Instantiate(moveImage, activeMovesUI[CurrentPlayer - 1].transform);
                img.color = CurrentColor;

                remainingMoveImages.Add(img);
            }
        }
    }
}
