using UnityEngine;

// Fish swim to the left waving up and down. Jellyfish rise towards mario
// in short bursts and then sink slowly.
public class SeaEnemy : MonoBehaviour
{
    public enum Kind
    {
        Fish,
        Jellyfish,
    }

    public Kind kind;
    public float speed = 2.5f;
    public float amplitude = 0.75f;
    public float minHeight = 1f;
    public float maxHeight = 11f;

    private Transform player;
    private Vector3 startPosition;
    private float time;

    private void Awake()
    {
        startPosition = transform.position;
        enabled = false;
    }

    private void Start()
    {
        Player mario = FindFirstObjectByType<Player>();

        if (mario != null) {
            player = mario.transform;
        }
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        // fish that already crossed the screen are gone for good
        if (kind == Kind.Fish && enabled) {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (kind == Kind.Fish) {
            Swim();
        } else {
            Drift();
        }
    }

    private void Swim()
    {
        Vector3 position = transform.position;
        position.x -= speed * Time.deltaTime;
        position.y = startPosition.y + Mathf.Sin(time * 2f) * amplitude;
        transform.position = position;
    }

    private void Drift()
    {
        Vector3 position = transform.position;
        bool rising = time % 2f < 0.6f;

        if (rising && player != null)
        {
            position.y += speed * 1.5f * Time.deltaTime;
            position.x = Mathf.MoveTowards(position.x, player.position.x, speed * Time.deltaTime);
        }
        else
        {
            position.y -= speed * 0.4f * Time.deltaTime;
        }

        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.starpower) {
                Hit();
            } else {
                player.Hit();
            }
        }
    }

    private void Hit()
    {
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        AudioManager.Play("Patada");
        enabled = false;
        Destroy(gameObject, 3f);
    }

}
