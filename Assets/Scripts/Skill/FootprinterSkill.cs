using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootprinterSkill : MonoBehaviour
{
    public GameObject footprinterPrefab;
    public GameObject poisonEffectPrefab; // ☠️ 이펙트 프리팹 연결
    public float skillDuration = 3f;
    public float fadeSpeed = 0.5f;
    public float footprinterInterval = 0.3f;

    private float lastFootprinterTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private bool isPoisonGasActive = false;

    // 🟡 이 오브젝트가 발자국인지 판단
    public bool isFootprint = false;

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
            initialColor = spriteRenderer.color;
    }

    void Update()
    {
        if (!isFootprint)
        {
            // ▶ 플레이어일 경우: 발자국 생성만
            if (Time.time - lastFootprinterTime >= footprinterInterval)
            {
                CreateFootprint();
                lastFootprinterTime = Time.time;
            }
        }
        else
        {
            // ▶ 발자국일 경우: 점점 사라지게 하고 제거
            if (spriteRenderer != null)
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
    }

    void CreateFootprint()
    {
        GameObject footprint = Instantiate(footprinterPrefab, transform.position, Quaternion.identity);

        // 👣 발자국으로 지정
        FootprinterSkill footprintScript = footprint.GetComponent<FootprinterSkill>();
        if (footprintScript != null)
        {
            footprintScript.isFootprint = true;
        }

        footprintList.Add(footprint);

        SpriteRenderer footprintRenderer = footprint.GetComponent<SpriteRenderer>();
        PoisonDamage poison = footprint.GetComponent<PoisonDamage>();
        if (poison != null) poison.Init();
        if (footprintRenderer != null) footprintRenderer.color = initialColor;

        if (isPoisonGasActive)
        {
            // ☠️ 콜라이더 활성화
            Collider2D col = footprint.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            // ☠️ 이펙트 생성
            if (poisonEffectPrefab != null)
            {
                GameObject effect = Instantiate(poisonEffectPrefab, footprint.transform);
                effect.transform.localPosition = Vector3.zero;
                StartCoroutine(DestroyEffectAfterDelay(effect, 10f));
            }

            StartCoroutine(DisablePoisonEffect(footprint, 10f));
        }

        StartCoroutine(DestroyFootprintAfterDelay(footprint, skillDuration));
    }

    IEnumerator DestroyFootprintAfterDelay(GameObject footprint, float delay)
    {
        yield return new WaitForSeconds(delay);
        footprintList.Remove(footprint);
        Destroy(footprint);
    }

    IEnumerator DisablePoisonEffect(GameObject footprint, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (footprint != null && footprint.TryGetComponent(out Collider2D col))
        {
            col.enabled = false;
        }
    }

    IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null) Destroy(effect);
    }

    public void ActivatePoisonGasMode(float duration)
    {
        StartCoroutine(PoisonGasRoutine(duration));
    }

    IEnumerator PoisonGasRoutine(float duration)
    {
        isPoisonGasActive = true;
        yield return new WaitForSeconds(duration);
        isPoisonGasActive = false;
    }
}
