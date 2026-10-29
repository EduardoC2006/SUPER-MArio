using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    public Color undergroundColor = Color.black;

    private Color surfaceColor;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;
        surfaceColor = camera.backgroundColor;
    }

    private void Start()
    {
        // start at the right height for where the player spawns
        SetUnderground(player.position.y < undergroundThreshold);
    }

    private void LateUpdate()
    {
        // track the player moving to the right
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;

        camera.backgroundColor = underground ? undergroundColor : surfaceColor;
    }

}
