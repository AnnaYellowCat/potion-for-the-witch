using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource deadSound;

    private Animator anim;

    private Vector3 savedPosition;
    private Vector3 savedDirection;
    private bool savedFlipX;

    public static Centipede Instance {get; set; }

    private float speed = 1.5f;
    private Vector3 dir;
    private SpriteRenderer sprite;

    [SerializeField] private int lives = 3;
    public int currentLives;

    public bool isAttacking = false;
    public bool isDamage = false;
    public bool isDeath = false;
    public bool isMusDead = true;

    private StatesEnemy State
    {
        get { return (StatesEnemy)anim.GetInteger("stateEnemy"); }
        set { anim.SetInteger("stateEnemy", (int)value); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentLives = lives;
    }

    void Start()
    {
        dir = transform.right;
    }

    void Update()
    {
        if (!isDeath)
        {
            Move();
        }

        if (!isAttacking && !isDamage && !isDeath) State = StatesEnemy.idle;

        if (isDamage == true)
        {
            DamageAnimation();
        }

        if (isDeath == true)
        {
            if (isMusDead)
            {
                if (deadSound != null) deadSound.Play();
                isMusDead = false;
            }
            DeathAnimation();
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.right * dir.x * 0.9f, 0.1f);

        bool isTouchingWallNow = false;
        foreach (var col in colliders)
        {
            if (col.gameObject != this.gameObject && col.gameObject != Hero.Instance.gameObject)
            {
                isTouchingWallNow = true;
                break;
            }
        }

        if (isTouchingWallNow)
        {
            dir *= -1f;
            sprite.flipX = dir.x > 0.0f;
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * speed);
        sprite.flipX = dir.x > 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
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
            if (attackSound != null) attackSound.Play();
            StartCoroutine(AttackAnimation());
        }
        else
        {
            dir *= -1f;
            sprite.flipX = dir.x > 0.0f;
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
        gameObject.SetActive(false);
    }

    public void GetDamage()
    {
        currentLives--;
        lives = currentLives;
        isDamage = true;

        if (currentLives < 1)
        {
            isDeath = true;
        }
        if (!isDeath && damageSound != null){
            damageSound.Play();
        }
    }

    public void SaveState()
    {
        savedPosition = transform.position;
        savedDirection = dir;
        savedFlipX = sprite.flipX;
    }

    public void SetLives(int newLives)
    {
        currentLives = newLives;
        lives = newLives;

        if (currentLives <= 0)
        {
            isDeath = true;
//            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public Vector3 GetDirection()
    {
        return dir;
    }

    public bool GetFlipX()
    {
        return sprite.flipX;
    }

    public void RestoreMovement(Vector3 direction, bool flipX)
    {
        dir = direction;
        sprite.flipX = flipX;
    }
}

public enum StatesEnemy
{
    idle,
    attack,
    takeDamage,
    death
}