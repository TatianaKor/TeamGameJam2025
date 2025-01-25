using System;
using UnityEngine;

public class FlyingGumSpit : MonoBehaviour
{
    [SerializeField] private LayerMask footholdLayer;
    [SerializeField] private GameObject footholdSpit;
    
    [SerializeField] private LayerMask horizontalPlatformLayer;
    [SerializeField] private GameObject horizontalPlatformSpit;
    
    [SerializeField] private LayerMask verticalPlatformLayer;
    [SerializeField] private GameObject verticalPlatformSpit;

    [SerializeField] private Rigidbody2D rb;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var layer = 1 << collision.gameObject.layer;
        if ((layer & footholdLayer.value) != 0)
        {
            Attach(collision, footholdSpit);
        }
        else if ((layer & horizontalPlatformLayer.value) != 0)
        {
            Attach(collision, horizontalPlatformSpit);
        }
        else if ((layer & verticalPlatformLayer.value) != 0)
        {
            Attach(collision, verticalPlatformSpit, transform.position.x > collision.gameObject.transform.position.x);
        }
    }

    private void Attach(Collision2D collision, GameObject prefab, bool flip = false)
    {
        var contactPoint = collision.GetContact(0).point;
        Instantiate(prefab, contactPoint, flip ? Quaternion.Euler(0, 0, 180) : Quaternion.identity);
        Destroy(gameObject);
    }

    void Update()
    {
        transform.up = rb.linearVelocity;
    }
}
