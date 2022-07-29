using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    Bounds playerBounds;
    public BoxCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        playerBounds = collider.bounds;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(collider.transform.position.x + player.transform.localScale.x * playerBounds.size.x, collider.transform.position.y, -10);
    }
}