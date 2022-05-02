using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour
{
    Transform following = null;


    Vector3 offset = Vector3.zero;

    float startScale;
    float startDist;
    bool isScaling = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (following != null)
        {
            //transform.position = following.transform.position + offset;
            transform.position = following.transform.TransformPoint(offset);
            transform.rotation = following.transform.rotation;
        }

        Vector3 center = ObjectManager.leftHand.transform.position + ObjectManager.rightHand.transform.position / 2;
        if (!isScaling && CustomHand.leftPinching && CustomHand.rightPinching && Vector3.Distance(center, transform.position) < 0.4f)
        {
            isScaling = true;
            startScale = transform.localScale.x;
            startDist = Vector3.Distance(ObjectManager.leftHand.transform.position, ObjectManager.rightHand.transform.position);
        }
        if (isScaling && (!CustomHand.leftPinching || !CustomHand.rightPinching))
        {
            isScaling = false;
        }

        if (isScaling)
        {
            float dist = Vector3.Distance(ObjectManager.leftHand.transform.position, ObjectManager.rightHand.transform.position);
            transform.localScale = new Vector3(1, 1, 1) * startScale * (dist / startDist);
        }

        
    }

    public void setFollow(CustomHand hand)
    {
        
        if (following != hand)
        {
            offset = hand.transform.InverseTransformPoint(transform.position);
            //offset = transform.position - hand.transform.position;
        }
        following = hand.transform;
    }

    public void stopFollow()
    {
        following = null;
    }
}
