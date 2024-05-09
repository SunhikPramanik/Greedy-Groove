using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFoodOnClick : MonoBehaviour
{
    [SerializeField]
    private GameObject targetFood;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(targetFood, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,0), Quaternion.identity);
        }
    }
}
