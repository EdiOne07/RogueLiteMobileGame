using UnityEditor.Media;
using UnityEngine;
using UnityEngine.UI;
public class UiText : MonoBehaviour
{
    [SerializeField]private string volumeName;
    [SerializeField] private string description;
    private Text txt;
    private void Awake()
    {
        txt = GetComponent<Text>();
    }
    private void Update()
    {
        UpdateVolume();
    }
    private void UpdateVolume()
    {
        float volumeValue=PlayerPrefs.GetFloat(volumeName)*100;
        txt.text = description + volumeValue.ToString();
    }
}
