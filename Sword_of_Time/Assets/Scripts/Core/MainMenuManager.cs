using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
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
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
    public void OnQuitClickedMainMenu()
    {
        Application.Quit();
    }
    public void Music()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
}
