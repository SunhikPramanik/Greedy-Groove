using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    enum state { patrol, gotoPlayer, chase };

    GameObject target;
    Vector3 targetPos;
    state State = state.patrol;
    int wayPointIndex = 0;
    Animator animator;


    [Header("Basic NPC Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float disFromNextWaypoint = 1.0f;
    [SerializeField] private float checkRadiusSize = 5f;
    [SerializeField] private float disFromTarget = 1.0f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float thresholdFoodValue = 1f;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();

    public GameObject Target { get => target; set => target = value; }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case state.patrol:
                Patrol();
                break;
            case state.gotoPlayer:
                GoToPlayer();
                break;
            case state.chase:
                Chase();
                break;
            default:
                break;
        }
    }
    //run after npc who are larger than particular size


    void RotateTowards(Vector3 targetLocation)
    {
        Vector3 direction = this.transform.position - targetLocation;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);
    }


    void Patrol()
    {
        targetPos = wayPoints[wayPointIndex].transform.position;
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        //movement
        if (Target == null)
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, targetPos) < disFromNextWaypoint)
        {
            wayPointIndex = Random.Range(0, wayPoints.Count);
        }
        if (Target != null && Target.tag == "Player")
        {
            targetPos = Target.transform.position;
            State = state.gotoPlayer;
        }

        //rotation
        RotateTowards(targetPos);

        //interaction with other objects around
        Collider2D[] targetsColliding = Physics2D.OverlapCircleAll(this.transform.position, checkRadiusSize);

        foreach (Collider2D c in targetsColliding)
        {
            if (c != this.GetComponent<Collider2D>())
            {
                if (c.tag == "Player")
                {
                    //Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                    Target = c.gameObject.transform.parent.gameObject;
                    targetPos = Target.transform.position;
                    State = state.chase;
                }
                if (c.tag == "Scarcity Greedy")
                {
                    if(c.GetComponent<ScarcityGreedy>().FoodCount >= thresholdFoodValue)
                    {
                        Target = c.gameObject;
                        targetPos = Target.transform.position;
                        State = state.chase;
                    }
                }
            }
        }


    }

    void GoToPlayer()
    {
        animator.SetBool("isChasing", true);
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
        RotateTowards(targetPos);
        if (Vector3.Distance(this.transform.position, targetPos) <= 0)
        {
            Collider2D[] targetsColliding = Physics2D.OverlapCircleAll(this.transform.position, checkRadiusSize);
            foreach (Collider2D c in targetsColliding)
            {
                if (c != this.GetComponent<Collider2D>())
                {
                    
                    if (c.tag == "Player")
                    {
                        print(c.name);
                        Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                        Target = c.gameObject.transform.parent.gameObject;
                        targetPos = Target.transform.position;
                        State = state.chase;
                    }
                    else
                    {
                        print(c.name);
                        target = null;
                        wayPointIndex = Random.Range(0, wayPoints.Count);
                        State = state.patrol;
                    }
                }
            }
        }
    }

    void Chase()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("isChasing", false);
        this.transform.position = Vector3.MoveTowards(this.transform.position, Target.transform.position, speed * 1.25f * Time.deltaTime);
        RotateTowards(target.transform.position);
        if (Vector3.Distance(this.transform.position, target.transform.position) > checkRadiusSize && Vector3.Distance(this.transform.position, target.transform.position) < maxDistance)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * 1.25f * Time.deltaTime);
            RotateTowards(target.transform.position);
        }
        if (Vector3.Distance(this.transform.position, target.transform.position) > maxDistance)
        {
            wayPointIndex = Random.Range(0, wayPoints.Count);
            target = null;
            State = state.patrol;
        }
    }


    private void OnDrawGizmos()
    {
        float radius = checkRadiusSize;
        Vector2 center = this.transform.position;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, radius);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((State == state.chase) && collision.gameObject.GetComponent<Collider2D>() == target.GetComponent<Collider2D>())
        {
            if (target.tag == "Scarcity Greedy")
            {
                print("killed");
                target.GetComponent<ScarcityGreedy>().startRespwan();
                target = null;
                wayPointIndex = Random.Range(0, wayPoints.Count);
                State = state.patrol;
            }
        }
    }

    //oncollision destroy objects
}
