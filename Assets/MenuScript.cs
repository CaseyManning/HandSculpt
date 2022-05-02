using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject obj;

    Vector3 offset = new Vector3(0, 0.2f, 0.05f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = obj.transform.position + offset;
    }
}
