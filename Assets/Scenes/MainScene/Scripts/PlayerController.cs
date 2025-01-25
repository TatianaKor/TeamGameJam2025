using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Jump")] [SerializeField] private float JUMP_ALLOWANCE_VERTICAL_DISTANCE = 1.7f;
	[SerializeField] private float DEATH_VELOCITY_Y = -10f;
	[SerializeField] private float JUMP_ALLOWANCE_VERTICAL_VELOCITY = 0.1f;
	[SerializeField] private float WALL_JUMP_ALLOWANCE_VERTICAL_VELOCITY = 1f;
	[SerializeField] private float GUM_FRICTION = 0.5f;
	[SerializeField] private float jumpPower = 4;
	[SerializeField] private LayerMask foothold;
	[SerializeField] private float touchingDownCheckCircleRadius = 0.35f;

	[Header("Movement")] [SerializeField] private float movementAcceleration = 2;
	[SerializeField] private Rigidbody2D rb;

	[Header("Throw Gum")] [SerializeField] private float throwGumPower = 1.5f;
	[SerializeField] private Transform spawnedObjectsRoot;
	[SerializeField] private Rigidbody2D gumPrefab;
	[SerializeField] private Rigidbody2D bubblePrefab;
	[SerializeField] private Transform gumCannon;
	[SerializeField] private Transform gumSpawnPoint;
	[SerializeField] private Transform bubbleSpawnPoint;
	[SerializeField] private Camera cam;

	[Header("State")]
	public PlayerState playerState;
	
	[Header("Misc")]
	public Transform head;
	[SerializeField] private Transform rightEyeTargetTransform;
	[SerializeField] private Transform leftEyeTargetTransform;

	private InputActionMap _inputActionMap;
	private InputAction _move;
	private InputAction _jump;
	private InputAction _throwGum;
	private InputAction _throwBubble;
	private InputAction _look;

	private Vector2 _spawnPosition;
	private Vector2 _lastLook;
	private float _lastVelocityY = 0;
	private int _startGumCount;

	private HashSet<GameObject> _holdingWallGums = new HashSet<GameObject>();

	void Awake()
	{
		_spawnPosition = transform.position;
		_startGumCount = playerState.gumCount;

		_inputActionMap = InputSystem.actions.FindActionMap("Player");

		_move = _inputActionMap.FindAction("Move");

		_jump = _inputActionMap.FindAction("Jump");
		_jump.performed += Jump;

		_throwGum = _inputActionMap.FindAction("Throw Gum");
		_throwGum.performed += ThrowGum;

		_throwBubble = _inputActionMap.FindAction("Throw Bubble");
		_throwBubble.performed += ThrowBubble;

		_look = _inputActionMap.FindAction("Look");
	}

	#region input_activation

	private void OnEnable()
	{
		_inputActionMap.Enable();
	}

	private void OnDisable()
	{
		_inputActionMap.Disable();
	}

	#endregion


	void Update()
	{
		UpdateHorizontalMove();
		UpdateSpitFriction();
		UpdateLook();

		if(_lastVelocityY - rb.linearVelocityY <= DEATH_VELOCITY_Y)
        {
			//TODO: death animation
			GameManager.Instance.RestartLevel();
		}
		_lastVelocityY = rb.linearVelocityY;
	}

	public void Restart()
    {
		transform.position = _spawnPosition;
		playerState.gumCount = _startGumCount;

		foreach(Transform child in spawnedObjectsRoot)
        {
			Destroy(child.gameObject);
        }
	}

	#region movement

	private void UpdateHorizontalMove()
	{
		if (rb.bodyType == RigidbodyType2D.Static)
		{
			return;
		}
		var input = _move.ReadValue<float>();
		// rb.AddForceX(input * movementAcceleration * Time.deltaTime);
		rb.linearVelocityX = input * movementAcceleration;
	}

	private void UpdateSpitFriction()
	{
		if (_holdingWallGums.Count > 0)
		{
			rb.linearVelocityY *= GUM_FRICTION;
		}
	}


	private void Jump(InputAction.CallbackContext context)
	{
		//Debug.Log("Vertical speed: " + Mathf.Abs(rb.linearVelocityY));

		if (Mathf.Abs(rb.linearVelocityY) < JUMP_ALLOWANCE_VERTICAL_VELOCITY && IsTouchingGround())
		{
			rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
		}
		else if (_holdingWallGums.Count > 0 && Mathf.Abs(rb.linearVelocityY) < WALL_JUMP_ALLOWANCE_VERTICAL_VELOCITY)
		{
			rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			_holdingWallGums.Clear();
		}
	}

	public bool IsTouchingGround()
	{
		RaycastHit2D hit =
			Physics2D.CircleCast(transform.position, touchingDownCheckCircleRadius, Vector2.down, JUMP_ALLOWANCE_VERTICAL_DISTANCE, foothold);
		// Debug.DrawRay(transform.position, Vector2.down * (JUMP_ALLOWANCE_VERTICAL_DISTANCE + touchingDownCheckCircleRadius), Color.red, 10);
		// Debug.DrawRay(transform.position + Vector3.right * touchingDownCheckCircleRadius, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
		// Debug.DrawRay(transform.position - Vector3.right * touchingDownCheckCircleRadius, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
		return hit.collider != null;
	}

	#endregion

	#region throw

	private void ThrowGum(InputAction.CallbackContext context)
	{
		if (playerState.gumCount <= 0)
		{
			return;
		}

		--playerState.gumCount;

		var spit = Instantiate(gumPrefab, gumSpawnPoint.position, Quaternion.identity, spawnedObjectsRoot);
		spit.AddForce(gumCannon.right * throwGumPower, ForceMode2D.Impulse);
	}

	private void ThrowBubble(InputAction.CallbackContext context)
	{
		if (playerState.gumCount <= 0)
		{
			return;
		}

		--playerState.gumCount;

		Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, spawnedObjectsRoot);
	}

	private void UpdateLook()
	{
		var input = _look.ReadValue<Vector2>();
		if (_lastLook == input)
		{
			return;
		}

		_lastLook = input;
		Vector2 worldPoint = cam.ScreenToWorldPoint(input);

		rightEyeTargetTransform.position = worldPoint;
		leftEyeTargetTransform.position = worldPoint;

		gumCannon.rotation = Quaternion.Euler(0, 0,
			Mathf.Atan2(worldPoint.y - gumCannon.position.y, worldPoint.x - gumCannon.position.x) * Mathf.Rad2Deg);
	}

    #endregion

    #region triggers

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag.Equals("Sticky"))
        {
			_holdingWallGums.Add(collider.gameObject);
		}
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
		if (collider.gameObject.tag.Equals("Sticky"))
		{
			_holdingWallGums.Remove(collider.gameObject);
		}
	}

    #endregion
}