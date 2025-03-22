using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private CircleCollider2D collider;
    [SerializeField] private float speed = 5f;
    private Vector3 targetPosition;
    private Rigidbody2D rb;

    void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveBullet(Vector3 startPosition, float direction)
    {
        transform.position = startPosition;
        targetPosition = startPosition + new Vector3(5 * direction, 0, 0); // Đích đến

        // Bắt đầu di chuyển đạn
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        
        Destroy(gameObject); // Hủy khi đạt mục tiêu
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bot"))
        {
             Debug.Log("Đạn đã chạm vào Bot - Dừng lại!");
            rb.velocity = Vector2.zero; // Dừng di chuyển
            rb.isKinematic = true; // Ngăn đạn bị lực tác động
            Destroy(gameObject, 0.1f); // Hủy đạn sau 0.1 giây
        }
    }
}
