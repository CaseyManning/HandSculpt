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

    public static Vector3 getCenter(CustomHand hand)
    {
        if (hand.left)
        {
            return hand.transform.position + 0.1f * hand.transform.right;
        }
        else
        {
            return hand.transform.position - 0.1f * hand.transform.right;
        }
    }

    public void OnGrab(CustomHand hand)
    {
        print("GRABBING");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        GameObject selected = null;
        float nearestDist = Mathf.Infinity;

        Vector3 handCenter = getCenter(hand);

        foreach(GameObject g in objs)
        {
            float dist = Vector3.Distance(g.transform.position, handCenter);
            if (dist < grabRange && dist < nearestDist)
            {
                selected = g;
                nearestDist = dist;
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
