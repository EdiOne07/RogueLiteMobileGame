using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP (if you're using URP effects)

public class TimeRewind : MonoBehaviour
{
    public bool isRewinding = false;

    [Header("Rewind Settings")]
    public float rewindDuration = 2f;
    private List<PointInTime> pointsInTime;
    private Rigidbody2D rb;

    [Header("Rewind Visuals")]
    public Volume postProcessVolume;      // Grayscale volume
    public ParticleSystem rewindParticles;

    [Header("Affected Objects")]
    private IRewindable[] rewindables;
    private GameObject[] timeAffectedObjects;
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        timeAffectedObjects = GameObject.FindGameObjectsWithTag("Enemy");

        // Make sure the grayscale is off initially
        if (postProcessVolume != null)
            postProcessVolume.enabled = false;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        List<IRewindable> temp = new List<IRewindable>();
        foreach (var obj in objects)
        {
            var rewindable = obj.GetComponent<IRewindable>();
            if (rewindable != null)
                temp.Add(rewindable);
        }
        rewindables = temp.ToArray();

        // Similarly for traps if they have a different tag:
        GameObject[] trapObjects = GameObject.FindGameObjectsWithTag("Trap");
        foreach (var obj in trapObjects)
        {
            var rewindable = obj.GetComponent<IRewindable>();
            if (rewindable != null)
                temp.Add(rewindable);
        }
        rewindables = temp.ToArray();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            StartRewind();
        if (Input.GetKeyUp(KeyCode.R))
            StopRewind();
        if (isRewinding)
            Rewind();
        else
            Record();
    }

  

    private void StartRewind()
    {
        isRewinding = true;
        rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (GameObject obj in timeAffectedObjects)
        {
            Rigidbody2D rb2d = obj.GetComponent<Rigidbody2D>();
            if (rb2d != null)
                rb2d.bodyType = RigidbodyType2D.Kinematic;
        }

        // Freeze nearby enemies or objects tagged as "Enemy"
        foreach (var rewindable in rewindables)
            rewindable.OnRewindStart();

        // Enable grayscale
        if (postProcessVolume != null)
            postProcessVolume.enabled = true;

        // Start rewind particles
        if (rewindParticles != null)
            rewindParticles.Play();
    }

    private void StopRewind()
    {
        isRewinding = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        foreach (GameObject obj in timeAffectedObjects)
        {
            Rigidbody2D rb2d = obj.GetComponent<Rigidbody2D>();
            if (rb2d != null)
                rb2d.bodyType = RigidbodyType2D.Dynamic;
        }
        foreach (var rewindable in rewindables)
            rewindable.OnRewindStop();

        // Disable grayscale
        if (postProcessVolume != null)
            postProcessVolume.enabled = false;

        // Stop particles
        if (rewindParticles != null)
            rewindParticles.Stop();
    }

    private void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime point = pointsInTime[0];
            transform.position = point.position;
            transform.rotation = point.rotation;
            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind(); // Automatically stop if out of recorded data
        }
    }

    private void Record()
    {
        // Limit buffer to duration
        if (pointsInTime.Count > Mathf.Round(rewindDuration / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }
}
