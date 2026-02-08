using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource deadSound;

    private Animator anim;

    public static Centipede Instance {get; set; }

    private float speed = 1.5f;
    private Vector3 dir;
    private SpriteRenderer sprite;
    private int lives = 3;

    public bool isAttacking = false;
    public bool isDamage = false;
    public bool isDeath = false;
    public bool isMusDead = true; //чтобы мелодия смерти не проигрывалась несколько раз

    private StatesEnemy State
    {
        get { return (StatesEnemy)anim.GetInteger("stateEnemy"); }
        set { anim.SetInteger("stateEnemy", (int)value); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        dir = transform.right;
    }

    void Update()
    {
        Move();
        if (!isAttacking && !isDamage && !isDeath) State = StatesEnemy.idle;

        if (isDamage == true)
        {
            DamageAnimation();
        }

        if (isDeath == true)
        {
            if (isMusDead)
            {
                deadSound.Play(); //звуки смерти
                isMusDead = false;
            }
            DeathAnimation();
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.1f);
        if (colliders.Length > 1) dir *= -1f;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime*speed);
        sprite.flipX = dir.x > 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime*speed);
            if (GameObject.Find("Hero").transform.position.x < this.transform.position.x && dir.x > 0.0f){
                dir *= -1f;
                sprite.flipX = false;
            }

            if (GameObject.Find("Hero").transform.position.x > this.transform.position.x && dir.x < 0.0f){
                dir *= -1f;
                sprite.flipX = false;
            }
            if(GameObject.Find("Hero").transform.position.y > this.transform.position.y){
                Hero.Instance.GetDamage();
            }
            else{
                Hero.Instance.GetHalfDamage();
            }
            


            isAttacking = true;
            State = StatesEnemy.attack;

            attackSound.Play(); //звук атаки

            StartCoroutine(AttackAnimation());
        }
    }

    private IEnumerator AttackAnimation(){
        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    private void DamageAnimation()
    {
        State = StatesEnemy.takeDamage;

        StartCoroutine(TakeDamageAnimation());

    }

    private IEnumerator TakeDamageAnimation(){
        yield return new WaitForSeconds(0.5f);
        isDamage = false;
    }

    private void DeathAnimation()
    {
        State = StatesEnemy.death;

        StartCoroutine(TakeDeathAnimation());

    }

    private IEnumerator TakeDeathAnimation(){
        yield return new WaitForSeconds(0.8f);
        Destroy(this.gameObject);
    }
    
    public void GetDamage()
    {
        lives--;
        isDamage = true;
        if (lives < 1)
        {
            isDeath = true;
        }
        if (!isDeath){
            damageSound.Play(); //дали по бошке
        }
    }
}

public enum StatesEnemy
{
    idle,
    attack,
    takeDamage,
    death
}