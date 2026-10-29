using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (!player.dead)
            {
                // bring mario back to the bottom of the screen so the jump is visible
                Camera camera = Camera.main;
                Vector3 position = player.transform.position;
                position.y = camera.transform.position.y - camera.orthographicSize - 0.5f;
                player.transform.position = position;

                player.deathAnimation.jumpVelocity = 20f;
                player.Death();
            }
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

}
