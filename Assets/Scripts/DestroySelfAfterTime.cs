using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfAfterTime : MonoBehaviour
{
    [SerializeField] private float destroyAfterTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyAfterTime);
        Destroy(this.gameObject);
    }
}
