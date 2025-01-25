using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Jump")] [SerializeField] private float JUMP_ALLOWANCE_VERTICAL_DISTANCE = 1.7f;
	[SerializeField] private float JUMP_ALLOWANCE_VERTICAL_VELOCITY = 0.1f;
	[SerializeField] private float jumpPower = 4;
	[SerializeField] private LayerMask foothold;

	[Header("Movement")] [SerializeField] private float movementAcceleration = 2;
	[SerializeField] private Rigidbody2D rb;

	[Header("Throw Gum")] [SerializeField] private float throwGumPower = 1.5f;
	[SerializeField] private Rigidbody2D gumPrefab;
	[SerializeField] private Rigidbody2D bubblePrefab;
	[SerializeField] private Transform gumCannon;
	[SerializeField] private Transform gumSpawnPoint;
	[SerializeField] private Camera cam;


	public PlayerState playerState;

	private InputActionMap _inputActionMap;
	private InputAction _move;
	private InputAction _jump;
	private InputAction _throwGum;
	private InputAction _throwBubble;
	private InputAction _look;

	private Vector2 _lastLook;

	void Awake()
	{
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
		UpdateLook();
	}

	#region movement

	private void UpdateHorizontalMove()
	{
		var input = _move.ReadValue<float>();
		// rb.AddForceX(input * movementAcceleration * Time.deltaTime);
		rb.linearVelocityX = input * movementAcceleration;
	}


	private void Jump(InputAction.CallbackContext context)
	{
		if (Mathf.Abs(rb.linearVelocityY) < JUMP_ALLOWANCE_VERTICAL_VELOCITY && IsTouchingGround())
		{
			rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
		}
	}

	private bool IsTouchingGround()
	{
		RaycastHit2D hit =
			Physics2D.Raycast(transform.position, Vector2.down, JUMP_ALLOWANCE_VERTICAL_DISTANCE, foothold);
		// Debug.DrawRay(transform.position, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
		return hit.collider != null;
	}

	#endregion

	#region throw

	private void ThrowGum(InputAction.CallbackContext context)
	{
		if (playerState.gumNumber <= 0)
		{
			return;
		}

		--playerState.gumNumber;

		var spit = Instantiate(gumPrefab, gumSpawnPoint.position, Quaternion.identity);
		spit.AddForce(gumCannon.right * throwGumPower, ForceMode2D.Impulse);
	}

	private void ThrowBubble(InputAction.CallbackContext context)
	{
		if (playerState.gumNumber <= 0)
		{
			return;
		}

		--playerState.gumNumber;

		var spit = Instantiate(bubblePrefab, gumSpawnPoint.position, Quaternion.identity);
		spit.AddForce(gumCannon.right * throwGumPower, ForceMode2D.Impulse);
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

		gumCannon.rotation = Quaternion.Euler(0, 0,
			Mathf.Atan2(worldPoint.y - gumCannon.position.y, worldPoint.x - gumCannon.position.x) * Mathf.Rad2Deg);
	}

	#endregion
}