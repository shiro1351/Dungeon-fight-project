using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;

    private Transform target;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isGrounded = false;
    private Coroutine attackCoroutine;

    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxCollider;

    [SerializeField] private LayerMask Ground;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        SetIdleState();
    }

    void Update()
    {
        isGrounded = CheckGround();

        if (isChasing && target != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            if (distanceToPlayer > attackRange && !isAttacking)
            {
                MoveTowardsPlayer();
            }
            else if (!isAttacking)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
        else if (!isChasing)
        {
            SetIdleState();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        FlipSprite(direction.x);

        if (!isAttacking)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
        }
    }

    private bool CheckGround()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.BoxCast(
            capsuleCollider.bounds.center,
            new Vector2(capsuleCollider.bounds.size.x * 0.9f, capsuleCollider.bounds.size.y),
            0f, Vector2.down, extraHeight, Ground);

        return hit.collider != null;
    }

    private void FlipSprite(float directionX)
{
    if (directionX != 0)
    {
        bool facingRight = transform.localScale.x > 0; // Kiểm tra hướng hiện tại
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}



    private IEnumerator Attack()
    {
        isAttacking = true;

        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");

        rb.velocity = Vector2.zero;
        anim.SetBool("Run", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Attack", true);

        float attackTime = anim.GetCurrentAnimatorStateInfo(0).length;
        float timer = 0f;

        while (timer < attackTime)
        {
            if (!isChasing || target == null)
            {
                Debug.Log("Dừng attack vì Player đã rời khỏi vùng!");
                anim.SetBool("Attack", false);
                isAttacking = false;
                SetIdleState();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;

        if (isChasing && target != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);
            if (distanceToPlayer <= attackRange)
            {
                attackCoroutine = StartCoroutine(Attack());
                yield break;
            }
        }

        SetIdleState();
    }

    private void SetIdleState()
    {
        if (isAttacking) return;

        rb.velocity = Vector2.zero;
        anim.SetBool("Run", false);
        anim.SetBool("Idle", true);
        anim.SetBool("Attack", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player vào vùng phát hiện!");
            target = other.transform;
            isChasing = true;
        }
        
        if (other == capsuleCollider && isChasing)
        {
            Debug.Log("Player chạm vào enemy -> Bắt đầu Attack!");
            if (!isAttacking)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player rời vùng phát hiện!");
            target = null;
            isChasing = false;

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            
            anim.SetBool("Attack", false);
            isAttacking = false;
            
            SetIdleState();
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
    }
}
