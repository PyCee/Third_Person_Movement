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
            FreezeArrow(collision);
    }
    private void FreezeArrow(Collision collision){
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;

        // Create an empty game object to act as intermediary parent to avoid the arrow inheriting scale and rotation
        GameObject empty = Instantiate(new GameObject());
        empty.transform.SetParent(collision.transform);
        transform.SetParent(empty.transform);
    }
}
