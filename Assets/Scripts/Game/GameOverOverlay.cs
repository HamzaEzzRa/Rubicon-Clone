using UnityEngine;
using TMPro;

public class GameOverOverlay : MonoBehaviour
{
    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI result;

    public void ReloadGame()
    {
        SceneHandler.Instance.LoadScene(SceneMap.GAME_SCENE, () =>
        {
            GameManager.Instance.NewGame();
        });
    }

    public void LoadMainMenu()
    {
        SceneHandler.Instance.LoadScene(SceneMap.MENU_SCENE, null);
    }

    private void OnGameOver(int[] winnerIds)
    {
        if (winnerIds.Length == 1)
        {
            result.SetText("Player " + (winnerIds[0] + 1) + " Wins!");
        }
        else
        {
            string str = "";
            for (int i = 0; i < winnerIds.Length - 1; i++)
            {
                str += (winnerIds[i] + 1) + ", ";
            }
            result.SetText("Draw Between Players " + str + " and " + (winnerIds[winnerIds.Length - 1] + 1));
        }

        gameOver.SetActive(true);
    }

    private void OnEnable()
    {
        GameEvents.GameOverEvent += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOverEvent -= OnGameOver;
    }
}
