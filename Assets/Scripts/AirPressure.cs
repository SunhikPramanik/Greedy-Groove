using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AirPressure : MonoBehaviour
{
    public float airFlowTime;
    public float airSpeed;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSomeTime());
    }

    //private void Update()
    //{
    //    this.transform.Translate(this.transform.up * airSpeed * Time.deltaTime);
    //}
    IEnumerator DestroyAfterSomeTime()
    {
        yield return new WaitForSeconds(airFlowTime);
        Destroy(this.gameObject);
    }
}
