using System;
using UnityEngine;

public class HorizontalSpitDescender : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private float descendSpeed;
	[SerializeField] private float verticalShift;

	private Rigidbody2D _playerRb;
	
	public PlayerController player;
	public Vector2 topContactPoint;

	private void Start()
	{
		_playerRb = player.GetComponent<Rigidbody2D>();
		_playerRb.bodyType = RigidbodyType2D.Static;
	}
	
	private void OnDestroy()
	{
		_playerRb.bodyType = RigidbodyType2D.Dynamic;
	}

	void Update()
	{
		player.transform.position += Vector3.down * (Time.deltaTime * descendSpeed);
		var height = topContactPoint.y - player.head.position.y;
		spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
		transform.position = new Vector3(transform.position.x, topContactPoint.y - height / 2 + verticalShift, 0);
	}

	private void FixedUpdate()
	{
		if (player.IsTouchingGround())
		{
			Destroy(gameObject);
		}
	}
}
