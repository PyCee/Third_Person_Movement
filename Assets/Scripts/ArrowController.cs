using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("Player"))
            return;
        else
            FreezeArrow();
    }
    private void FreezeArrow(){
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        // print(collision.gameObject.name);
    }
}
