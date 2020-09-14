using UnityEngine;

public class HelpVictory : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnPoint
    {
        public Transform Point;
        public RandomHelpObject[] SpawnableHelpObjectsPrefabs;
    }

    public SpawnPoint[] SpawnPoints;
    public float TimeBeforeHelp = 1f;

    public UnlockTrapeze Trapeze = null;

    float timer = 0f;
    bool canUseHelp = true;

    void Awake()
    {
        Trapeze.gameObject.SetActive(true);
        Trapeze.ActivateEvent.AddListener(SpawnRandomObject);
    }

    void Update()
    {
        if (canUseHelp)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeHelp);
            if (timer == TimeBeforeHelp)
            {
                Trapeze.SetIsActivated(true);
                timer = 0f;
                canUseHelp = false;
            }
        }
    }

    public void SpawnRandomObject()
    {
        SpawnPoint randomSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Instantiate(randomSpawnPoint.SpawnableHelpObjectsPrefabs[Random.Range(0, randomSpawnPoint.SpawnableHelpObjectsPrefabs.Length)], randomSpawnPoint.Point.position, randomSpawnPoint.Point.rotation).OnDeath.AddListener(OnHelpObjectDestroyed);
    }

    public void OnHelpObjectDestroyed()
    {
        canUseHelp = true;
    }
}
