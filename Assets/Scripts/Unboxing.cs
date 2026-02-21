using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator anim;
    public GameObject activeGameObject;

    private bool isPlayerInRange = false;
    private GameObject hintPanel;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Transform hintTransform = transform.Find("Hint");
        if (hintTransform != null)
        {
            hintPanel = hintTransform.gameObject;
            hintPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        BoxState = States.unboxing;
        Invoke("ActivateObject", 0.5f);
    }

    public void ActivateObject()
    {
        activeGameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
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