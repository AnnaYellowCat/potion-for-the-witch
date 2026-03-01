using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : MonoBehaviour
{
    [SerializeField] private AudioSource miskSound;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Item")
        {
            string itemName = coll.gameObject.name;

            miskSound.Play();

            Hero.Instance.amount = Hero.Instance.amount + 1;

            Destroy(coll.gameObject);
        }
    }

    public void TakeChestItem(GameObject chestItem)
    {
        if (Hero.Instance.amount < 5)
        {
            string itemName = chestItem.name;

            Hero.Instance.amount++;

            if (chestItem.CompareTag("RightItem"))
            {
                Hero.Instance.amountRight++;
            }

            if (miskSound != null)
                miskSound.Play();

            Destroy(chestItem);
        }
    }
}