using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : MonoBehaviour
{
    [SerializeField] private AudioSource miskSound;
    private ItemManager itemManager;

    private void Start()
    {
        itemManager = FindFirstObjectByType<ItemManager>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Item")
        {
            CollectItem(coll.gameObject);
        }
    }

    public void TakeChestItem(GameObject chestItem)
    {
        CollectItem(chestItem);
    }

    private void CollectItem(GameObject item)
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<ItemManager>();
        }

        bool added = itemManager.AddItemToInventory(item);

        if (added)
        {
            miskSound.Play();

            Hero.Instance.amount++;

            if (itemManager.IsItemTarget(item))
            {
                Hero.Instance.amountRight++;
            }

            Destroy(item);
        }
    }
}