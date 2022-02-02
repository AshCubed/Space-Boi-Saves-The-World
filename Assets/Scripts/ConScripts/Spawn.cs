using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public int indexSelector = 0;
    public GameObject[] asteroids = new GameObject[3];
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObject",spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        indexSelector = Random.Range(0, 3);

        Instantiate(asteroids[indexSelector], new Vector3(transform.position.x, transform.position.y + Random.Range(-3,3), transform.position.z), new Quaternion(Random.Range (0,361), Random.Range(0, 361), Random.Range(0, 361) , 0));

        if (stopSpawning)
        {

        }
    }

}
