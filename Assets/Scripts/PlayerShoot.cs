using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float timeBetweenShots;
    


    bool isfireContinuously;
    bool fireSingle;
    float lastFireTime;

    void Update()
    {
        if (isfireContinuously || fireSingle)
        {
            float timeSinceLastFire = Time.time - lastFireTime;
            if (timeSinceLastFire >= timeBetweenShots && GetComponent<Player>().FoodCount >= 1.25)
            {
                FireBullet();
                lastFireTime = Time.time;
                fireSingle = false;
            }
        }
    }

    void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunOffset.position, transform.rotation);
        Rigidbody2D rb2D = bullet.GetComponent<Rigidbody2D>();
        GetComponent<Player>().FoodCount -= 0.25f;
        rb2D.velocity = bulletSpeed * transform.up;
    }    

    void OnFire(InputValue inputValue)
    {
        isfireContinuously = inputValue.isPressed;

        if(inputValue.isPressed)
        {
            fireSingle = true;
        }
    }
}
