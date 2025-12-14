using UnityEngine;

public class FinalBossScript : MonoBehaviour
{
    EnemyManager finalBossEnemy;
    public GameObject shield;
    public SpriteRenderer shieldSprite;
    public Transform[] summonSpots;
    public GameObject enemy;
    public bool shieldUp = true;
    bool firstWaveSummoned = false;
    bool secondWaveSummoned = false;

    void Start()
    {
        finalBossEnemy = GetComponent<EnemyManager>();
        shieldSprite = shield.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (shield != null && finalBossEnemy.enemyHealth == 4 && !shieldUp && !firstWaveSummoned)
        {
            firstWave();
            shieldUp = true;
            shield.SetActive(true);
        }
        if (shield != null && finalBossEnemy.enemyHealth == 2 && !shieldUp && !secondWaveSummoned)
        {
            secondWave();
            shieldUp = true;
            shield.SetActive(true);
        }
        if (!shieldUp)
        {
            shield.SetActive(false);
        }
    }

    private void firstWave()
    {
        int spawnCount = 2;
        for (int n = 0; n < spawnCount; n++)
        {
            var spot = summonSpots[n];
            if (spot != null)
            {
                GameObject spawned = Instantiate(enemy, spot.position, Quaternion.identity);
            }
        }
        firstWaveSummoned = true;
        shieldUp = true;
        shieldSprite.enabled = true;
    }
    private void secondWave()
    {
        int spawnCount = 4;
        for (int n = 0; n < spawnCount; n++)
        {
            var spot = summonSpots[n];
            if (spot != null)
            {
                GameObject spawned = Instantiate(enemy, spot.position, Quaternion.identity);
            }
        }
        secondWaveSummoned = true;
        shieldUp = true;
        shieldSprite.enabled = true;
    }

    public void shieldBreak()
    {
        if (shieldUp)
        {
            shieldUp = false;
            shieldSprite.enabled = false;
        }
    }

}
