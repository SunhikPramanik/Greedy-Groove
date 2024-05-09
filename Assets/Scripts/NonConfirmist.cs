using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class NonConfirmist : MonoBehaviour
{
    #region
    enum state { patrol, run };

    GameObject target;
    Vector3 targetPos;
    Vector3 currentPos;
    float speedCop;
    float foodCountCop;
    int wayPointIndex = 0;
    state State = state.patrol;
    Animator animator;
    SpriteFlash spriteFlash;
    Player player;


    [Header("Basic NPC Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float disFromNextWaypoint = 1.0f;
    [SerializeField] private float checkRadiusSize = 5f;
    [SerializeField] private float disFromTarget = 1.0f;
    [SerializeField] private float respawnAfterTimer = 1.0f;
    [SerializeField] private float degradationRate = 0.1f;
    [SerializeField] private float foodCount = 1.0f;
    [SerializeField] private float flashDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private Color flashColor;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        speed = Random.Range(2, 4.5f);
        spriteFlash = GetComponent<SpriteFlash>();
        animator = GetComponent<Animator>();
        currentPos = this.transform.position;
        speedCop = speed;
        foodCountCop = foodCount;
        if (wayPoints.Count == 0)
            Debug.LogWarning("No Waypoints Provided");
        wayPointIndex = Random.Range(0, wayPoints.Count);
    }

    // Update is called once per frame
    void Update()
    {
        switch(State)
        {
            case state.patrol:
                Patrol();
                break;
            case state.run:
                Run();
                break;
            default:
                break;
        }

        //death condition & leaving food morsel on its way, also non confirmist can drop certain stuffs that will help the players go up the ladder
        if(foodCount>0)
        {
            foodCount -= degradationRate;
            //changeAlphaAccordingToFoodCount(foodCount);
        }
        if(foodCount < 0.2f)
        {
            spriteFlash.CallCoroutine(flashDuration, flashColor, numberOfFlashes);
        }
        if(foodCount < 0)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            speed = 0;
            StartCoroutine(RespawnAfterTime());
        }
    }

    void changeAlphaAccordingToFoodCount(float foodC)
    {
        if (foodC <= 1)
        {
            var objAlpha = this.GetComponent<SpriteRenderer>().color;
            objAlpha.a = foodC;
            this.GetComponent<SpriteRenderer>().color = objAlpha;
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
                //if (/*c.tag == "Food" || c.tag == "Non Confirmist"*/ ||  c.tag == "Scarcity Greedy" || c.tag == "Abundance" || c.tag == "Confirmist" || c.tag == "Hunger")
                //{
                //    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                //    target = c.gameObject;
                //    targetPos =new Vector3 (Random.Range(-target.transform.position.x, target.transform.position.x), Random.Range(-target.transform.position.y, target.transform.position.y), Random.Range(-target.transform.position.z, target.transform.position.z));
                //    State = state.run;
                //}

                if (c.tag == "Player")
                {
                    Debug.LogWarning(this.gameObject.name + ": Detected " + c.gameObject.name);
                    target = c.gameObject.transform.parent.gameObject;
                    targetPos = this.transform.position - target.transform.position;
                    State = state.run;
                }
            }
        }


    }

    //few might run away at full speed, others will move slowly

    void Run()
    {
        animator.SetBool("isRunning", true);
        //run away from objects
        this.transform.Translate(targetPos.normalized * speed * 1.25f * Time.deltaTime);

        Vector3 direction = new Vector3(-targetPos.x, -targetPos.y, 0);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);

        if (target == null)
        { 
            State = state.patrol;
        }
        else if (target != null && (Vector3.Distance(this.transform.position, target.transform.position) > disFromTarget))
        {
            target = null;
            wayPointIndex = Random.Range(0, wayPoints.Count);
            State = state.patrol;
        }
        if (target == null)
            State = state.patrol;
    }

    public void ResetStats()
    {
        this.transform.position = currentPos;
        this.GetComponent<SpriteRenderer>().enabled = true;
        changeAlphaAccordingToFoodCount(1);
        this.GetComponent<CapsuleCollider2D>().enabled = true;
        foodCount = foodCountCop;
        speed = speedCop;
        target = null;
        State = state.patrol;
    }

    IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnAfterTimer);
        ResetStats();
    }

    //private void OnDrawGizmos()
    //{
    //    float radius = checkRadiusSize;
    //    Vector2 center = this.transform.position;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(center, radius);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            speed = 0;
            StartCoroutine(RespawnAfterTime());
            player.FoodCount += 0.1f;
            //instantiate reward
        }
    }
}
