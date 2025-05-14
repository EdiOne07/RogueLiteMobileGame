using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] groundTiles;
    [SerializeField] private DecorationPattern[] decorationPatterns;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private GameObject[] collectibles;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject basicWall;
    [SerializeField] private GameObject[] EnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPatrolPrefab;
    [SerializeField] private GameObject meleeEnemyPatrolPrefab;
    [SerializeField] private int minSpikeSpacing; // Tiles apart


    public int numberOfRooms = 5;
    public Vector2 roomSize = new Vector2(15f, 5f); // width, height
    private List<Vector3> rightWallPositions = new();
    private List<Vector3> ceilingPositions = new();
    private List<Vector3> groundPositions = new();
    [SerializeField] private float spikeSpawnChance = 0.2f;
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
            GameObject room = new GameObject("Room_" + i);
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
            for (int k = 0; k < Random.Range(1, 3); k++)
            {
                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1, roomSize.x - 1),
                    Random.Range(1f, roomSize.y - 1f),
                    0
                );
                Instantiate(GetRandom(collectibles), pos, Quaternion.identity, room.transform);
            }
            //Enemy
            for (int z = 0; z < Random.Range(1, 2); z++)
            {
                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1, roomSize.x - 1),
                    roomSize.y / 6.5f - 1f,
                    0
                );
                Instantiate(GetRandom(EnemyPrefab), pos, Quaternion.identity, room.transform);
            }
            //Dec
            foreach (var pattern in decorationPatterns)
            {
                if (Random.value < pattern.spawnChance)
                {
                    if (pattern.prefab.name == "BoxShapeL" || pattern.prefab.name == "BoxTower" || pattern.prefab.name == "BoxShapeU")
                    {
                        Vector3 pos = currentOrigin + new Vector3(
                       Random.Range(2f, roomSize.x - 4f),
                       roomSize.y / 6.5f,
                       0f
                   );
                        Instantiate(pattern.prefab, pos, Quaternion.identity, room.transform);
                    }
                    else
                    {
                        Vector3 pos = currentOrigin + new Vector3(
                           Random.Range(2f, roomSize.x - 4f),
                           roomSize.y / 6.5f - 1.25f,
                           0f
                       );
                        Instantiate(pattern.prefab, pos, Quaternion.identity, room.transform);
                    }


                    break; // Spawn only one pattern per room
                }
            }

            if (Random.value < 0.5f) // 50% chance to spawn a patrol enemy
            {
                GameObject patrolPrefab = Random.value < 0.5f ? rangedEnemyPatrolPrefab : meleeEnemyPatrolPrefab;

                Vector3 pos = currentOrigin + new Vector3(
                    Random.Range(1f, roomSize.x - 1f),
                   roomSize.y / 6.5f - 1.75f,
                    0
                );

                Instantiate(patrolPrefab, pos, Quaternion.identity, room.transform);
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
    GameObject GetBasicWallOnly()
    {
        return basicWall;
    }
}
