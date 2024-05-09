using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Camera b_camera;

    private void Awake()
    {
        b_camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check for tag and destroy
        Destroy(gameObject);
    }

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = b_camera.WorldToScreenPoint(transform.position);

        if(screenPosition.x < 0 || screenPosition.x > b_camera.pixelWidth || screenPosition.y < 0 || screenPosition.y > b_camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}
