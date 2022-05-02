using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject obj;

    static ObjectManager instance;


    public static GameObject leftHand;
    public static GameObject rightHand;

    public GameObject lHand;
    public GameObject rHand;

    public GameObject objMenu;

    // Start is called before the first frame update
    void Start()
    {
        leftHand = lHand;
        rightHand = rHand;

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void OnGrab(CustomHand hand)
    {
        print("GRABBING WITH HAND. LEFT: " + hand.left);

        if (hand.selected == null)
        { 
            GameObject newObj = Instantiate(instance.obj);
            if (hand.left)
            {
                newObj.transform.position = hand.transform.position + 0.2f * hand.transform.right;
            }
            else
            {
                newObj.transform.position = hand.transform.position - 0.2f * hand.transform.right;
            }
            hand.selected = newObj;
        }

        hand.selected.GetComponent<ObjectScript>().setFollow(hand);
    }

    public static void OnGrabStop(CustomHand hand)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject g in objs)
        {
            g.GetComponent<ObjectScript>().stopFollow();
        }
    }

    public static void OnPoke(CustomHand hand)
    {
        print("POKE START");
        if (hand.currentMenu == null)
        {
            GameObject menu = Instantiate(instance.objMenu);
            menu.GetComponent<MenuScript>().obj = hand.selected;
            hand.currentMenu = menu;
        }
    }
    public static void OnPokeStop(CustomHand hand)
    {
        print("POKE END");
        Destroy(hand.currentMenu);
        hand.currentMenu = null;
    }
}