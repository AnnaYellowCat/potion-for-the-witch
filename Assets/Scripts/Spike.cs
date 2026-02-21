using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float damageInterval = 0.4f;
    private bool isHeroOnSpike = false;
    private Coroutine damageCoroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            isHeroOnSpike = true;

            Hero.Instance.GetDamage();
            Hero.Instance.isDamage = true;

            if (damageCoroutine == null)
                damageCoroutine = StartCoroutine(PeriodicDamage());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
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