using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public float duration = 4f;

    private void Start()
    {
        Invoke(nameof(BackToMenu), duration);
    }

    private void Update()
    {
        // let the player skip the screen
        if (Time.timeSinceLevelLoad > 1f && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) {
            BackToMenu();
        }
    }

    private void BackToMenu()
    {
        CancelInvoke();
        SceneManager.LoadScene("Menu");
    }

}
