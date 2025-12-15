using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOneScript : MonoBehaviour
{
    private EnemyManager bossOne;

    // 10 available hole spots set in the inspector (these are existing pit GameObjects)
    public Transform[] holeSpots;

    // how many holes to enable at each threshold
    public int holesToSpawn = 3;

    public GameObject pitWarningPrefab;
    public float warningDuration = 2f;

    // track enabled holes so they can be referenced/cleaned later if needed
    private readonly List<GameObject> spawnedHoles = new List<GameObject>();

    // ensure each threshold only triggers once
    private bool holesAt5Triggered = false;
    private bool holesAt3Triggered = false;
    private bool holesAt1Triggered = false;

    void Start()
    {
        bossOne = GetComponent<EnemyManager>();

        if (bossOne == null)
        {
            Debug.LogWarning("BossOneScript: EnemyManager not found on boss object.");
        }
        else
        {
            // initialize boss health as before
            bossOne.enemyHealth = 6;
        }
    }

    void Update()
    {
        if (bossOne == null) return;

        // enable holes on reaching 5, 3, and 1 health (each only once)
        if (bossOne.enemyHealth == 5 && !holesAt5Triggered)
        {
            holesAt5Triggered = true;
            StartCoroutine(EnableHolesAfterDelay());
        }
        else if (bossOne.enemyHealth == 3 && !holesAt3Triggered)
        {
            holesAt3Triggered = true;
            StartCoroutine(EnableHolesAfterDelay());
        }
        else if (bossOne.enemyHealth == 1 && !holesAt1Triggered)
        {
            holesAt1Triggered = true;
            StartCoroutine(EnableHolesAfterDelay());
        }
    }

    private IEnumerator EnableHolesAfterDelay()
    {
        if (holeSpots == null || holeSpots.Length == 0)
            yield break;

        // Step 1: Collect inactive pit indices
        List<int> candidates = new List<int>();
        for (int i = 0; i < holeSpots.Length; i++)
        {
            if (holeSpots[i] != null && !holeSpots[i].gameObject.activeInHierarchy)
                candidates.Add(i);
        }

        if (candidates.Count == 0)
            yield break;

        int toSpawn = Mathf.Min(holesToSpawn, candidates.Count);

        // Step 2: Choose pits NOW
        List<Transform> chosenSpots = new List<Transform>();
        for (int i = 0; i < toSpawn; i++)
        {
            int pick = Random.Range(0, candidates.Count);
            int index = candidates[pick];
            candidates.RemoveAt(pick);

            chosenSpots.Add(holeSpots[index]);
        }

        // Step 3: Spawn warning visuals
        List<GameObject> warnings = new List<GameObject>();
        foreach (Transform spot in chosenSpots)
        {
            if (spot == null || pitWarningPrefab == null) continue;

            GameObject warning = Instantiate(
                pitWarningPrefab,
                spot.position,
                spot.rotation
            );
            warning.tag = "Warning";
            warnings.Add(warning);
        }

        // Step 4: Wait before enabling pits
        yield return new WaitForSeconds(warningDuration);

        // Step 5: Remove warnings and enable pits
        foreach (GameObject warning in warnings)
        {
            if (warning != null)
                Destroy(warning);
        }

        foreach (Transform spot in chosenSpots)
        {
            if (spot == null) continue;

            GameObject pit = spot.gameObject;
            pit.SetActive(true);
            pit.tag = "BossPit";

            if (!spawnedHoles.Contains(pit))
                spawnedHoles.Add(pit);
        }
    }

}
