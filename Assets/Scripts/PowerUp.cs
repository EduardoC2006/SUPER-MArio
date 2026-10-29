using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type
    {
        Coin,
        ExtraLife,
        MagicMushroom,
        Starpower,
    }

    public Type type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        switch (type)
        {
            case Type.Coin:
                GameManager.Instance.AddCoin();
                AudioManager.Play("Moneda");
                break;

            case Type.ExtraLife:
                GameManager.Instance.AddLife();
                AudioManager.Play("VidaExtra");
                break;

            case Type.MagicMushroom:
                player.GetComponent<Player>().Grow();
                AudioManager.Play("Crecer");
                break;

            case Type.Starpower:
                player.GetComponent<Player>().Starpower();
                AudioManager.Play("Crecer");
                break;
        }

        Destroy(gameObject);
    }

}
