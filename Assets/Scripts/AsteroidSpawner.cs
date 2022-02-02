using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider colliderArea;
    [SerializeField] private int numOfObjectsToSpawn;
    [SerializeField] private List<GameObject> asteroidsToSpawn;

    Vector3 cubeSize;
    Vector3 cubeCenter;

    private void Awake()
    {
        Transform cubeTrans = colliderArea.gameObject.GetComponent<Transform>();
        cubeCenter = cubeTrans.position;

        // Multiply by scale because it does affect the size of the collider
        cubeSize.x = cubeTrans.localScale.x * colliderArea.size.x;
        cubeSize.y = cubeTrans.localScale.y * colliderArea.size.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnAllObjects();
    }

    //Responsible for spawning all objects in scene
    public void SpawnAllObjects()
    {
        for (int i = 0; i < numOfObjectsToSpawn; i++)
        {
            //objectsToSpawn.Count - 1
            GameObject newGo = Instantiate(asteroidsToSpawn[Random.Range(0, asteroidsToSpawn.Count)],
                GetRandomPosition(), Quaternion.identity, this.transform);
            newGo.GetComponent<ZeroGravity>().enabled = false;
            newGo.GetComponent<AsteroidRotate>().enabled = true;
/*            if (newGo.GetComponent<MeteorRotate>() == null)
            {
                newGo.AddComponent<MeteorRotate>();
            }*/
        }
    }

    //Returns random position within a box collider
    private Vector3 GetRandomPosition()
    {
        // You can also take off half the bounds of the thing you want in the box, so it doesn't extend outside.
        // Right now, the center of the prefab could be right on the extents of the box
        Vector3 randomPosition = new Vector3(
            Random.Range(-cubeSize.x / 2, cubeSize.x / 2),
            Random.Range(-cubeSize.y / 2, cubeSize.y / 2),
            Random.Range(-cubeSize.y / 2, cubeSize.y / 2));
        return cubeCenter + randomPosition;
    }
}
