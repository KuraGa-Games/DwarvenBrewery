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
    public int currentDayForTesting = 1; // �������� ���� ��� ����� � Inspector
    public KeyCode testSpawnKey = KeyCode.P; // ������� ��� ������� ������

    private bool isSpawning = false; // ����, ����� �� ��������� ����� ��������� ��� ������������

    // ��������� Update ��� ������������
    void Update()
    {
        if (Input.GetKeyDown(testSpawnKey))
        {
            if (!isSpawning)
            {
                Debug.Log($"--- �������� ������ ������ ��� ��� {currentDayForTesting} �� ������� {testSpawnKey} ---");
                // �������� ����, ������������� � Inspector
                StartSpawningNightEnemies(currentDayForTesting);
            }
            else
            {
                Debug.Log("����� ��� ����!");
            }
        }
    }


    // ��� ������� ����� ���������� �� GameManager, ����� ��������� ���� (��� �� Update ��� �����)
    public void StartSpawningNightEnemies(int dayNumber)
    {
        if (isSpawning) return; // ���� ��� �������, ������ �� ������

        if (urkPrefab == null)
        {
            Debug.LogError("Urk Prefab �� �������� � EnemySpawner!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("����� ������ (Spawn Points) �� ��������� � EnemySpawner!");
            return;
        }

        isSpawning = true; // ������������� ����, ��� ����� �������
        Debug.Log($"EnemySpawner: �������� ����� ������ ��� ���� ��� {dayNumber}.");
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
                Debug.Log("���� 5: ���� �� ��������.");
                isSpawning = false; // ���������� ����
                yield break;
            default:
                if (dayNumber > 5)
                {
                    Debug.LogWarning($"EnemySpawner: ���� {dayNumber} �� ������ � ������ ������ GDD, ������� ����������� ����������.");
                    urksToSpawnThisNight = Random.Range(3, 6);
                }
                else
                {
                    Debug.LogWarning($"EnemySpawner: ����������� ����� ��� ��� ������: {dayNumber}");
                    isSpawning = false; // ���������� ����
                    yield break;
                }
                break;
        }

        Debug.Log($"����������� ���������� {urksToSpawnThisNight} �����.");

        for (int i = 0; i < urksToSpawnThisNight; i++)
        {
            if (spawnPoints == null || spawnPoints.Length == 0) // �������������� �������� �� ������ �������� ����� �� ����� ������
            {
                Debug.LogError("����� ������ ���� ������� �� ����� ������!");
                isSpawning = false; // ���������� ����
                yield break;
            }
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (randomSpawnPoint == null)
            {
                Debug.LogWarning("���� �� ����� ������ null, ���������� ����� ����� ����.");
                continue;
            }

            // Debug.Log($"������� ���� #{i + 1} � ����� {randomSpawnPoint.name} ({randomSpawnPoint.position})");
            GameObject newUrkGO = Instantiate(urkPrefab, randomSpawnPoint.position, Quaternion.identity);

            UrkAI urkAI = newUrkGO.GetComponent<UrkAI>();
            if (urkAI != null)
            {
                urkAI.InitializeStats(dayNumber);
            }
            else
            {
                Debug.LogError("��������� ������ ���� �� �������� ������ UrkAI!");
            }

            yield return new WaitForSeconds(spawnDelayBetweenUrksInWave);
        }
        Debug.Log($"EnemySpawner: ����� ��� ��� {dayNumber} ��������.");
        isSpawning = false; // ���������� ����, ����� ��� ����������
    }
}