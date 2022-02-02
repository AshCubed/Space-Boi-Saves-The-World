using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishMoveTo : MonoBehaviour
{
    private Transform planetPosToMoveTo;

    // Update is called once per frame
    void Update()
    {
        MoveTo();
    }

    public void SetMoveToPos(Transform moveToPos)
    {
        planetPosToMoveTo = moveToPos;
    }

    private void MoveTo()
    {
        if (planetPosToMoveTo)
        {
            StopAllCoroutines();
            transform.position =
                Vector3.Lerp
                    (transform.position, planetPosToMoveTo.transform.position, .05f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Planet"))
        {
            Destroy(gameObject);
        }
    }
}
