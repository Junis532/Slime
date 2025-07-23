//using System.Collections;
//using UnityEngine;
//using DG.Tweening; // DOTween�� ����Ѵٸ� �߰�

//public class SpecialDashProjectile : MonoBehaviour
//{
//    private Vector2 initialVelocity;
//    private Vector2 targetPosition;

//    // ���� DamageArea ��� PotionEnemyDamage �������� �޽��ϴ�.
//    private GameObject potionDamagePrefab;
//    private float potionLifetime; // ����(�� ����)�� ���� �ð�

//    private float startTime;

//    void OnEnable()
//    {
//        // Ǯ���� ��Ȱ��� �� �ʱ�ȭ�ǹǷ� ���⼭ startTime�� �ʱ�ȭ�� �ʿ�� �����ϴ�.
//    }

//    /// <summary>
//    /// DashProjectile�� �ʱ�ȭ�մϴ�.
//    /// </summary>
//    /// <param name="velocity">����ü�� �ʱ� �ӵ� (���� * �ӵ�).</param>
//    /// <param name="targetPos">����ü�� ���߰� �� ������ ������ ���� ��ǥ ��ġ.</param>
//    /// <param name="potionPrefab">������ PotionEnemyDamage ������.</param>
//    /// <param name="lifetime">������ �� ������ ���� �ð�.</param>
//    public void Init(Vector2 velocity, Vector2 targetPos, GameObject potionPrefab, float lifetime)
//    {
//        initialVelocity = velocity;
//        targetPosition = targetPos;
//        potionDamagePrefab = potionPrefab;
//        potionLifetime = lifetime; // �� ���� ���� �ð� ����

//        startTime = Time.time; // ����ü �̵� Ÿ�̸� ����
//    }

//    void Update()
//    {
//        float totalMoveDistance = Vector2.Distance(transform.position, targetPosition);
//        float totalMoveTime = (initialVelocity.magnitude > 0) ? totalMoveDistance / initialVelocity.magnitude : 0f;

//        float timeElapsed = Time.time - startTime;
//        float fractionOfJourney = (totalMoveTime > 0) ? timeElapsed / totalMoveTime : 1f;

//        if (fractionOfJourney < 1.0f)
//        {
//            transform.position = Vector2.Lerp(transform.position, targetPosition, fractionOfJourney);
//        }
//        else // ��ǥ ������ �����߰ų� �������� ��
//        {
//            transform.position = targetPosition; // ��Ȯ�� ��ǥ ������ ��ġ
//            SpawnPotionDamageAndDeactivate(); // PotionEnemyDamage ���� �Լ� ȣ��
//        }
//    }

//    private void SpawnPotionDamageAndDeactivate()
//    {
//        if (potionDamagePrefab != null)
//        {
//            // Ǯ���� PotionEnemyDamage ������Ʈ ����
//            GameObject poisonArea = GameManager.Instance.poolManager.SpawnFromPool(
//                potionDamagePrefab.name, targetPosition, Quaternion.identity);

//            if (poisonArea != null)
//            {
//                // PotionBehavior ��ũ��Ʈ�� ã�� Init �޼ҵ带 ȣ��
//                PotionBehavior potionBehavior = poisonArea.GetComponent<PotionBehavior>();
//                if (potionBehavior != null)
//                {
//                    potionBehavior.StartLifetime(potionLifetime); // �� ���� ���� �ð� ����
//                }
//                else
//                {
//                    Debug.LogWarning("������ PotionDamage ������Ʈ�� PotionBehavior ��ũ��Ʈ�� ã�� �� �����ϴ�! �� ������ ������� ���� �� �ֽ��ϴ�.");
//                }

//                // PotionEnemyDamage ��ũ��Ʈ �ʱ�ȭ (������ ����� ����)
//                PotionEnemyDamage potionDamage = poisonArea.GetComponent<PotionEnemyDamage>();
//                if (potionDamage != null)
//                {
//                    potionDamage.Init(); // PotionEnemyDamage�� Init ȣ�� (playerStats ��� ������ ���)
//                }
//                else
//                {
//                    Debug.LogWarning("������ PotionDamage ������Ʈ�� PotionEnemyDamage ��ũ��Ʈ�� ã�� �� �����ϴ�! �� �������� ����� �۵����� ���� �� �ֽ��ϴ�.");
//                    GameManager.Instance.poolManager.ReturnToPool(poisonArea); // ��ũ��Ʈ ������ �ٷ� Ǯ�� ��ȯ
//                }
//            }
//            else
//            {
//                Debug.LogError($"'{potionDamagePrefab.name}' �������� Ǯ���� �������� ���߽��ϴ�. PoolManager�� ��ϵǾ�����, �̸��� ��Ȯ���� Ȯ���ϼ���.");
//            }
//        }
//        else
//        {
//            Debug.LogWarning("Potion Damage Prefab�� �������� �ʾ� �� ������ ������ ������ �� �����ϴ�.");
//        }

//        // �ڽ��� Ǯ�� ��ȯ
//        if (GameManager.Instance != null && GameManager.Instance.poolManager != null)
//        {
//            GameManager.Instance.poolManager.ReturnToPool(this.gameObject);
//        }
//        else
//        {
//            Destroy(this.gameObject);
//        }
//    }
//}