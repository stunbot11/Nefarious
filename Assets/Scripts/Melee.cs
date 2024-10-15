using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public bool isPlayer = false;

    private Transform player;

    private void Start()
    {
        if (!isPlayer)
        {
            player = GameObject.Find("Player").transform;
        }
    }
    void Update()
    {
        if (isPlayer)
        {
            Vector2 pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.right = (pointerPos - (Vector2)transform.position).normalized;
        }
        else
        {
            Vector2 targetPos = player.position - transform.position;
            transform.right = targetPos.normalized;
        }
    }
}
