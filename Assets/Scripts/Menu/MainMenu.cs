using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public int PlayersCount { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        MenuSelection.menuSelectionList.Find(x => x.Id == 0).OnPointerDown(null);
    }

    public void LoadGame()
    {
        SceneHandler.Instance.LoadScene(SceneMap.GAME_SCENE, () => {
            GameManager.Instance.PlayersCount = PlayersCount;
            GameManager.Instance.NewGame();
        });
    }

    public void UpdatePlayersCount(int newCount)
    {
        PlayersCount = newCount;
    }
}
