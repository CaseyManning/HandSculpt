using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject obj;

    Vector3 offset = new Vector3(0, 0.2f, 0);

    private void Start()
    {
        
    }

    void Update()
    {
        if(obj == null)
        {
            Destroy(gameObject);
        }
        transform.position = obj.transform.position + offset;

        transform.LookAt(transform.position - (Camera.main.transform.position - transform.position), transform.up);
        transform.rotation = Quaternion.Euler(new Vector3(15, transform.rotation.eulerAngles.y, 0));
    }

    public void delete()
    {
        StartCoroutine(delayeddelete());
    }

    IEnumerator delayeddelete()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(obj);
        Destroy(gameObject);
    }

    public void edit()
    {
        ObjectManager.EnterEditMode(obj);
        StartCoroutine(delayeddeletemenu());
    }

    IEnumerator delayeddeletemenu()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
