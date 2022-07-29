using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grouded : MonoBehaviour
{
    // Start is called before the first frame update
    
    public bool isGrounded;
    BoxCollider2D boxCollider2d;

    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerExit(Collider coll)
    {
        isGrounded = false;
        
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("dwdw");
        isGrounded = true;
    }
}
