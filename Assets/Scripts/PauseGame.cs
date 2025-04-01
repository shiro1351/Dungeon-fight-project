using UnityEngine;
using UnityEngine.UI; // Import để dùng Button

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu; // Kéo Panel PauseMenu vào đây trong Inspector
    public Button pauseButton; // Kéo Button vào đây trong Inspector
    private bool isPaused = false;

    void Start()
    {
        // Ẩn menu khi bắt đầu game
        pauseMenu.SetActive(false);

        // Gán sự kiện cho nút PauseButton
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; // Dừng hoặc tiếp tục game
        pauseMenu.SetActive(isPaused); // Bật/tắt giao diện Pause
    }
}
