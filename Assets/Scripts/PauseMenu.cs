using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel của menu pause

    void Start()
    {
        // Tìm các nút theo Tag
        GameObject homeButton = GameObject.FindGameObjectWithTag("Home");
        GameObject restartButton = GameObject.FindGameObjectWithTag("Restart");
        GameObject resumeButton = GameObject.FindGameObjectWithTag("Resume");

        // Gán sự kiện OnClick nếu các nút tồn tại
        if (homeButton != null)
            homeButton.GetComponent<Button>().onClick.AddListener(Home);

        if (restartButton != null)
            restartButton.GetComponent<Button>().onClick.AddListener(Restart);

        if (resumeButton != null)
            resumeButton.GetComponent<Button>().onClick.AddListener(Resume);
    }

    // Hàm Resume - Tiếp tục game
    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false); // Ẩn menu pause

        Time.timeScale = 1f; // Tiếp tục thời gian
    }

    // Hàm Restart - Load lại Scene hiện tại
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Đảm bảo game không bị pause sau khi restart
    }

    // Hàm Home - Quay về Scene 0 (Main Menu)
    public void Home()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f; // Đảm bảo thời gian bình thường khi về menu
    }

    // Hàm để kích hoạt Pause Menu (có thể gọi từ script khác)
    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true); // Hiện menu pause

        Time.timeScale = 0f; // Dừng thời gian (Pause game)
    }
}