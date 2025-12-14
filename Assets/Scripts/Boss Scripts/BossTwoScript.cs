using System.Collections.Generic;
using UnityEngine;

public class BossTwoScript : MonoBehaviour
{
    private EnemyManager bossTwo;
    public Transform[] enemySpawnSpots;
    public GameObject enemy;
    public GameObject toughEnemy;

    private bool summonedOne = false;
    private bool summonedTwo = false;

    // track spawned minions so we can wait until they're all defeated
    private readonly List<GameObject> spawnedMinions = new List<GameObject>();
    private bool bossHidden = false;

    // saved components used to hide the boss without deactivating the whole thing
    private Collider2D bossCollider;
    public SpriteRenderer bossSprite;

    void Start()
    {
        bossTwo = GetComponent<EnemyManager>();
        bossCollider = GetComponent<Collider2D>();
        bossTwo.enemyHealth = 6;
    }

    void Update()
    {
        // if the boss is hidden, check the # of minions to see if they can be brought back.
        if (bossHidden)
        {
            CheckMinions();
            return;
        }

        SummonMinions();
    }

    //summon some minons once they lose 2 hp & hide the boss
    private void SummonMinions()
    {
        if (bossTwo == null) return;

        if (bossTwo.enemyHealth == 4 && !summonedOne)
        {
            int spawnCount = Mathf.Min(enemySpawnSpots.Length, 4);
            for (int n = 0; n < spawnCount; n++)
            {
                var spot = enemySpawnSpots[n];
                if (spot != null)
                {
                    GameObject spawned = Instantiate(enemy, spot.position, Quaternion.identity);
                    spawnedMinions.Add(spawned);
                }
            }

            summonedOne = true;
            HideBossUntilMinionsDead();
        }

        if (bossTwo.enemyHealth == 2 && !summonedTwo)
        {
            int spawnCount = Mathf.Min(enemySpawnSpots.Length, 4);
            for (int n = 0; n < spawnCount; n++)
            {
                var spot = enemySpawnSpots[n];
                if (spot == null) continue;

                GameObject spawned;
                if (n <= 1)
                    spawned = Instantiate(toughEnemy, spot.position, Quaternion.identity);
                else
                    spawned = Instantiate(enemy, spot.position, Quaternion.identity);

                spawnedMinions.Add(spawned);
            }

            summonedTwo = true;
            HideBossUntilMinionsDead();
        }
    }

    private void HideBossUntilMinionsDead()
    {
        // disable the saved stuff so boss is outta the way for the minion fight
        if (bossTwo != null) bossTwo.enabled = false;
        if (bossCollider != null) bossCollider.enabled = false;
        if (bossSprite != null) bossSprite.enabled = false;

        bossHidden = true;
    }

    //re-enable the stuff so that the boss can return to fighting you. might wanna make an indicator before they respawn so the player can be ready.
    private void RestoreBoss()
    {
        if (bossTwo != null) bossTwo.enabled = true;
        if (bossCollider != null) bossCollider.enabled = true;
        if (bossSprite != null) bossSprite.enabled = true;

        bossHidden = false;
    }

    private void CheckMinions()
    {
        // remove minions that are null or inactive
        spawnedMinions.RemoveAll(m => m == null || !m.activeInHierarchy);

        if (spawnedMinions.Count == 0)
        {
            RestoreBoss();
        }
    }
}
