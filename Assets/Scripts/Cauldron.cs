using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject hintPanel;

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
            Hero.Instance.isOver = true;
        }
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
}