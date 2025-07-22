using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("🔫 총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("🕒 전체 생성 간격")]
    public float spawnInterval = 2f;

    [Header("🎯 동시에 생성할 총알 개수")]
    public int bulletCount = 1;

    [Header("🌟 화살 발사 연출용 효과 활 프리팹")]
    public GameObject effectBowPrefab;

    [Header("↩️ 플레이어로부터 활의 거리")]
    public float bowDistance = 1.0f; // Distance from player

    [Header("🎯 플레이어로부터 화살의 거리")]
    public float arrowDistanceFromPlayer = 1.2f; // New: Independent distance for the arrow from the player

    private float timer;
    private GameObject bowInstance; // Assuming this is instantiated elsewhere if needed for the "original bow"
    private GameObject effectBowInstance;
    private Transform playerTransform;
    private bool isBowActive = true;
    private BulletAI lastArrowAI = null; // 최근 발사한(준비중인) 화살
    private bool arrowIsFlying = false;
    private float arrowAngle = 0f;
    private Vector3 currentBowPosition; // Store the calculated bow position
    private Vector3 currentArrowPosition; // Store the calculated arrow position

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        if (effectBowPrefab != null)
        {
            effectBowInstance = Instantiate(effectBowPrefab);
            effectBowInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!GameManager.Instance.IsGame())
            return;

        // Check if there are any enemies in the scene
        bool hasEnemy = false;
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };
        foreach (string tag in enemyTags)
        {
            if (GameObject.FindGameObjectWithTag(tag) != null) { hasEnemy = true; break; }
        }
        if (!hasEnemy) return; // Don't spawn if no enemies
        if (playerTransform == null || bulletPrefab == null) return; // Essential references check

        // Calculate the target direction to the closest enemy from the player
        Transform closestEnemy = FindClosestEnemy(playerTransform.position);
        Vector3 playerToEnemyDir = Vector3.right; // Default direction
        if (closestEnemy != null)
        {
            playerToEnemyDir = (closestEnemy.position - playerTransform.position).normalized;
        }

        // Calculate the bow's position based on its distance from the player
        currentBowPosition = playerTransform.position + playerToEnemyDir * bowDistance;

        // Calculate the arrow's position based on its own independent distance from the player
        currentArrowPosition = playerTransform.position + playerToEnemyDir * arrowDistanceFromPlayer;

        // Calculate the angle for both bow and arrow to face the enemy
        arrowAngle = Mathf.Atan2(playerToEnemyDir.y, playerToEnemyDir.x) * Mathf.Rad2Deg;

        // Sync the positions and directions of the effect bow and the preparing arrow
        SyncBowAndArrowToPlayer();
        SyncBowAndArrowDirection(arrowAngle);

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            // (1) Hide the player's original bow (if it exists and is active)
            arrowIsFlying = false; // Reset arrow flying state
            if (bowInstance != null)
            {
                bowInstance.transform.DOKill(); // Stop any ongoing DOTween animations
                bowInstance.SetActive(false);
                isBowActive = false;
            }

            // (2) Activate and position the effect bow for the firing animation
            if (effectBowInstance != null)
            {
                effectBowInstance.SetActive(true);
                effectBowInstance.transform.position = currentBowPosition; // Set to bow's calculated position
                effectBowInstance.transform.rotation = Quaternion.Euler(0, 0, arrowAngle - 180f); // Adjust rotation for bow sprite
                effectBowInstance.transform.localScale = new Vector3(0.4f, 0.4f, 1f); // Set desired scale for the effect bow
            }

            // (3) Spawn the arrow (bullet) from the object pool
            GameObject bullet = GameManager.Instance.poolManager.SpawnFromPool(
                bulletPrefab.name, currentArrowPosition, Quaternion.Euler(0, 0, arrowAngle) // Spawn at arrow's calculated position
            );

            // Get the BulletAI component and initialize it
            lastArrowAI = bullet.GetComponent<BulletAI>();
            if (lastArrowAI != null)
            {
                // Initialize the bullet's position and rotation directly
                lastArrowAI.InitializeBullet(currentArrowPosition, arrowAngle);
            }

            timer = 0f; // Reset timer for next spawn

            // Start a coroutine to release the arrow after a short delay (animation preparation)
            StartCoroutine(ReleaseArrowAfterDelay(0.4f));
        }
    }

    // Coroutine to handle the arrow release animation and state transition
    IEnumerator ReleaseArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        arrowIsFlying = true; // Set arrow to flying state (will stop tracking player)

        // Deactivate the effect bow
        if (effectBowInstance != null)
        {
            effectBowInstance.SetActive(false);
        }

        // Re-activate the player's original bow (if it exists)
        if (bowInstance != null)
            bowInstance.SetActive(true);
        isBowActive = true;
    }

    // Method to continuously sync the positions of the bow and preparing arrow to their calculated targets
    void SyncBowAndArrowToPlayer()
    {
        if (!arrowIsFlying && playerTransform != null) // Only sync if the arrow isn't flying yet
        {
            if (effectBowInstance != null && effectBowInstance.activeSelf)
                effectBowInstance.transform.position = currentBowPosition; // Sync bow to its position

            if (lastArrowAI != null && lastArrowAI.isActiveAndEnabled)
                lastArrowAI.transform.position = currentArrowPosition; // Sync arrow to its position
        }
    }

    // Method to continuously sync the rotations of the bow and preparing arrow
    void SyncBowAndArrowDirection(float currentArrowAngle)
    {
        if (!arrowIsFlying && effectBowInstance != null && effectBowInstance.activeSelf && lastArrowAI != null)
        {
            // Set effect bow rotation (offset by 180 degrees if needed for sprite orientation)
            effectBowInstance.transform.rotation = Quaternion.Euler(0, 0, currentArrowAngle - 180f);
            // Set arrow rotation
            lastArrowAI.SyncSetRotation(currentArrowAngle);
        }
    }

    // Finds the closest enemy to the given position
    Transform FindClosestEnemy(Vector3 fromPos)
    {
        string[] enemyTags = { "Enemy", "DashEnemy", "LongRangeEnemy", "PotionEnemy" };
        float closestDist = Mathf.Infinity;
        Transform closest = null;
        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(fromPos, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }
}