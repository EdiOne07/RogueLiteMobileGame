using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class ItemUnlockedHint : MonoBehaviour
{
    public GameObject hintPanel; 
    public float displayTime = 3f;

    private Text hintText;

    void Awake()
    {
        hintText = hintPanel.GetComponent<Text>(); 
        hintPanel.SetActive(false);
    }

    public void ShowHint(string message)
    {
        StopAllCoroutines(); 
        hintText.text = message;
        hintPanel.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        hintPanel.SetActive(false);
    }
}
