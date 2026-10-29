using System.Collections;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    public GameObject item;
    public Sprite emptyBlock;
    public int maxHits = -1;
    public bool breakable;
    private bool animating;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.DotTest(transform, Vector2.up))
            {
                Player player = collision.gameObject.GetComponent<Player>();

                // big mario breaks empty bricks
                if (breakable && item == null && player != null && player.big) {
                    Break();
                } else {
                    Hit();
                }
            }
        }
    }

    private void Hit()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true; // show if hidden

        maxHits--;

        if (maxHits == 0) {
            spriteRenderer.sprite = emptyBlock;
        }

        AudioManager.Play("Golpe");

        if (item != null)
        {
            Instantiate(item, transform.position, Quaternion.identity);

            if (item.GetComponent<BlockCoin>() == null) {
                AudioManager.Play("Aparece");
            }
        }

        StartCoroutine(Animate());
    }

    private void Break()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Vector2[] velocities = {
            new Vector2(-3f, 12f), new Vector2(3f, 12f),
            new Vector2(-3f, 8f), new Vector2(3f, 8f),
        };

        foreach (Vector2 velocity in velocities)
        {
            GameObject piece = new GameObject("Brick Piece");
            piece.transform.position = transform.position + (Vector3)(velocity.normalized * 0.25f);
            piece.transform.localScale = Vector3.one * 0.5f;

            SpriteRenderer renderer = piece.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 5;

            Rigidbody2D rigidbody = piece.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 4f;
            rigidbody.linearVelocity = velocity;
            rigidbody.angularVelocity = velocity.x * 120f;

            Destroy(piece, 2f);
        }

        AudioManager.Play("Ladrillo");
        Destroy(gameObject);
    }

    private IEnumerator Animate()
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;

        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

}
