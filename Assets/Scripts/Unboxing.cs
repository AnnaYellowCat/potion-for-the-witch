using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator anim;
    public GameObject activeGameObject;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ActivateObject()
    {
        activeGameObject.SetActive(true);
    }


    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)){
            BoxState = States.unboxing;
            Invoke("ActivateObject", 0.5f);
            //ActivateObject();
            //GameObject Item = Instantiate(item, transform.position, Quaternion.identity);
            //temp.SetActive(true);
        }
    }

    private States BoxState
    {
        get { return (States)anim.GetInteger("BoxState"); }
        set { anim.SetInteger("BoxState", (int)value); }
    }

    public enum States
    {
        boxing,
        unboxing
    }
}
