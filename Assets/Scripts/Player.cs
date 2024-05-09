using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public float p_foodCount = 1;
    float baseScale;
    Rigidbody2D rb;
    Vector2 _movementInput;
    Vector2 _smoothedMovementInput;
    Vector2 _movementInputSmoothVelocity;
    Camera p_camera;
    float baseOrthoSize;
    Animator animator;


    [SerializeField] private float p_speed;
    [SerializeField] private float p_rotateSpeed;
    [SerializeField] private float p_increaseSizeAfterFoodCount;
    [SerializeField] private float p_increaseSizeBy;
    [SerializeField] private float p_degradationRate;
    [SerializeField] private float p_foodNeededForNextLevelTransition;
    [SerializeField] private Vector3 p_eatingPosition;
    [SerializeField] public Hunger p_hungerNPC;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image foodBar2;
    [SerializeField] private Color hungerColor;
    [SerializeField] private Color NormalStateColor;
    [SerializeField] private Color levelUnlockedColor;
    [SerializeField] private GameObject p_shadow;

    public float FoodCount { get => p_foodCount; set => p_foodCount = value; }
    public Vector3 P_eatingPosition { get => p_eatingPosition; set => p_eatingPosition = value; }
    public Hunger P_hungerNPC { get => p_hungerNPC; set => p_hungerNPC = value; }

    private void Awake()
    {
        p_camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movementInputSmoothVelocity,
                    0.1f);

        rb.velocity = _smoothedMovementInput * p_speed;
        animator.SetBool("isMoving", true);
    }


    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, p_rotateSpeed * Time.deltaTime);

            rb.MoveRotation(rotation);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }



    // Start is called before the first frame update
    void Start()
    {
        hungerColor.a = 1;
        NormalStateColor.a = 1;
        levelUnlockedColor.a = 1;
        baseOrthoSize = p_camera.orthographicSize;
        baseScale = this.transform.localScale.x;
        foodBar.fillAmount = p_foodCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movementInput == Vector2.zero)
        {
            animator.SetBool("isMoving", false);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }
        if (p_foodCount >= 1)
        {
            var excessFood = p_foodCount - 1;
            var increaseSizeByVal = p_increaseSizeBy * excessFood;
            var exceededSize = new Vector3(baseScale + increaseSizeByVal, baseScale + increaseSizeByVal, baseScale + increaseSizeByVal);
            this.transform.localScale = exceededSize;
            var camSize = baseOrthoSize + (excessFood * 0.25f);
            p_camera.orthographicSize = camSize;
            foodBar2.color = NormalStateColor;
            foodBar2.fillAmount = excessFood/p_foodNeededForNextLevelTransition;
            foodBar.fillAmount = 1;
        }

        if (p_foodCount > 0)
        {
            p_foodCount -= p_degradationRate;
        }

        if(p_foodCount >= p_foodNeededForNextLevelTransition)
        {
            foodBar2.color = levelUnlockedColor;
            foodBar.fillAmount = 1;
        }

        if(p_foodCount < 1)
        {
            foodBar.color = hungerColor;
            foodBar.fillAmount = p_foodCount;
        }

        if(p_foodCount <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //repeatedly smash buttons to make the enemy leave
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.tag == "Food")
        {
            p_hungerNPC.Target = this.gameObject;
            p_eatingPosition = collision.transform.position;
            Destroy(collision.gameObject);
            Instantiate(p_shadow, this.transform.position, Quaternion.identity);
            FoodCount++;
            if (FoodCount > p_increaseSizeAfterFoodCount)
            {
                var changeToVector = new Vector3(p_increaseSizeBy, p_increaseSizeBy, p_increaseSizeBy);
                this.transform.localScale += changeToVector;
                this.GetComponent<SpawnFoodAroundPlayer>().SpawnRadius += p_increaseSizeBy;
                this.GetComponent<SpawnFoodAroundPlayer>().LowVal += p_increaseSizeBy;
            }
        }

        if(collision.tag == "Bullet")
        {
            p_foodCount -= 1;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Hunger")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
