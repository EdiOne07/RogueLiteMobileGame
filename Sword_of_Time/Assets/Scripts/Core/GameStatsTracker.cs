using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class GameStats
{
    public float totalPlayTime;
    public int deathCount;
  
    public List<string> pickedAbilities = new();

}

public class GameStatsTracker : MonoBehaviour
{
    public static GameStatsTracker Instance;

    private GameStats stats = new();
    private float sessionStartTime;
    private string savePath;

    [Header("Google Form Settings")]
    [SerializeField] private string firebaseURL = "https://sword-of-time-statistics-default-rtdb.firebaseio.com/gameStats.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "gameStats.json");
            LoadStats();
            sessionStartTime = Time.time;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UploadStatsFromFile();
        }
    }

    private void OnApplicationQuit()
    {
        SaveStats();
        
    }

    public void RecordAbilityPickup(string abilityName)
    {
        if (!stats.pickedAbilities.Contains(abilityName))
            stats.pickedAbilities.Add(abilityName);
    }


    public void RecordDeath() => stats.deathCount++;

    public void SaveStats()
    {
        stats.totalPlayTime += Time.time - sessionStartTime;
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadStats()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            stats = JsonUtility.FromJson<GameStats>(json);
        }
    }

    public void UploadStatsFromFile()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameStats stats = JsonUtility.FromJson<GameStats>(json);

            string abilitySummary = string.Join(", ", stats.pickedAbilities);

            StartCoroutine(PostToFirebase());
        }
        else
        {
            Debug.LogWarning("Stats file not found.");
        }
    }

    private IEnumerator PostToFirebase()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gameStats.json");
        if (!File.Exists(filePath))
        {
            Debug.LogError("Stats file not found!");
            yield break;
        }

        string json = File.ReadAllText(filePath);
        Debug.Log("Uploading JSON: " + json);

        // Replace with your actual Firebase DB URL
        string firebaseUrl = "https://sword-of-time-statistics-default-rtdb.firebaseio.com/gameStats.json";

        UnityWebRequest request = new UnityWebRequest(firebaseUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Upload failed: " + request.error);
        }
        else
        {
            Debug.Log("Upload successful: " + request.downloadHandler.text);
        }

        // Reset stats and overwrite the local file
       // stats = new GameStats();  
       //File.WriteAllText(savePath, JsonUtility.ToJson(stats, true));
        //Debug.Log("Local stats file reset after upload.");

    }



    [Serializable]
    public class Wrapper
    {
        public Dictionary<string, object> wrapper;

        public Wrapper(Dictionary<string, object> dict)
        {
            wrapper = dict;
        }
    }



}
