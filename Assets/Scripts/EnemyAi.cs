using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private string currentAnim;
    private Transform target;
    private bool isChasing, isAttacking, isGrounded;
    private float saveSpeed;
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxCollider;
    [SerializeField] private float Hp;

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
        if (isGrounded && !isChasing && !isAttacking)
        {
            ChangeAnim("Idle");
        }
        
        if (isGrounded && isChasing)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                isAttacking = false;
                speed = saveSpeed;
                MoveToPlayer();
            }
            else
            {
                isAttacking = true;
                speed = 0;
                Attack();
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
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            FlipSprite(direction.x);
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            ChangeAnim("Run");
        }
    }

    protected virtual void Attack()
    {
        ChangeAnim("Attack");
        StartCoroutine(WaitAttack());
    }

    protected IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
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
        }
    }

    private void Die()
    {
        ChangeAnim("Die");
        Destroy(gameObject, 1f); // Xóa kẻ địch sau 1 giây
    }

}
