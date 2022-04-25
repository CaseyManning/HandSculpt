using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMatController : MonoBehaviour
{
    public Material baseHand;
    public Material grabHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGrab()
    {
        GetComponent<SkinnedMeshRenderer>().material = grabHand;
    }

    public void OnGrabEnd()
    {
        GetComponent<SkinnedMeshRenderer>().material = baseHand;
    }
}
