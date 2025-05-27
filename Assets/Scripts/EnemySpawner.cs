using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject urkPrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnDelayBetweenUrksInWave = 1.0f;
    public float delayBeforeFirstWave = 2.0f;

    [Header("Testing")]
    public int currentDayForTesting = 1; // Установи день для теста в Inspector
    public KeyCode testSpawnKey = KeyCode.P; // Клавиша для запуска спавна

    private bool isSpawning = false; // Флаг, чтобы не запускать спавн несколько раз одновременно

    // Временный Update для тестирования
    void Update()
    {
        if (Input.GetKeyDown(testSpawnKey))
        {
            if (!isSpawning)
            {
                Debug.Log($"--- Тестовый запуск спавна для Дня {currentDayForTesting} по нажатию {testSpawnKey} ---");
                // Передаем день, установленный в Inspector
                StartSpawningNightEnemies(currentDayForTesting);
            }
            else
            {
                Debug.Log("Спавн уже идет!");
            }
        }
    }


    // Эта функция будет вызываться из GameManager, когда наступает ночь (или из Update для теста)
    public void StartSpawningNightEnemies(int dayNumber)
    {
        if (isSpawning) return; // Если уже спавним, ничего не делаем

        if (urkPrefab == null)
        {
            Debug.LogError("Urk Prefab не назначен в EnemySpawner!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Точки спавна (Spawn Points) не назначены в EnemySpawner!");
            return;
        }

        isSpawning = true; // Устанавливаем флаг, что спавн начался
        Debug.Log($"EnemySpawner: Начинаем спавн врагов для ночи Дня {dayNumber}.");
        StartCoroutine(SpawnLogicForNight(dayNumber));
    }

    IEnumerator SpawnLogicForNight(int dayNumber)
    {
        yield return new WaitForSeconds(delayBeforeFirstWave);

        int urksToSpawnThisNight = 0;

        switch (dayNumber)
        {
            case 1:
                urksToSpawnThisNight = 1;
                break;
            case 2:
                urksToSpawnThisNight = Random.Range(4, 7);
                break;
            case 3:
                int squads = Random.Range(2, 4);
                urksToSpawnThisNight = squads * Random.Range(3, 6);
                break;
            case 4:
                urksToSpawnThisNight = Random.Range(5, 8);
                break;
            case 5:
                Debug.Log("День 5: Урки не нападают.");
                isSpawning = false; // Сбрасываем флаг
                yield break;
            default:
                if (dayNumber > 5)
                {
                    Debug.LogWarning($"EnemySpawner: День {dayNumber} не описан в логике спавна GDD, спавним стандартное количество.");
                    urksToSpawnThisNight = Random.Range(3, 6);
                }
                else
                {
                    Debug.LogWarning($"EnemySpawner: Неизвестный номер дня для спавна: {dayNumber}");
                    isSpawning = false; // Сбрасываем флаг
                    yield break;
                }
                break;
        }

        Debug.Log($"Планируется заспавнить {urksToSpawnThisNight} урков.");

        for (int i = 0; i < urksToSpawnThisNight; i++)
        {
            if (spawnPoints == null || spawnPoints.Length == 0) // Дополнительная проверка на случай удаления точек во время спавна
            {
                Debug.LogError("Точки спавна были удалены во время спавна!");
                isSpawning = false; // Сбрасываем флаг
                yield break;
            }
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (randomSpawnPoint == null)
            {
                Debug.LogWarning("Одна из точек спавна null, пропускаем спавн этого урка.");
                continue;
            }

            // Debug.Log($"Спавним Урка #{i + 1} в точке {randomSpawnPoint.name} ({randomSpawnPoint.position})");
            GameObject newUrkGO = Instantiate(urkPrefab, randomSpawnPoint.position, Quaternion.identity);

            UrkAI urkAI = newUrkGO.GetComponent<UrkAI>();
            if (urkAI != null)
            {
                urkAI.InitializeStats(dayNumber);
            }
            else
            {
                Debug.LogError("Созданный префаб Урка не содержит скрипт UrkAI!");
            }

            yield return new WaitForSeconds(spawnDelayBetweenUrksInWave);
        }
        Debug.Log($"EnemySpawner: Спавн для Дня {dayNumber} завершен.");
        isSpawning = false; // Сбрасываем флаг, когда все заспавнены
    }
}