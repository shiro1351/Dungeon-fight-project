using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected LayerMask enemyLayers;

    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;
    [SerializeField] protected LayerMask Ground;
    protected bool isGround = false;
    [SerializeField] protected float speed;
    protected Vector3 move = Vector3.zero;
    protected string currentAnim;
    [SerializeField] protected float jumpForce;
    protected bool isJumping = false;
    protected bool isAttack = false;
    public float saveSpeed;
    protected bool isDefend = false;
    protected int n_jump;
    private ThanhMau thanhMau;
    [SerializeField] protected float luongMauHienTai;
    private float luongMauToiDa;
    [SerializeField] protected float damage;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        move = Vector3.zero;
        saveSpeed = speed;
        anim = GetComponent<Animator>();
        n_jump = 0;
        thanhMau = FindAnyObjectByType<ThanhMau>();
        luongMauToiDa = luongMauHienTai;
        thanhMau.updateBlood(luongMauHienTai,luongMauToiDa);
    }

    protected virtual void Update()
    {
        isGround = checkGround();
        if (isGround) n_jump = 0;
        HandleInput();
        MoveCharacter();
    }

    protected void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && n_jump < 1)
        {
            n_jump++;
            Jump();
        }

        if (!isGround && rb.velocity.y < 0 && !isAttack)
        {
            ChangeAnim("Fall");
            isJumping = false;
        }
        Skill();
        Defend();
        if (Input.GetKeyDown(KeyCode.K) && isGround && !isAttack && !isDefend)
        {
            Attack();
        }
    }

    protected void MoveCharacter()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        transform.position += move * Time.deltaTime * speed;

        if (move.x != 0 && isGround && !isAttack && !isJumping && !isDefend)
        {
            Run();
        }

        if (isGround && !isJumping && !isAttack && !isDefend && move.x == 0)
        {
            ChangeAnim("Idle");
        }
    }

    protected bool checkGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, Ground);
        Debug.DrawRay(transform.position, Vector2.down * 2.0f, Color.red);
        return hit.collider != null;
    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    protected virtual void Attack()
    {
        isAttack = true;
        ChangeAnim("Attack");
        StartCoroutine(WaitAttack());
        AttackRange();
    }

    protected void AttackRange()
    {
        // Kiểm tra kẻ địch trong phạm vi
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            enemy.GetComponent<EnemyAI>()?.TakeDamage(damage);
        }
    }


    protected IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(0.45f);
        isAttack = false;
    }

    protected virtual void Jump()
    {
        isJumping = true;
        ChangeAnim("Jump");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    protected virtual void Run()
    {
        ChangeAnim("Run");
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    protected virtual void Defend()
    {
        if (Input.GetKeyDown(KeyCode.U) && isGround && !isAttack && !isDefend)
        {
            speed = 1.0f;
            ChangeAnim("Defend");
            isDefend = true;
        }
        else if (Input.GetKeyUp(KeyCode.U) && isGround && !isAttack && isDefend)
        {
            speed = saveSpeed;
            isDefend = false;
        }
    }

    protected virtual void Skill()
    {
        if(Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend)
        {
            ChangeAnim("Skill");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
    
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
}
