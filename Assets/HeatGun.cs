using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGun : MonoBehaviour
{
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        print(transform.position);

        if (Physics.Raycast(transform.position, transform.right, out hit, 100, layerMask))
        {
            lr.SetPosition(1, hit.point);
        } else
        {
            lr.SetPosition(1, transform.position);
        }
        
    }
}
