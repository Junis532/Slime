using System.Collections.Generic;
using UnityEngine;

public class MeteorOrbitSkill : MonoBehaviour
{
    [Header("���� ������")]
    public GameObject meteorPrefab;

    [Header("���� ����")]
    public int meteorCount = 3;

    [Header("�⺻ �˵� ������")]
    public float baseOrbitRadius = 1f;

    [Header("ȸ�� �ӵ� (��/��)")]
    public float rotationSpeed = 180f;

    private List<GameObject> meteors = new List<GameObject>();
    private float currentAngle = 0f;

    private float currentOrbitRadius; // �⺻ �ݰ�
    private float cachedPlayerScale = 1f; // �ε巴�� ���� �÷��̾� ������
    private float orbitRadiusVelocity = 0f; // SmoothDamp ��

    void Start()
    {
        RefreshMeteor();
        currentOrbitRadius = baseOrbitRadius;
        cachedPlayerScale = Mathf.Max(transform.localScale.x, 1f);
    }

    void Update()
    {
        float targetScale = Mathf.Max(Mathf.Abs(transform.localScale.x), 1f);

        cachedPlayerScale = Mathf.SmoothDamp(cachedPlayerScale, targetScale, ref orbitRadiusVelocity, 0.3f);

        if (Mathf.Abs(cachedPlayerScale - targetScale) < 0.001f)
        {
            cachedPlayerScale = targetScale;
            orbitRadiusVelocity = 0f;
        }

        RotateMeteors();
    }

    void SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
            meteors.Add(meteor);

            MeteorDamage md = meteor.GetComponent<MeteorDamage>();
            if (md != null)
            {
                md.Init();
            }
        }
    }

    void RotateMeteors()
    {
        currentAngle += rotationSpeed * Time.deltaTime;

        float orbitRadius = currentOrbitRadius * (1f + (cachedPlayerScale - 1f) * 0.2f);

        for (int i = 0; i < meteors.Count; i++)
        {
            if (meteors[i] == null) continue;

            float anglePerMeteor = 360f / meteorCount;
            float angle = currentAngle + anglePerMeteor * i;

            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius,
                0f
            );

            Vector3 targetPos = transform.position + offset;

            Vector3 dir = (targetPos - transform.position).normalized;
            float rotationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            meteors[i].transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle - 90f);
            meteors[i].transform.position = targetPos;
        }
    }

    public void RefreshMeteor()
    {
        // ���� ���׿� ����
        foreach (GameObject meteor in meteors)
        {
            if (meteor != null)
            {
                Destroy(meteor);
            }
        }
        meteors.Clear();

        // ���� ����
        SpawnMeteors();
    }

    void OnDestroy()
    {
        foreach (GameObject meteor in meteors)
        {
            if (meteor != null)
            {
                Destroy(meteor);
            }
        }
        meteors.Clear();
    }
}
