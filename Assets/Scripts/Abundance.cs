using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abundance : MonoBehaviour
{
    enum state { patrol, gotofood, chase, attack };

    GameObject target;
    Vector3 targetPos;
    Vector3 currentPos;
    state State = state.patrol;
    int wayPointIndex = 0;
    float foodCount = 1;
    float healthtCop;
    float speedCop;
    Animator animator;
    float attackIntervalTime;
    bool hasAttacked = false;


    [Header("Basic NPC Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float disFromNextWaypoint = 1.0f;
    [SerializeField] private float checkRadiusSize = 5f;
    [SerializeField] private float disFromTarget = 1.0f;
    [SerializeField] private float increaseSizeAfterFoodCount;
    [SerializeField] private float increaseSizeBy;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float maxDistance = 1f;
    [SerializeField] private float attackInterval;
    [SerializeField] private float degradationRate = 0.001f;
    [SerializeField] private float pushForce;
    [SerializeField] private float respawnAfterTimer = 1.0f;
    [SerializeField] private float health = 1;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject blowPrefab;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();



    public float FoodCount { get => foodCount; set => foodCount = value; }



    // Start is called before the first frame update
    void Start()
    {
        healthtCop = health;
        animator = GetComponent<Animator>();
        speedCop = speed;
        currentPos = this.transform.position;
        if (wayPoints.Count == 0)
            Debug.LogWarning("No Waypoints Provided");
        wayPointIndex = Random.Range(0, 9);
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case state.patrol:
                Patrol();
                break;
            //case state.gotofood:
            //    GoToFood();
                //break;
            case state.attack:
                Attack();
                break;
            case state.chase:
                Chase();
                break;
            default:
                break;
        }
        if (attackInterval <= attackIntervalTime)
        {
            attackIntervalTime = 0;
            hasAttacked = false;
        }
        //if (foodCount >= 1)
        //{
        //    var excessFood = foodCount - 1;
        //    var increaseSizeByVal = increaseSizeBy * excessFood;
        //    var exceededSize = new Vector3(baseScale + increaseSizeByVal, baseScale + increaseSizeByVal, baseScale + increaseSizeByVal);
        //    this.transform.localScale = exceededSize;
        //}

        //if(foodCount > 1)
        //{
        //    foodCount-= degradationRate;
        //}

        if (health <= 0)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            speed = 0;
            hasAttacked = true;
            StartCoroutine(RespawnAfterTime());
        }
        //death condition and leaving food morsels
    }

    void RotateTowards(Vector3 targetLocation)
    {
        Vector3 direction = this.transform.position - targetLocation;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);
    }


    void Patrol()
    {
        attackIntervalTime = 0;
        hasAttacked = false;
        animator.SetBool("isRunning", false);
        targetPos = wayPoints[wayPointIndex].transform.position;

        //movement
        if (target == null)
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, targetPos) < disFromNextWaypoint)
        {
            wayPointIndex = Random.Range(0, wayPoints.Count);
        }

        //rotation
        RotateTowards(targetPos);

        //interaction with other objects around
        Collider2D[] targetsColliding = Physics2D.OverlapCircleAll(this.transform.position, checkRadiusSize);

        foreach (Collider2D c in targetsColliding)
        {
            if (c != this.GetComponent<Collider2D>())
            {
                //if (c.tag == "Food" && foodCount<=2)
                //{
                //    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                //    target = c.gameObject;
                //    targetPos = target.transform.position;
                //    State = state.gotofood;
                //}
                if (c.tag == "Player")
                {
                    target = c.gameObject;
                    targetPos = target.transform.position;
                    State = state.chase;
                    //check if thats a player nearby and push them away
                }
            }
        }


    }

    void Chase()
    {
        attackIntervalTime = 0;
        hasAttacked = false;
        animator.SetBool("isRunning", true);
        this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * 1.25f * Time.deltaTime);
        RotateTowards(target.transform.position);
        if (Vector3.Distance(this.transform.position, target.transform.position) > checkRadiusSize && Vector3.Distance(this.transform.position, target.transform.position) < maxDistance)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * 1.25f * Time.deltaTime);
            RotateTowards(target.transform.position);
        }
        if (Vector3.Distance(this.transform.position, target.transform.position) < 5.0f)
        {
            State = state.attack;
        }
        if (Vector3.Distance(this.transform.position, target.transform.position) > maxDistance)
        {
            wayPointIndex = Random.Range(0, wayPoints.Count);
            target = null;
            State = state.patrol;
        }
    }

    void GoToFood()
    {
        //go towards food
        //animator.SetBool("isRunning", true);
        //this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        //RotateTowards(targetPos);
        //if (target == null)
        //{
        //    wayPointIndex = Random.Range(0, wayPoints.Count);
        //    State = state.patrol;
        //}
    }

    void Attack()
    {
        //animator.SetTrigger("isAttacking");
        //animator.SetBool("isRunning", false);



        //if (foodCount <= 2)
        //    State = state.patrol;
        PushAttack();
        if (Vector3.Distance(this.transform.position, target.transform.position) > checkRadiusSize && Vector3.Distance(this.transform.position, target.transform.position) < maxDistance)
        {
            State = state.chase;
        }
        RotateTowards(target.transform.position);
    }

    void PushAttack()
    {
        if (attackIntervalTime == 0)
        {
            StartCoroutine(PushAttackCoroutine());
            hasAttacked = true;
        }
        if (hasAttacked)
        {
            attackIntervalTime += Time.deltaTime;
        }
    }

    IEnumerator PushAttackCoroutine()
    {
        if (health > 0)
        {
            animator.SetBool("isRunning", false);
            yield return new WaitForSeconds(.5f);

            var gO = Instantiate(blowPrefab, spawnPoint.transform.position, spawnPoint.rotation);
            Rigidbody2D gOrb = gO.GetComponent<Rigidbody2D>();
            gOrb.AddForce(-spawnPoint.transform.right * 200f, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmos()
    {
        float radius = checkRadiusSize;
        Vector2 center = this.transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, radius);
    }

    IEnumerator waitAfterFeeding()
    {
        var tempSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(waitTime);
        speed = tempSpeed;
    }

    IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnAfterTimer);
        ResetStats();
    }


    public void ResetStats()
    {
        this.transform.position = currentPos;
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<Collider2D>().enabled = true;
        foodCount = 1;
        speed = speedCop;
        target = null;
        health = healthtCop;
        State = state.patrol;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if(collision.tag == "Bullet")
        {
            health--;
            if(health < 1)
            {
                Instantiate(foodPrefab, this.transform.position, Quaternion.identity);
            }
        }
    }
}
