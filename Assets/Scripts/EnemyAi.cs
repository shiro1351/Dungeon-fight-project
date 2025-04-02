using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private string currentAnim;
    private Transform target;
    private bool isChasing, isAttacking, isGrounded, isTakingDamage;
    private float saveSpeed;
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxCollider;
    [SerializeField] private float Hp;
    [SerializeField] private float attackDamage; // Sát thương mỗi đòn đánh

    private Coroutine attackCoroutine;
    private Coroutine damageCoroutine;
    
    private Character player; // Tham chiếu đến máu của Player

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        saveSpeed = speed;
    }

    void Update()
    {
        isGrounded = CheckGround();
        if (Hp <= 0f)
        {
            Die();
            return;
        }

        if (!isTakingDamage && isGrounded && !isChasing && !isAttacking)
        {
            ChangeAnim("Idle");
        }

        if (!isTakingDamage && isGrounded && isChasing)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                isAttacking = false;
                speed = saveSpeed;
                MoveToPlayer();
            }
        }
    }

    protected virtual bool CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }

    protected virtual void MoveToPlayer()
    {
        if (target != null && !isAttacking && !isTakingDamage)
        {
            Vector3 direction = target.position - transform.position;
            FlipSprite(direction.x);
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            ChangeAnim("Run");
        }
    }

    protected virtual void Attack()
    {
        if (Hp > 0 && target != null && !isTakingDamage)
        {
            isAttacking = true;
            ChangeAnim("Attack");
            StartCoroutine(AttackSequence());
        }
    }

    protected IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(0.5f); // Thời gian ra đòn

        if (Vector3.Distance(transform.position, target.position) <= attackRange && isAttacking)
        {
            player?.TakeDamage(attackDamage);
        }
        isAttacking = false; 
        ChangeAnim("Idle"); // Sau khi đánh xong, quái trở lại trạng thái Idle
        yield return new WaitForSeconds(1.5f); // Đợi 1.5s trước khi tấn công tiếp
    }

    private void FlipSprite(float directionX)
    {
        if (directionX != 0)
        {
            bool facingRight = transform.localScale.x > 0;
            if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            target = collision.transform;
            
            // Lấy tham chiếu đến PlayerHealth
            player = collision.GetComponent<Character>();

            if (attackCoroutine == null) 
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            isAttacking = false;
            speed = saveSpeed;
            target = null;
            player = null; // Reset tham chiếu

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (!isTakingDamage && target != null && Vector3.Distance(transform.position, target.position) <= attackRange && Hp > 0 && !isAttacking)
            {
                Attack();
            }
            yield return new WaitForSeconds(1.5f); // Chờ 1.5s trước khi kiểm tra tấn công tiếp
        }
    }

    protected virtual void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        
        if (Hp <= 0)
        {
            Die();
            return;
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine); // Reset lại nếu bị đánh liên tiếp
        }
        damageCoroutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        isTakingDamage = true;
        ChangeAnim("Damage");
        yield return new WaitForSeconds(1.0f);
        isTakingDamage = false;
    }

    private void Die()
    {
        ChangeAnim("Death");
        speed = 0;
        Destroy(gameObject, 1.5f);
    }
}
