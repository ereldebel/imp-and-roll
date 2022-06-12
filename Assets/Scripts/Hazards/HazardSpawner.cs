using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HazardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject hazard;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float[] rangeX = {-15, 15};
    [SerializeField] private float[] rangeY = {-12, 12};
    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(SpawnEach(timeBetweenSpawns));
    }
    private IEnumerator SpawnEach(float time)
    {
        while (true)
        {
            SpawnCollectable();
            yield return new WaitForSeconds(time);
        }
    }
    public GameObject SpawnCollectable()
    {
        return Instantiate(hazard,
            new Vector3(Random.Range(rangeX[0], rangeX[1]), -2f, Random.Range(rangeY[0], rangeY[1])),
            quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
