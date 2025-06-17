using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    private void Awake()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
            {
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;

    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void OnQuitClicked()
    {
        GameStatsTracker.Instance.SaveStats();
        GameStatsTracker.Instance.UploadStatsFromFile();
        Application.Quit();
    }

    public void OnFeedbackClicked()
    {
        GameStatsTracker.Instance.SaveStats();
        GameStatsTracker.Instance.UploadStatsFromFile();
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSd4h5gqtmdPQvg1HWTNPESz2lwZhJ704KTDnTkaurdhNnF7Ig/viewform?usp=dialog");
    }
    public void GameOver(bool status)
    {
        gameOverScreen.SetActive(status);
        Time.timeScale = 0;
    }
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void PauseGame(bool status)
    {

        pauseScreen.SetActive(status);
        if (status)
        {
            Time.timeScale = 0;
        }
        else
        {

            Time.timeScale = 1;
        }

    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
    public void Music()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
}
