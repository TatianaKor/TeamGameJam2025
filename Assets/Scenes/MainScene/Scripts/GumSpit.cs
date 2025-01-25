using UnityEngine;

public class GumSpit : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
