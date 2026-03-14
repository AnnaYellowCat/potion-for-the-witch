using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource deadSound;

    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public int amount = 0;
    public int amountRight = 0;

    public static Hero Instance {get; set; }
    
    private double lives = 5;
    private double helth;
    public float speed = 500f;
    public float jumpForce = 15f;
    private bool isGrounded = false;

    public Image[] hearts;

    public Sprite aliveHeart;
    public Sprite deadHeart;

    public bool isAttacking = false;
    public bool isRecharged = true;
    public bool isDamage = false;
    public bool isDeath = false;
    public bool isMusDead = true; //чтобы мелодия смерти не проигрывалась несколько раз
    public bool isOver = false;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public float CheckGroundOffsetY  = -1.8f;
    public float CheckGroundRadius  = 0.3f;

    public GameObject obj;
    private GameManager Manag;

    private void Start()
    {
        Manag = obj.GetComponent<GameManager>();
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        helth = lives;

        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded && !isAttacking && !isDamage && !isDeath) State = States.idle;

        if (!isAttacking && Input.GetButton("Horizontal"))
        {
            Run();
        }
        if (!isAttacking && isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.F))
        {
            Attack();
        }
        if (isDamage == true)
        {
            DamageAnimationHero();
        }
        if (isDeath == true)
        {
            if (isMusDead)
            {
                deadSound.Play(); //звуки смерти
                isMusDead = false;
            }
            DeathAnimationHero();
        }

        if (helth>lives && lives%1 == 0){
            helth = lives;
        }
        for (int i=0; i<hearts.Length; i++){
            if(i<helth)
            {
                hearts[i].sprite = aliveHeart;
            }
            else{
                hearts[i].sprite = deadHeart;
            }
        }

        if(isOver == true){
            Debug.Log("Правильных предметов: " + amountRight);
            if(amountRight == 5){
                Manag.LoadScene("WinFull");
                // КОТИК ПРИНЕС ВСЕ НУЖНЫЕ ПРЕДМЕТЫ, ВСЕ ПРАВИЛЬНО, ПОБЕДА--------------------------------------------------------------------------------------------------------
            }
            else{
                Manag.LoadScene("Defeat");
                // КОТИК ПРИНЕС ЧТО-ТО НЕ ТО, ПРОИГРЫШ--------------------------------------------------------------------------------------------------------------------
            }
        }
    }

    private void Run()
    {
        if(isGrounded && !isDamage && !isDeath) State = States.run;

        Vector3 dir  = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        jumpSound.Play(); //звук прыжка
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        Collider2D[] colliders  = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + CheckGroundOffsetY), CheckGroundRadius);
        if (colliders.Length > 1){
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }
        if(!isGrounded){
            State = States.jump;
        }
    }
    

    private void Attack()
    {
        attackSound.Play(); //звук атаки

        State = States.attack;
        isAttacking = true;
        isRecharged = false;

        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);
        for (int i = 0; i < colliders.Length; i++)
        {
            Centipede centipede = colliders[i].GetComponent<Centipede>();
            if (centipede != null)
            {
                centipede.GetDamage();
            }
        }
    }

    private IEnumerator AttackAnimation(){
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown(){
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }

    private void DamageAnimationHero()
    {
        State = States.takeDamage;

        StartCoroutine(TakeDamageAnimation());
    }

    private IEnumerator TakeDamageAnimation(){
        yield return new WaitForSeconds(0.5f);
        isDamage = false;
    }

    private void DeathAnimationHero()
    {
        State = States.death;

        StartCoroutine(TakeDeathAnimation());
    }

    private IEnumerator TakeDeathAnimation(){
        yield return new WaitForSeconds(1.1f);
        Destroy(this.gameObject);
        Manag.LoadScene("Death");
        // КОТИК УМЕР И НЕ ДОШЕЛ ДО КОТЛА, ПРОИГРЫШ-------------------------------------------------------------------------------------------------------------------
    }

    public void GetDamage()
    {
        isDamage = true;
        lives -= 1;
        if (lives < 1)
        {
            foreach (var h in hearts){
                h.sprite = deadHeart;
            }
            isDeath = true;
        }
        if (!isDeath){
            damageSound.Play(); //дали по бошке
        }
    }
    
    public void GetHalfDamage()
    {
        isDamage = true;
        lives = lives - 0.5;
        if (lives < 1)
        {
            foreach (var h in hearts){
                h.sprite = deadHeart;
            }
            isDeath = true;
        }
        if (!isDeath){
            damageSound.Play(); //дали по бошке
        }
    }
    public double GetCurrentHealth()
    {
        return lives;
    }

    public void SetHealth(double health)
    {
        lives = health;
        helth = lives;
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < helth) ? aliveHeart : deadHeart;
        }
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

}

public enum States
{
    idle,
    run,
    jump,
    attack,
    takeDamage,
    death
}
