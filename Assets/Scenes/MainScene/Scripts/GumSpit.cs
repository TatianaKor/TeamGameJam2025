using UnityEngine;

public class GumSpit : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private float autoDestroyTime = 10f;

    private float _lifeTimer = 0f;

    void Awake()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= autoDestroyTime)
        {
            Destroy(gameObject);
        }
    }
}
