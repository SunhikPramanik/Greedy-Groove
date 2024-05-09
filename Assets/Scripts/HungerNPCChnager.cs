using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerNPCChnager : MonoBehaviour
{
    public Player player;
    public Hunger hunger;
    public Animator animator;

    private void Start()
    {
        animator.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            print("hi");
            player.p_hungerNPC = hunger;
            animator.gameObject.SetActive(true);
        }
    }
}
