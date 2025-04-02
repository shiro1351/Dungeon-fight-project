using System.Collections;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // Vị trí mới sau khi dịch chuyển
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Kiểm tra xem có phải Player không
        {
            collision.transform.position = targetPosition.position; // Dịch chuyển Player
        }
    }

}
