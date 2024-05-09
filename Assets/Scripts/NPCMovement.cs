using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    enum npcType { RationalConformist, RationalNonConfirmist, AbundanceGreedy, ScarcityGreedy, PlayerCharacters };


    #region Variables

    GameObject target;
    Vector3 currentPos;
    Vector3 targetPos;
    int foodCount = 0;
    int wayPointIndex = 0;
    bool isFoodNear = false;

    [Header("Basic NPC Settings")]
    [SerializeField] private float speed;
    [SerializeField] private npcType npcArchType;
    [SerializeField] private int increaseSizeAfterFoodCount;
    [SerializeField] private float increaseSizeBy;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (wayPoints.Count == 0)
            Debug.LogWarning("No Waypoints Provided");
        //target = GameObject.FindGameObjectWithTag("Food");
        currentPos = wayPoints[0].transform.position;
        wayPointIndex = Random.Range(0,9);
        //targetPos = wayPoints[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(npcArchType)
        {
            case npcType.RationalConformist:
                MoveTowardsFood();
                break;
            case npcType.RationalNonConfirmist:
                MoveTowardsFood();
                break;
            case npcType.AbundanceGreedy:
                MoveTowardsFood();
                break;
            case npcType.ScarcityGreedy:
                ScarcityGreedyMovement();
                //MoveTowardsFood();
                break;
            case npcType.PlayerCharacters:
                break;
            default:
                Debug.LogWarning("Wrong Archtype");
                break;
        }
    }

    void MoveTowardsFood()
    {
        target = GameObject.FindGameObjectWithTag("Food");

        if (target != null)
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime);
        else if (target == null)
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentPos, speed * Time.deltaTime);
    }


    void ScarcityGreedyMovement()
    {
        targetPos = wayPoints[wayPointIndex].transform.position;
        Vector3 direction = targetPos - this.transform.position;
        if(target == null)
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        else if (isFoodNear && target != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime);
        }
        //this.transform.Translate(direction.normalized * speed * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, targetPos) < 0.1f)
        {
            currentPos = targetPos;
            wayPointIndex= Random.Range(0, wayPoints.Count);
            // add rotation
            // check for collision with other scarcity NPC
        }


        float radius = 2f;
        float distance = 3f;

        RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, radius, Vector2.zero, distance);

        if(hit.collider.gameObject.CompareTag("Food"))
        {
            Debug.LogWarning(this.gameObject.name + ": Detected");
            target = hit.collider.gameObject;
            isFoodNear= true;
        }

        if (hit.collider.gameObject.CompareTag("Scarcity Greedy"))
        {
            Debug.LogWarning(this.gameObject.name+ ": Same Type nearby" + hit.collider.gameObject.name);
        }

    }

    private void OnDrawGizmos()
    {
        float radius = 3f;
        Vector2 center = this.transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, radius);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            foodCount++;
            isFoodNear = false;
            Destroy(collision.gameObject);


            if(foodCount > increaseSizeAfterFoodCount)
            {
                var changeToVector = new Vector3(increaseSizeBy, increaseSizeBy, increaseSizeBy);
                this.transform.localScale += changeToVector; 
            }
        }
    }
}
