using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float thrust = 10f;
    public float lifespan = 3f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //apply force to the projectile as soon as it spawns
        rb.AddForce(transform.forward * thrust, ForceMode.Impulse);
        rb.useGravity = false;
        //destroy projectile after a set amount of time
        Destroy(gameObject, lifespan);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //destroy projectile after colliding with anything
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
