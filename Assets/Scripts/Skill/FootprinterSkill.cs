using UnityEngine;

public class FootprinterSkill : MonoBehaviour
{
    public GameObject footprinterPrefab; // Prefab for the footprinter skill
    public float skillDuration = 3f; // Duration for which the footprinter skill is active
    public float fadeSpeed = 0.5f; // Speed at which the footprinter fades out
    public float footprinterInterval = 0.3f; // Interval between footprinter placements

    private float lastFootprinterTime = 0f; // Time when the next footprinter should be placed
    private SpriteRenderer spriteRenderer; // Renderer for the footprinter sprite
    private Color initialColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color; // Store the initial color of the sprite
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
        }
    }

    // Update is called once per frame
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
                Destroy(gameObject); // Destroy the GameObject when the alpha reaches 0
            }
        }
    }

    void CreateFootprint()
    {
        GameObject footprint = Instantiate(footprinterPrefab, transform.position, Quaternion.identity);
        SpriteRenderer footprintRenderer = footprint.GetComponent<SpriteRenderer>();


        PoisonDamage poison = footprint.GetComponent<PoisonDamage>();
        if (poison != null)
        {
            poison.Init(); 
        }


        if (footprintRenderer != null)
        {
            footprintRenderer.color = initialColor;
        }

        Destroy(footprint, skillDuration); // Destroy the footprint after the specified duration
    }
}
