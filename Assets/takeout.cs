using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takeout : MonoBehaviour
{
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);
        //transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void takeOut()
    {
        transform.SetParent(null);
    }
}
