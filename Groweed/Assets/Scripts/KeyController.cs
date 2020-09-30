using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public float playerSpeed;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        player.transform.position += new Vector3(moveX,moveY,0) * playerSpeed * Time.deltaTime;
    }
}
