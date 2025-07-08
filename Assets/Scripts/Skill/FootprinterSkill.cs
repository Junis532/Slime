using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootprinterSkill : MonoBehaviour
{
    public GameObject footprinterPrefab;
    public float skillDuration = 3f;
    public float fadeSpeed = 0.5f;
    public float footprinterInterval = 0.3f;

    private float lastFootprinterTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    // 발자국 리스트
    private static List<GameObject> footprintList = new List<GameObject>();

    // 외부 접근용: 가장 오래된 발자국 위치
    public static Vector3 OldestFootprintPosition
    {
        get
        {
            if (footprintList.Count > 0 && footprintList[0] != null)
                return footprintList[0].transform.position;
            return Vector3.zero;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
        }
    }

    void Update()
    {
        if (Time.time - lastFootprinterTime >= footprinterInterval)
        {
            CreateFootprint();
            lastFootprinterTime = Time.time;
        }

        if (spriteRenderer == null)
        {
            if (spriteRenderer.color.a > 0)
            {
                float alpha = spriteRenderer.color.a - fadeSpeed * Time.deltaTime;
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Max(alpha, 0f));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void CreateFootprint()
    {
        GameObject footprint = Instantiate(footprinterPrefab, transform.position, Quaternion.identity);

        // 리스트에 추가
        footprintList.Add(footprint);

        SpriteRenderer footprintRenderer = footprint.GetComponent<SpriteRenderer>();
        PoisonDamage poison = footprint.GetComponent<PoisonDamage>();
        if (poison != null) poison.Init();
        if (footprintRenderer != null) footprintRenderer.color = initialColor;

        StartCoroutine(DestroyFootprintAfterDelay(footprint, skillDuration));
    }

    IEnumerator DestroyFootprintAfterDelay(GameObject footprint, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 리스트에서 제거
        footprintList.Remove(footprint);

        Destroy(footprint);
    }
}
