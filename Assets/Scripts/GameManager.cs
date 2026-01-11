using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float timeToWin = 300f;
    [SerializeField] private float timeBetweenWaves = 10f; 

    [Header("Wave Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float spawnRadiusMin = 5f;
    [SerializeField] private float spawnRadiusMax = 10f;

    [Header("Items Settings")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private int itemsPerWave = 2;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    public int Score { get; private set; }

    private float survivalTimer;
    private float waveTimer;
    private int currentWave = 0;
    private bool isGameOver = false;

    private Transform playerTransform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Игрок не найден");
        }
    }

    private void Start()
    {
        SpawnWave();
    }

    private void Update()
    {
        if (isGameOver || playerTransform == null) return;

        survivalTimer += Time.deltaTime;
        UpdateUI();

        if (survivalTimer >= timeToWin)
        {
            WinGame();
        }

        waveTimer += Time.deltaTime;
        if (waveTimer >= timeBetweenWaves)
        {
            waveTimer = 0f;
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        currentWave++;

        int enemyCount;

        if (currentWave < 7)
        {
            enemyCount = 3 + (currentWave - 1);
        }
        else
        {
            enemyCount = 8;
        }

        float baseSpeed = 3f;
        float currentSpeed = baseSpeed;

        if (currentWave >= 7)
        {
            int wavesAfterCap = currentWave - 6;
            currentSpeed += wavesAfterCap * 0.04f;
        }

        float enemyHealth = 100f + ((currentWave - 1) * 10f);
        float enemyDamage = 10f + (currentWave * 2f);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 pos = GetRandomSpawnPosition();
            if (pos != Vector2.zero)
            {
                GameObject enemy = Instantiate(zombiePrefab, pos, Quaternion.identity);

                if (enemy.TryGetComponent(out EnemyAI ai))
                {
                    ai.Initialize(currentSpeed, enemyDamage, enemyHealth);
                }

                if (enemy.TryGetComponent(out Health health))
                {
                    health.SetMaxHealth(enemyHealth);
                }
            }
        }

        if (itemPrefabs != null && itemPrefabs.Length > 0)
        {
            for (int i = 0; i < itemsPerWave; i++)
            {
                Vector2 pos = GetRandomSpawnPosition();
                if (pos != Vector2.zero)
                {
                    int randomIndex = Random.Range(0, itemPrefabs.Length);
                    Instantiate(itemPrefabs[randomIndex], pos, Quaternion.identity);
                }
            }
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnRadiusMin, spawnRadiusMax);
            Vector2 potentialPos = (Vector2)playerTransform.position + randomDir * distance;

            if (MapGenerator.Instance != null)
            {
                potentialPos.x = Mathf.Clamp(potentialPos.x, MapGenerator.Instance.MinBounds.x, MapGenerator.Instance.MaxBounds.x);
                potentialPos.y = Mathf.Clamp(potentialPos.y, MapGenerator.Instance.MinBounds.y, MapGenerator.Instance.MaxBounds.y);
            }

            if (!Physics2D.OverlapCircle(potentialPos, 0.5f, LayerMask.GetMask("Default")))
            {
                return potentialPos;
            }
        }
        return Vector2.zero; 
    }

    private void UpdateUI()
    {
        float timeLeft = Mathf.Max(0, timeToWin - survivalTimer);

        if (timerText)
            timerText.text = $"{Mathf.FloorToInt(timeLeft / 60):00}:{Mathf.FloorToInt(timeLeft % 60):00}";

        if (waveText)
            waveText.text = $"Волна: {currentWave}";
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }

    public void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (loseScreen) loseScreen.SetActive(true);
    }

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (winScreen) winScreen.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}