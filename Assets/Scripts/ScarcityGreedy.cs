using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarcityGreedy : MonoBehaviour
{
    #region
    enum state { patrol, gotofood, chase, attack };

    GameObject target;
    Vector3 targetPos;
    state State = state.patrol;
    int wayPointIndex = 0;
    float foodCount = 1;
    float latchOnTimer;
    bool isLatchedOn = false;
    float numberOfTimeLatchedOn = 0;
    Vector3 currentPos;
    float speedCop;
    float foodCountCop;
    float baseScale;


    [Header("Basic NPC Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float disFromNextWaypoint = 1.0f;
    [SerializeField] private float checkRadiusSize = 5f;
    [SerializeField] private float disFromTarget = 1.0f;
    [SerializeField] private float increaseSizeAfterFoodCount;
    [SerializeField] private float increaseSizeBy;
    [SerializeField] private float latchOnTime;
    [SerializeField] private float respawnAfterTimer = 1.0f;
    [SerializeField] private float degradationRate = 0.001f;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();

    public Animator animator;
    public float FoodCount { get => foodCount; set => foodCount = value; }
    public Vector3 TargetPos { get => targetPos; set => targetPos = value; }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        baseScale = this.transform.localScale.x;
        speedCop = speed;
        foodCountCop = foodCount;
        currentPos = this.transform.position;
        if (wayPoints.Count == 0)
            Debug.LogWarning("No Waypoints Provided");
        wayPointIndex = Random.Range(0, wayPoints.Count);
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case state.patrol:
                Patrol();
                break;
            case state.gotofood:
                GoToFood();
                break;
            case state.chase:
                Chase();
                break;
            case state.attack:
                Attack();
                break;
            default:
                break;
        }

        //death condition and leaving food morsels

        if (foodCount > 0)
        {
            foodCount -= degradationRate;
            //changeAlphaAccordingToFoodCount(foodCount);
        }
        if (foodCount < 0)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            speed = 0;
            StartCoroutine(RespawnAfterTime());
        }
        if (foodCount >= 1)
        {
            var excessFood = foodCount - 1;
            var increaseSizeByVal = increaseSizeBy * excessFood;
            var exceededSize = new Vector3(baseScale + increaseSizeByVal, baseScale + increaseSizeByVal, baseScale + increaseSizeByVal);
            this.transform.localScale = exceededSize;
        }

        if(Mathf.RoundToInt(foodCount) == 0)
        {
            this.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
        }

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
        //animator.SetBool("isAttacked", false);
        animator.SetBool("isChasing", false);
        latchOnTimer = 0;
        numberOfTimeLatchedOn = 0;
        isLatchedOn = false;
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
                if (c.tag == "Food")
                {
                    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                    target = c.gameObject;
                    targetPos = target.transform.position;
                    State = state.gotofood;
                }
                if(c.tag == "Scarcity Greedy")
                {
                    //check their sizes and then start chasing them
                    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                    if (Mathf.RoundToInt(this.foodCount) > Mathf.RoundToInt(c.GetComponent<ScarcityGreedy>().FoodCount))
                    {
                        return;
                    }
                    if (Mathf.RoundToInt(this.foodCount) < Mathf.RoundToInt(c.GetComponent<ScarcityGreedy>().FoodCount) && (Mathf.RoundToInt(c.GetComponent<ScarcityGreedy>().FoodCount) - Mathf.RoundToInt(this.foodCount) >= 2 ))
                    {
                        target = c.gameObject;
                        State = state.chase;
                    }
                }
                if (c.tag == "Player")
                {
                    //check their sizes and then start chasing them
                    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.transform.parent.gameObject.name);
                    if (Mathf.RoundToInt(this.foodCount) > Mathf.RoundToInt(c.GetComponentInParent<Player>().FoodCount))
                    {
                        return;
                    }
                    if (Mathf.RoundToInt(this.foodCount) < Mathf.RoundToInt(c.GetComponentInParent<Player>().FoodCount) && (Mathf.RoundToInt(c.GetComponentInParent<Player>().FoodCount) - Mathf.RoundToInt(this.foodCount) >= 2))
                    {
                        target = c.gameObject.transform.parent.gameObject;
                        State = state.chase;
                    }
                }
            }
        }
    }

    void GoToFood()
    {
        //go towards food
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        RotateTowards(targetPos);
        if (target == null)
        {
            wayPointIndex = Random.Range(0, wayPoints.Count);
            State = state.patrol;
        }
    }

    void Chase()
    {
        animator.SetBool("isChasing", true);
        //run towards the objects
        if (Vector3.Distance(this.transform.position, target.transform.position) > disFromTarget)
        { 
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * 1.1f * Time.deltaTime);
            RotateTowards(target.transform.position);
        }
        if (Vector3.Distance(this.transform.position, target.transform.position) > checkRadiusSize)
        {
            target = null;
            State = state.patrol;
        }
        if (target != null && Vector3.Distance(this.transform.position, target.transform.position) < disFromTarget)
        {
            //this.transform.SetParent(target.transform.GetChild(0).transform);
            this.transform.position = target.transform.GetChild(0).transform.position;
            isLatchedOn = true;
            State = state.attack;
        }
    }

    void Attack()
    {
        animator.SetBool("isChasing", false);
        //latching on a attack
        latchOnTimer += Time.deltaTime;
        if (isLatchedOn && numberOfTimeLatchedOn == 0)
        {
            if (target.tag == "Scarcity Greedy")
            {
                if (target.GetComponent<ScarcityGreedy>().FoodCount >= increaseSizeAfterFoodCount)
                {
                    FoodCount++;
                    target.GetComponent<ScarcityGreedy>().FoodCount--;
                }
                numberOfTimeLatchedOn++;
            }
            if(target.tag == "Player")
            {
                if (target.GetComponent<Player>().FoodCount >= increaseSizeAfterFoodCount)
                {
                    FoodCount++;
                    target.GetComponent<Player>().FoodCount--;
                }
                numberOfTimeLatchedOn++;
            }
        }

        if(isLatchedOn && (latchOnTimer < latchOnTime) && target.tag == "Scarcity Greedy")
        {
            this.transform.position = target.transform.GetChild(0).transform.position;
            RotateTowards(new Vector3 (-target.GetComponent<ScarcityGreedy>().TargetPos.x, -target.GetComponent<ScarcityGreedy>().TargetPos.y, 0));
            target.GetComponent<ScarcityGreedy>().animator.SetBool("isAttacked", true);
        }

        if (isLatchedOn && (latchOnTimer < latchOnTime) && target.tag == "Player")
        {
            this.transform.position = target.transform.GetChild(0).transform.position;
            print(-target.GetComponent<Player>().transform.position.x);
            RotateTowards(new Vector3(target.GetComponent<Player>().transform.position.x, target.GetComponent<Player>().transform.position.y, 0));
        }

        if (latchOnTimer >= latchOnTime)
        {
            isLatchedOn = false;
            //this.transform.SetParent(null);
            if (!isLatchedOn && target != null && target.tag == "Scarcity Greedy")
            {
                target.GetComponent<ScarcityGreedy>().animator.SetBool("isAttacked", false);
            }
            targetPos = wayPoints[wayPointIndex].transform.position;
            RotateTowards(targetPos);
            target = null;
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
            if (Vector3.Distance(this.transform.position, targetPos) < disFromNextWaypoint)
            {
                State = state.patrol;
            }
        }
    }

    public void ResetStats() //reset all stats after death
    {
        this.transform.position = currentPos;
        this.GetComponent<SpriteRenderer>().enabled = true;
        //changeAlphaAccordingToFoodCount(1);
        this.GetComponent<PolygonCollider2D>().enabled = true;
        this.transform.localScale= new Vector3(baseScale, baseScale, baseScale);
        foodCount = foodCountCop;
        speed = speedCop;
        target = null;
        State = state.patrol;
    }

    public void startRespwan()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<PolygonCollider2D>().enabled = false;
        speed = 0;
        StartCoroutine(RespawnAfterTime());
    }


    IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnAfterTimer);
        ResetStats();
    }

    private void OnDrawGizmos()
    {
        float radius = checkRadiusSize;
        Vector2 center = this.transform.position;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.tag == "Food")
        {
            Destroy(collision.gameObject);
            FoodCount++;
            if (FoodCount > increaseSizeAfterFoodCount)
            {
                //var changeToVector = new Vector3(increaseSizeBy, increaseSizeBy, increaseSizeBy);
                //this.transform.localScale += changeToVector;
                checkRadiusSize += increaseSizeBy;
            }
        }
    }
}
