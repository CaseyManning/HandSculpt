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

    public Material fadedOut;
    public Material objMat;

    static float lastGrab;

    public enum Mode
    {
        Main, Edit
    }

    public static Mode mode = Mode.Main;

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
        if (GestureDetector.instance.left.prevGesture.name == "Fist" && GestureDetector.instance.right.prevGesture.name == "Fist")
        {
            EndEditMode();
        }
    }

    public static void OnGrab(CustomHand hand)
    {
        if (mode == Mode.Main)
        {

            OnPokeStop(hand);
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
        } else
        {
            if (Time.time - lastGrab < 0.2f)
            {
                EndEditMode();
            }
            lastGrab = Time.time;
        }
    }

    public static void OnGrabStop(CustomHand hand)
    {
        if (mode == Mode.Main)
        {
            hand.selected.GetComponent<ObjectScript>().stopFollow();
        }
    }

    public static void OnPoke(CustomHand hand)
    {
        if (mode == Mode.Main)
        {
            if (hand.currentMenu == null)
            {
                GameObject menu = Instantiate(instance.objMenu);
                menu.GetComponent<MenuScript>().obj = hand.selected;
                hand.currentMenu = menu;
            }
        }
    }
    public static void OnPokeStop(CustomHand hand)
    {
        if (mode == Mode.Main)
        {
            instance.StartCoroutine(instance.waitForMenuDestroy(hand));
            //if (hand.currentMenu != null)
            //{
            //    Destroy(hand.currentMenu);
            //    hand.currentMenu = null;
            //}
        }
    }

    IEnumerator waitForMenuDestroy(CustomHand hand)
    {
        while(hand.currentMenu != null)
        {
            if(Vector3.Distance(hand.getCenter(), hand.currentMenu.transform.position) > CustomHand.grabRange)
            {
                Destroy(hand.currentMenu);
                hand.currentMenu = null;
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public static void EnterEditMode(GameObject toEdit)
    {
        mode = Mode.Edit;
        SculptManager.editing = toEdit;

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Object"))
        {
            if (g != toEdit)
            {
                g.GetComponent<MeshRenderer>().material = instance.fadedOut;
            }
        }
    }

    public static void EndEditMode()
    {
        print("ENDING EDIT MODE");
        mode = Mode.Main;
        SculptManager.editing = null;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Object"))
        {
            
            g.GetComponent<MeshRenderer>().material = instance.objMat;
        }

        SculptManager.deactivateVisual();
    }
}