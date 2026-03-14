using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unboxing : MonoBehaviour
{
    private Animator anim;
    public GameObject activeGameObject;

    private bool isPlayerInRange = false;
    private GameObject hintPanel;

    public bool IsOpened { get; private set; } = false;

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
            if (!IsOpened)
            {
                Interact();
            }
            else
            {
                TakeItemFromChest();
            }
        }
    }

    private void Interact()
    {
        if (!IsOpened)
        {
            IsOpened = true;
            BoxState = States.unboxing;
            Invoke("ActivateObject", 0.5f);

            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }
    }

    public void ActivateObject()
    {
        if (activeGameObject != null)
        {
            activeGameObject.SetActive(true);

            ItemManager itemManager = FindFirstObjectByType<ItemManager>();
            if (itemManager != null)
            {
                itemManager.ApplyItemsState();
            }
        }
    }

    private void TakeItemFromChest()
    {
        Transform itemTransform = null;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("ChestItem"))
            {
                itemTransform = child;
                break;
            }
        }

        if (itemTransform != null)
        {
            GameObject item = itemTransform.gameObject;

            Objects objectsScript = FindFirstObjectByType<Objects>();
            if (objectsScript != null)
            {
                objectsScript.TakeChestItem(item);
            }
            else
            {
                Debug.LogError("Objects script not found!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (hintPanel != null && !IsOpened)
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

    public void SetOpened()
    {
        IsOpened = true;
        BoxState = States.unboxing;
        if (activeGameObject != null)
        {
            activeGameObject.SetActive(true);
        }
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }

        ItemManager itemManager = FindFirstObjectByType<ItemManager>();
        if (itemManager != null)
        {
            itemManager.ApplyItemsState();
        }
    }
}