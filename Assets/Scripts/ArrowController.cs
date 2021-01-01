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
            FreezeArrow(collision.gameObject);
    }
    private void FreezeArrow(GameObject go){
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        // print(go.name);
        transform.SetParent(go.transform);
    }
}
