using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script represents of an object to button at specfic speed.

public class FallingObject : MonoBehaviour
{
    [SerializeField]
    public float fallSpeed = 2.0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(0, -fallSpeed);
    }
}
