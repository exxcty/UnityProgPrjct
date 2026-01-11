using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int enemyCount;
        public float spawnRate;
        public float enemyHealthMult = 1f;
        public float enemyDamageMult = 1f;
    }

    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;

    private int currentWaveIndex = 0;

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            GameManager.Instance.WinGame();
            yield break;
        }

        Wave wave = waves[currentWaveIndex];

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        currentWaveIndex++;
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnWave());
    }

    private void SpawnEnemy(Wave wave)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, sp.position, Quaternion.identity);

        if (enemy.TryGetComponent(out EnemyAI ai))
        {
            ai.Initialize(3f, 10f * wave.enemyDamageMult, 100f * wave.enemyHealthMult);
        }
    }
}