using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject obj;

    float grabRange = 0.3f;

    public static GameObject leftHand;
    public static GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        leftHand = GameObject.FindGameObjectWithTag("LeftHand");
        rightHand = GameObject.FindGameObjectWithTag("RightHand");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGrab(CustomHand hand)
    {
        print("GRABBING");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        GameObject selected = null;

        foreach(GameObject g in objs)
        {
            if(Vector3.Distance(g.transform.position, hand.transform.position) < grabRange)
            {
                selected = g;
            }
        }
        if(selected == null)
        {
            GameObject newObj = Instantiate(obj);
            if (hand.left)
            {
                newObj.transform.position = hand.transform.position + 0.2f * hand.transform.right;
            }
            else
            {
                newObj.transform.position = hand.transform.position - 0.2f * hand.transform.right;
            }
            selected = newObj;
        }

        selected.GetComponent<TestBall>().setFollow(hand);
    }

    public void OnGrabStop()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject g in objs)
        {
            g.GetComponent<TestBall>().stopFollow();
        }
    }
}
