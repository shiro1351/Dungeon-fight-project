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
        boxCollider.isTrigger = true; // Đảm bảo collider phát hiện va chạm

        SetIdleState();
    }

    void Update()
    {
        isGrounded = CheckGround();

        if (isChasing && target != null && !isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                StartCoroutine(Attack());
            }
        }
        else if (!isChasing && !isAttacking)
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

        anim.SetBool("Run", true);
        anim.SetBool("Idle", false);
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
            Vector3 scale = transform.localScale;
            scale.x = (directionX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        anim.SetBool("Run", false);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        SetIdleState();
    }

    private void SetIdleState()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("Run", false);
        anim.SetBool("Idle", true);
        anim.Play("Idle");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player vào vùng phát hiện!");
            target = other.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player rời vùng phát hiện!");
            target = null;
            isChasing = false;
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
