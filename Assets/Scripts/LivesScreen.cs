using UnityEngine;
using UnityEngine.UI;

public class LivesScreen : MonoBehaviour
{
    public Text worldText;
    public Text livesText;
    public float duration = 2.5f;

    private void Start()
    {
        GameManager game = GameManager.Instance;

        worldText.text = $"MUNDO {game.world}-{game.stage}";
        livesText.text = $"x  {game.lives}";

        Invoke(nameof(StartLevel), duration);
    }

    private void StartLevel()
    {
        GameManager.Instance.StartLevel();
    }

}
