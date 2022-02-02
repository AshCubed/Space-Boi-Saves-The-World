using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravity : MonoBehaviour
{
    public Rigidbody rb;
    public float RandomRotationStrength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RandomRotationStrength = Random.Range(-.5f,0);
        rb.AddForce(new Vector3(0, 0, 1));
        transform.Rotate(RandomRotationStrength, RandomRotationStrength, RandomRotationStrength);
    }
}
