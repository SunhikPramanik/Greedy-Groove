using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{

    bool isChild = false;

    public GameObject childObj;
    public float changeSizeValue;
    public float foodCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !isChild)
        {
            print("isChild");
            childObj.transform.SetParent(this.transform);
            isChild = true;
        }
        if (Input.GetKeyDown(KeyCode.Q) && isChild)
        {
            print("is not Child");
            childObj.transform.SetParent(null);
            isChild = false;
        }
        if (foodCount >= 1)
        {
            var excessFood = foodCount - 1;
            var increaseSizeBy = changeSizeValue * excessFood;
            var exceededSize = new Vector3(0.25f + increaseSizeBy, 0.25f + increaseSizeBy, 0.25f + increaseSizeBy);
            this.transform.localScale = exceededSize;
        }
        if (isChild && Input.GetKeyDown(KeyCode.Space))
        {
            //Vector3 val = new Vector3(1 / changeSizeValue, 1 / changeSizeValue, 1 / changeSizeValue);
            //Vector3 val2 = new Vector3(changeSizeValue, changeSizeValue, changeSizeValue);
            //childObj.transform.localScale = Vector3.Scale(childObj.transform.localScale, val);
            //this.transform.localScale -= val2;

        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            print(childObj.transform.localScale);
            foodCount++;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            print(childObj.transform.localScale);
            foodCount--;
        }

    }
}
