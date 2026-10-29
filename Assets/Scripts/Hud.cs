using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Text livesText;
    public Text coinsText;
    public Text worldText;

    private void Update()
    {
        GameManager game = GameManager.Instance;

        if (game == null) {
            return;
        }

        livesText.text = $"MARIO\n  x{game.lives}";
        coinsText.text = $"x{game.coins:00}";
        worldText.text = $"MUNDO\n {game.world}-{game.stage}";
    }

}
