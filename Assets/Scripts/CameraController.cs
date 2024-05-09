using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 cameraOffset;
    public float cameraSpeed = 0.1f;

    public Vector2 maxBounds;
    public Vector2 minBounds;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        cameraOffset = transform.position - player.position;
        transform.position = player.position + cameraOffset;
    }

    void Update()
    {
        Vector3 finalPosition = player.position + cameraOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, cameraSpeed);

        lerpPosition.x = Mathf.Clamp(lerpPosition.x, minBounds.x, maxBounds.x);
        lerpPosition.y = Mathf.Clamp(lerpPosition.y, minBounds.y, maxBounds.y);

        transform.position = lerpPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, transform.position.z), new Vector3(maxBounds.x, minBounds.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, transform.position.z), new Vector3(minBounds.x, maxBounds.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, transform.position.z), new Vector3(minBounds.x, maxBounds.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, transform.position.z), new Vector3(maxBounds.x, minBounds.y, transform.position.z));
    }
}
