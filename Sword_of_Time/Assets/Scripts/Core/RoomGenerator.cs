using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [Header("RoomDetails")]
    [SerializeField] private Vector2 roomSize = new Vector2(15f, 5f);
    [SerializeField] private Transform startPoint;
    [Header("Tiles")]
    [SerializeField] private GameObject[] groundTiles;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [Header("Decorations")]
    [SerializeField] private DecorationPattern[] decorationPatterns;
    [Header("Obstacles")]
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private int minSpikeSpacing;
    [SerializeField] private float spikeSpawnChance = 0.2f;
    [Header("Collectibles")]
    [SerializeField] private GameObject[] collectibles;
    [Header("Abilities")]
    [SerializeField] private GameObject[] abilityPrefabs;
    [Header("Enemy")]
    [SerializeField] private GameObject[] EnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPatrolPrefab;
    [SerializeField] private GameObject meleeEnemyPatrolPrefab;
    [SerializeField] private GameObject roomBackgroundPrefab;


    private List<Vector3> rightWallPositions = new();
    private List<Vector3> ceilingPositions = new();
    private List<Vector3> groundPositions = new();

    public int numberOfRooms = 5;

    
    [System.Serializable]
    public class DecorationPattern
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance = 0.5f;
    }

    struct PlacementData
    {
        public Vector3 position;
        public Quaternion rotation;

        public PlacementData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    void Start()
    {
        GenerateRooms();
    }



    void GenerateRooms()
    {
        Vector3 currentOrigin = startPoint.position;

        for (int i = 0; i < numberOfRooms; i++)
        {
            int numberRoom = i + 2;
            GameObject room = new GameObject("Room" + numberRoom);
            room.transform.position = currentOrigin;
     
            List<float> spikeXPositions = new(); // Track placed spike Xs

            // Floor
            for (float x = 0; x < roomSize.x; x += 1f)
            {
                Vector3 groundPos = currentOrigin + new Vector3(x, roomSize.y / 2.35f, 0);
                groundPositions.Add(groundPos);
                Instantiate(GetRandom(groundTiles), groundPos, Quaternion.identity, room.transform);

                // Check spike spacing
                bool canPlaceSpike = true;
                foreach (float existingX in spikeXPositions)
                {
                    if (Mathf.Abs(x - existingX) < minSpikeSpacing)
                    {
                        canPlaceSpike = false;
                        break;
                    }
                }

                if (canPlaceSpike && Random.value < spikeSpawnChance)
                {
                    GameObject spike = GetSpikePrefab();
                    if (spike != null)
                    {
                        Vector3 spikePos = groundPos + new Vector3(0, -5.0f, 0);
                        Instantiate(spike, spikePos, Quaternion.identity, room.transform);
                        spikeXPositions.Add(x); // Remember this spike
                    }
                }
            }


            List<float> ceilingSpikeXPositions = new();

            for (float x = 0; x < roomSize.x; x += 1f)
            {
                Vector3 ceilingPos = currentOrigin + new Vector3(x, roomSize.y, 0);
                ceilingPositions.Add(ceilingPos);
                Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity, room.transform);

                bool canPlaceSpike = true;
                foreach (float existingX in ceilingSpikeXPositions)
                {
                    if (Mathf.Abs(x - existingX) < minSpikeSpacing)
                    {
                        canPlaceSpike = false;
                        break;
                    }
                }

                if (canPlaceSpike && Random.value < spikeSpawnChance)
                {
                    GameObject spike = GetSpikePrefab();
                    if (spike != null)
                    {
                        Vector3 spikePos = ceilingPos + new Vector3(0, 1.9f, 0);
                        Instantiate(spike, spikePos, Quaternion.Euler(0, 0, 180f), room.transform);
                        ceilingSpikeXPositions.Add(x);
                    }
                }
            }

            // Right wall

            for (float y = 0; y < roomSize.y; y += 1f)
            {
                Vector3 wallPos = currentOrigin + new Vector3(roomSize.x, roomSize.y / 2.35f, 0);

                if (y == 0) // Only at bottom of the wall
                {
                    // Instantiate the full gate prefab (arrow + wall + start marker)
                    GameObject gate = Instantiate(wallPrefab, wallPos, Quaternion.identity, room.transform);
                }

                rightWallPositions.Add(wallPos);
            }




            // Obstacles (other than spikes)
            for (int j = 0; j < Random.Range(1, 4); j++)
            {
                GameObject obstacle = GetRandom(obstacles);

                if (obstacle.CompareTag("Spike")) continue;

                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1f, roomSize.x - 1f),
                    Random.Range(1f, roomSize.y - 1f),
                    0
                );
                Instantiate(obstacle, pos, Quaternion.identity, room.transform);
            }

            // Collectibles
            for (int k = 0; k < Random.Range(0, 2); k++)
            {
                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1, roomSize.x - 1),
                    Random.Range(1f, roomSize.y - 3f),
                    0
                );
                Instantiate(GetRandom(collectibles), pos, Quaternion.identity, room.transform);
                if (Random.value < 0.5f && abilityPrefabs.Length > 0)
                {
                    Vector3 abilityPos = currentOrigin + new Vector3(
                        Random.Range(1f, roomSize.x - 1f),
                        Random.Range(1f, roomSize.y - 3f),
                        0
                    );

                    GameObject selectedAbility = GetRandom(abilityPrefabs);

                    if (selectedAbility != null)
                    {
                        Instantiate(selectedAbility, abilityPos, Quaternion.identity, room.transform);
                    }
                    else
                    {
                        Debug.LogWarning("Selected ability prefab was null.");
                    }
                }

            }


            //Enemy
            // Enemy
            for (int z = 0; z < Random.Range(1, 3); z++)
            {
                Vector3 pos;
                GameObject enemyToSpawn = GetRandom(EnemyPrefab);

                // Adjust height for ranged enemies
                if (enemyToSpawn.name.Contains("RangedEnemyHolder"))
                {
                    pos = currentOrigin + new Vector3(
                        Random.Range(1, roomSize.x - 1),
                        roomSize.y / 6.5f - 0.9f,
                        0
                    );
                }
                else
                {
                    pos = currentOrigin + new Vector3(
                        Random.Range(1, roomSize.x - 1),
                        roomSize.y / 6.5f - 1f,
                        0
                    );
                }

                if (!IsEnemyOverlapping(pos, enemyToSpawn))
                {
                    GameObject spawnedEnemy = Instantiate(enemyToSpawn, pos, Quaternion.identity, room.transform);

                    // Flip the enemy if it is NOT a patrol enemy
                    if (!spawnedEnemy.name.Contains("Patrol"))
                    {
                        Vector3 scale = spawnedEnemy.transform.localScale;
                        scale.x = Mathf.Abs(scale.x) * -1f; // face left
                        spawnedEnemy.transform.localScale = scale;
                    }
                }
            }


            //Dec
            foreach (var pattern in decorationPatterns)
            {
                if (Random.value < pattern.spawnChance)
                {
                    if (pattern.prefab.name == "BoxShapeL" || pattern.prefab.name == "BoxShapeU")
                    {
                        Vector3 pos = currentOrigin + new Vector3(
                            Random.Range(2f, roomSize.x - 4f),
                            roomSize.y / 6.5f,
                            -1.8f
                        );

                        if (!IsOverlapping(pos, pattern.prefab))
                            Instantiate(pattern.prefab, pos, Quaternion.identity, room.transform);
                    }
                    else if (pattern.prefab.name == "BoxTower")
                    {
                        Vector3 pos = currentOrigin + new Vector3(
                            Random.Range(2f, roomSize.x - 4f),
                            roomSize.y / 2.1f,
                            -1.8f
                        );

                        if (!IsOverlapping(pos, pattern.prefab))
                            Instantiate(pattern.prefab, pos, Quaternion.identity, room.transform);
                    }
                    else
                    {
                        Vector3 pos = currentOrigin + new Vector3(
                            Random.Range(2f, roomSize.x - 4f),
                            roomSize.y / 6.5f - 1.25f,
                            -1.8f
                        );

                        if (!IsOverlapping(pos, pattern.prefab))
                            Instantiate(pattern.prefab, pos, Quaternion.identity, room.transform);
                    }



                    break; // Spawn only one pattern per room
                }
            }

            if (Random.value < 0.5f)
            {
                GameObject patrolPrefab = Random.value < 0.5f ? rangedEnemyPatrolPrefab : meleeEnemyPatrolPrefab;

                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1f, roomSize.x - 1f),
                    roomSize.y / 6.5f - 1.75f,
                    0
                );

                if (!IsEnemyOverlapping(pos, patrolPrefab))
                {
                    Instantiate(patrolPrefab, pos, Quaternion.identity, room.transform);
                }
            }


            currentOrigin += new Vector3(roomSize.x, 0, 0);
        }
    }

    // Returns a random spike prefab from obstacles array
    GameObject GetSpikePrefab()
    {
        List<GameObject> spikes = new();
        foreach (var obj in obstacles)
        {
            if (obj.CompareTag("Spike")) spikes.Add(obj);
        }
        return spikes.Count > 0 ? spikes[Random.Range(0, spikes.Count)] : null;
    }
    GameObject GetRandom(GameObject[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    bool IsOverlapping(Vector3 position, GameObject prefab)
    {
        BoxCollider2D collider = prefab.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogWarning($"{prefab.name} has no BoxCollider2D.");
            return false; 
        }

        Vector2 size = collider.size;
        Vector2 offset = collider.offset;

        Vector2 worldCenter = (Vector2)position + offset;
        Collider2D hit = Physics2D.OverlapBox(worldCenter, size, 0f);

        return hit != null;
    }
    bool IsEnemyOverlapping(Vector3 position, GameObject enemyPrefab)
    {
        Animator animator = enemyPrefab.GetComponent<Animator>();

        // If the enemy has no collider, just allow it to spawn.
        Collider2D collider = enemyPrefab.GetComponent<Collider2D>();
        if (collider == null)
        {
            // Optionally add a check for tag
            if (enemyPrefab.CompareTag("Enemy"))
            {
                Debug.LogWarning($"{enemyPrefab.name} has no Collider2D, but is tagged as 'Enemy'. Allowing spawn.");
                return false; // Allow spawn
            }
            else
            {
                Debug.LogWarning($"{enemyPrefab.name} has no Collider2D and is not tagged as 'Enemy'. Assuming overlap.");
                return true; // Be conservative if it's not tagged correctly
            }
        }

        Vector2 size = collider.bounds.size;
        Vector2 center = (Vector2)position + collider.offset;

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, LayerMask.GetMask("Default")); // Use proper layer
        return hit != null;
    }


}
