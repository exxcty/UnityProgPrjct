using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    [Header("Settings")]
    public int width = 20;  
    public int height = 20;  
    public int innerWallCount = 15;

    [Header("Adjustment")]
    public float tileSize = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;

    public Vector2 MinBounds { get; private set; }
    public Vector2 MaxBounds { get; private set; }

    private void Awake()
    {
        Instance = this;
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        MinBounds = new Vector2(-halfWidth + 1, -halfHeight + 1) * tileSize;
        MaxBounds = new Vector2(halfWidth - 1, halfHeight - 1) * tileSize;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                Vector2 pos = new Vector2(x * tileSize, y * tileSize);

                if (x == -halfWidth || x == halfWidth || y == -halfHeight || y == halfHeight)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }
            }
        }

        SpawnInnerWalls(halfWidth, halfHeight);
    }

    private void SpawnInnerWalls(int halfW, int halfH)
    {
        for (int i = 0; i < innerWallCount; i++)
        {
            int randX = Random.Range(-halfW + 2, halfW - 2);
            int randY = Random.Range(-halfH + 2, halfH - 2);

            Vector2 pos = new Vector2(randX * tileSize, randY * tileSize);

            if (Vector2.Distance(pos, Vector2.zero) > (3f * tileSize))
            {
                if (!Physics2D.OverlapCircle(pos, 0.4f * tileSize))
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}