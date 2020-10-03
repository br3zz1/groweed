using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public float playerSpeed;
    public GameObject player;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(moveX * playerSpeed, moveY * playerSpeed);
        Vector3 pos = player.transform.position;
        Vector3 newPos = new Vector3(pos.x, pos.y, pos.y / 100);
        player.transform.position = newPos;
    }
}
