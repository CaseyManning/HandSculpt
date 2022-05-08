using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KetchupBall : MonoBehaviour
{
    bool moving = false;

    float speed = 1f;

    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.Translate(-Vector3.up * Time.deltaTime * speed);
        }
        if(Time.time - startTime > 10f && moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("bottle") && Time.time - startTime > 0.1f)
        {
            moving = false;
        }
        
    }
}
