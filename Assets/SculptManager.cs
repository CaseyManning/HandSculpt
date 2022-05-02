using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SculptManager : MonoBehaviour
{
    public static GameObject editing;

    void Start()
    {
        
    }

    void Update()
    {
        CustomHand[] hands = new CustomHand[2] { ObjectManager.leftHand.GetComponent<CustomHand>(), ObjectManager.rightHand.GetComponent<CustomHand>() };

        foreach(CustomHand hand in hands)
        {
            if(hand.hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {

            }
        }
    }
}
