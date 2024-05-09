using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFoodAroundPlayer : MonoBehaviour
{
    [SerializeField] private float spawnAfterTime;
    [SerializeField] private float spawnTimeInterval;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float maxFoodInScene;
    [SerializeField] private float lowVal = 1;
    [SerializeField] private GameObject foodPrefab;


    List<GameObject> foodInScene = new List<GameObject>();

    public float SpawnRadius { get => spawnRadius; set => spawnRadius = value; }
    public float LowVal { get => lowVal; set => lowVal = value; }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnFood", spawnAfterTime, spawnTimeInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnFood()
    {
        if(foodInScene.Count < maxFoodInScene)
        {
            var randomNum = Random.Range(0, 4);
            switch(randomNum)
            {
                case 0:
                    Instantiate(foodPrefab, new Vector3(this.transform.position.x + Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), this.transform.position.y + Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), 0), Quaternion.identity);
                    break;
                case 1:
                    Instantiate(foodPrefab, new Vector3(this.transform.position.x - Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), this.transform.position.y - Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), 0), Quaternion.identity);
                    break;
                case 2:
                    Instantiate(foodPrefab, new Vector3(this.transform.position.x - Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), this.transform.position.y + Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), 0), Quaternion.identity);
                    break;
                case 3:
                    Instantiate(foodPrefab, new Vector3(this.transform.position.x + Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), this.transform.position.y - Mathf.RoundToInt(Random.Range(lowVal, SpawnRadius)), 0), Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, SpawnRadius);
    }
}
