using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRotate : MonoBehaviour
{
    private int _force = 100;
    private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        int num = Random.Range(0, 3);
        if (num == 0)
        {
            _rigidbody.AddForce(new Vector3(_force, 0, 0));
            _rigidbody.AddTorque(new Vector3(_force, 0, 0));
        }
        else if (num == 1)
        {
            _rigidbody.AddForce(new Vector3(0, _force, 0));
            _rigidbody.AddTorque(new Vector3(0, _force, 0));
        }
        else if (num == 2)
        {
            _rigidbody.AddForce(new Vector3(0, 0, _force));
            _rigidbody.AddTorque(new Vector3(0, 0, _force));
        }
    }
}
