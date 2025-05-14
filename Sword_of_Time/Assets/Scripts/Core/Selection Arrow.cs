using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    [SerializeField]private AudioClip changeSound;
    [SerializeField]private AudioClip interactSound;
    private RectTransform rect;
    private int currentPos;
    private void Awake()
    {
        rect= GetComponent<RectTransform>();  
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangePos(-1);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePos(1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }

    }
    private void ChangePos(int pos)
    {
        currentPos += pos;
        if (pos != 0)
        {
            SoundManager.instance.PlaySound(changeSound);
        }
        if (currentPos < 0)
        {
            currentPos = options.Length - 1;
        }
        else if (currentPos > options.Length - 1) { 
            currentPos = 0;
        }
        rect.position = new Vector3(rect.position.x, options[currentPos].position.y, 0);
    }
    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);
        options[currentPos].GetComponent<Button>().onClick.Invoke(); ;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
