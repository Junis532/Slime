//using UnityEngine;

//public class PlayerSkill : MonoBehaviour
//{
//    public GameObject fireballPrefab;    // ȭ���� ������
//    public Transform firePoint;          // �÷��̾� ���� �߻� ����
//    public FloatingJoystick joystick;    // �ʰ� ���� ���̽�ƽ ������Ʈ ����

//    private void ShootFireball()
//    {
//        if (fireballPrefab == null || firePoint == null)
//        {
//            Debug.LogWarning("Fireball prefab or firePoint not assigned.");
//            return;
//        }

//        GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

//        Vector2 shootDir = lastInputDirection;
//        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
//        fireballObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

//        FireballProjectile fireball = fireballObj.GetComponent<FireballProjectile>();
//        if (fireball != null)
//        {
//            fireball.Init(shootDir);
//        }
//    }

//    private void TeleportPlayer(Vector3 targetPos)
//    {
//        transform.position = targetPos;
//        Debug.Log($"�÷��̾� �ڷ���Ʈ: {targetPos}");
//    }
//}
