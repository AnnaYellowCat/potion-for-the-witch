using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float damageInterval = 1f;
    private bool isHeroOnSpike = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Hero.Instance.gameObject)
        {
            isHeroOnSpike = true;

            if (Hero.Instance != null)
            {
                Hero.Instance.GetDamage();
                Hero.Instance.isDamage = true;
            }

            if (damageCoroutine == null)
                damageCoroutine = StartCoroutine(PeriodicDamage());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Hero.Instance.gameObject)
        {
            isHeroOnSpike = false;

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator PeriodicDamage()
    {
        while (isHeroOnSpike)
        {
            yield return new WaitForSeconds(damageInterval);

            if (isHeroOnSpike && Hero.Instance != null)
            {
                Hero.Instance.GetDamage();
                Hero.Instance.isDamage = true;
            }
        }
        damageCoroutine = null;
    }
}