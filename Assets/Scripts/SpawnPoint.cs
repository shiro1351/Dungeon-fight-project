using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject enemyPrefab;  // Prefab của enemy
    public int enemyCount = 3;      // Số lượng enemy spawn ra
    private bool hasSpawned = false; // Tránh spawn nhiều lần

    private void Start()
    {
        SpawnEnemies(); // Spawn ngay khi game bắt đầu
        hasSpawned = true; // Đánh dấu đã spawn
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy spawned at: " + transform.position);
        }
    }
}
