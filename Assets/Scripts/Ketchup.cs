using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ketchup : MonoBehaviour
{
    bool grabbing = false;

    public GameObject ketchupball;

    float spawnPeriod = 0.05f;
    float lastSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbing && Time.time - lastSpawn > spawnPeriod)
        {
            ShootKetchup();
            lastSpawn = Time.time;
        }
    }

    void ShootKetchup()
    {
        //GameObject ball = Instantiate(ketchupball);

        //ball.transform.position = transform.position;
        //ball.transform.rotation = transform.rotation;

        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(transform.position, -transform.up,  out hit, 100, layerMask))
        {
            if (hit.transform.gameObject.tag != "ball")
            {
                print("instantiating");
                GameObject ball = Instantiate(ketchupball);

                ball.transform.position = hit.point;
                ball.transform.parent = hit.transform;
                //ball.transform.rotation = transform.rotation;
            }
        }
    }

    public void grabStart()
    {
        grabbing = true;
    }
    public void grabEnd()
    {
        grabbing = false;
    }
}
