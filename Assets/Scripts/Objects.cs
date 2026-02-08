using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : MonoBehaviour
{
    [SerializeField] private AudioSource miskSound;

    private bool isInTrigger = false;
    private string tagObject;
    private bool isRight = false;
    //private GameObject obj;
    //public RandomObject spr;



    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if(Hero.Instance.amount < 3){
                if(isRight == true){
                    if(Hero.Instance.amount == 0){
                        Rand16.Instance.number = Rand13.Instance.rand;
                    }
                    if(Hero.Instance.amount == 1){
                        Rand17.Instance.number = Rand13.Instance.rand;
                    }
                    if(Hero.Instance.amount == 2){
                        Rand18.Instance.number = Rand13.Instance.rand;
                    }
                    Hero.Instance.amountRight = Hero.Instance.amountRight + 1;
                    isRight = false;
                }
                else{
                    if(Hero.Instance.amount == 0){
                        Rand16.Instance.number = Rand11.Instance.rand;
                    }
                    if(Hero.Instance.amount == 1){
                        Rand17.Instance.number = Rand11.Instance.rand;
                    }
                    if(Hero.Instance.amount == 2){
                        Rand18.Instance.number = Rand11.Instance.rand;
                    }
                }
                Hero.Instance.amount = Hero.Instance.amount + 1;
                miskSound.Play();
                Destroy(GameObject.FindGameObjectWithTag(tagObject));
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "object1")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand2.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand2.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand2.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
            
        }

        if (coll.gameObject.tag == "object2")
        {
           if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand1.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand1.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand1.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object3")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand3.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand3.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand3.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object5")            //������
        {
            isInTrigger = true;
            tagObject = "object5";
        }

        if (coll.gameObject.tag == "object6")             //������
        {
            isRight = true;
            isInTrigger = true;
            tagObject = "object6";
        }

        if (coll.gameObject.tag == "object7")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand4.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand4.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand4.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object8")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand5.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand5.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand5.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object9")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand15.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand15.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand15.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
                Hero.Instance.amountRight = Hero.Instance.amountRight + 1;
            }
        }

        if (coll.gameObject.tag == "object10")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand14.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand14.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand14.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
                Hero.Instance.amountRight = Hero.Instance.amountRight + 1;
            }
        }

        if (coll.gameObject.tag == "object11")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand8.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand8.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand8.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object12")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand9.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand9.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand9.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }

        if (coll.gameObject.tag == "object13")
        {
            if(Hero.Instance.amount < 3){
                if(Hero.Instance.amount == 0){
                    Rand16.Instance.number = Rand10.Instance.rand;
                }
                if(Hero.Instance.amount == 1){
                    Rand17.Instance.number = Rand10.Instance.rand;
                }
                if(Hero.Instance.amount == 2){
                    Rand18.Instance.number = Rand10.Instance.rand;
                }
                miskSound.Play();
                Destroy(coll.gameObject);
                Hero.Instance.amount = Hero.Instance.amount + 1;
            }
        }
    }
}
