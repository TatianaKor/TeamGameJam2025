using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	#region cached_variables
	
	private static readonly int IsGumDescendingAnimHash = Animator.StringToHash("IsGumDescending");
	private static readonly int IsMovingHorizontallyAnimHash = Animator.StringToHash("IsMovingHorizontally");
	private static readonly int JumpAnimHash = Animator.StringToHash("Jump");
	private static readonly int IsOnWallGumAnimHash = Animator.StringToHash("IsOnWallGum");
	private static readonly int FacingRightAnimHash = Animator.StringToHash("FacingRight");
	private static readonly int TimeSinceJumpAnimHash = Animator.StringToHash("TimeSinceJump");
	private static readonly int IsOnGroundAnimHash = Animator.StringToHash("IsOnGround");
    private static readonly int DieAnimHash = Animator.StringToHash("Die");
    private static readonly int IsGoingUpAnimHash = Animator.StringToHash("IsGoingUp");

	#endregion
	
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
	[SerializeField] private BubbleGum bubblePrefab;
	[SerializeField] private Transform gumCannon;
	[SerializeField] private Transform gumSpawnPoint;
	[SerializeField] private Transform bubbleSpawnPoint;
	[SerializeField] private Camera cam;

	[Header("State")]
	[SerializeField] private TextMeshProUGUI gumCountText;
	[SerializeField] private Button deathScreen;
	public PlayerState playerState;
	
	[Header("Misc")]
	public Transform head;
	[SerializeField] private Transform rightEyeTargetTransform;
	[SerializeField] private Transform leftEyeTargetTransform;
	[SerializeField] private Animator anim;
	[SerializeField] private Transform spriteRoot;
	[SerializeField] private float spriteScaleMultiplier = 0.35f;

	public bool IsTouchingGround { get; private set; }

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
	private float _timeSinceJump;

	private HashSet<GameObject> _holdingWallGums = new();

	void Awake()
	{
		gumCountText.text = playerState.gumCount.ToString();

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

    private void Start()
    {
		deathScreen.onClick.AddListener(GameManager.Instance.RestartLevel);
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
		UpdateTimeSinceJump();
		CheckIsTouchingGround();
		UpdateHorizontalMove();
		UpdateSpitFriction();
		UpdateLook();

		// TODO: WTF?! redo?
		if (_lastVelocityY - rb.linearVelocityY <= DEATH_VELOCITY_Y)
		{
			anim.SetBool(DieAnimHash, true);
			_inputActionMap.Disable();
			deathScreen.gameObject.SetActive(true);
			GameManager.Instance.deathSound.Play();
		}

		_lastVelocityY = rb.linearVelocityY;
		anim.SetBool(IsGoingUpAnimHash, rb.linearVelocityY > 0);
	}

    private void UpdateTimeSinceJump()
	{
		_timeSinceJump += Time.deltaTime;
		anim.SetFloat(TimeSinceJumpAnimHash, _timeSinceJump);
	}

	public void Restart()
    {
		transform.position = _spawnPosition;
		playerState.gumCount = _startGumCount;

		foreach(Transform child in spawnedObjectsRoot)
        {
			Destroy(child.gameObject);
        }
        anim.SetBool(DieAnimHash, false);

		gumCountText.text = playerState.gumCount.ToString();
		_inputActionMap.Enable();
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
		
		anim.SetBool(IsMovingHorizontallyAnimHash, input != 0);
		if (input != 0 && !GameManager.Instance.stepSounds.isPlaying)
		{
			GameManager.Instance.stepSounds.Play();
		}
		else
		{
			GameManager.Instance.stepSounds.Stop();
		}

		switch (input)
		{
			case < 0:
				anim.SetBool(FacingRightAnimHash, true);
				spriteRoot.localScale = new Vector3(-spriteScaleMultiplier, spriteScaleMultiplier, spriteScaleMultiplier);
				break;
			case > 0:
				anim.SetBool(FacingRightAnimHash, false);
				spriteRoot.localScale = new Vector3(spriteScaleMultiplier, spriteScaleMultiplier, spriteScaleMultiplier);
				break;
		}
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
		if (Mathf.Abs(rb.linearVelocityY) < JUMP_ALLOWANCE_VERTICAL_VELOCITY && IsTouchingGround)
		{
			rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetTrigger(JumpAnimHash);
			_timeSinceJump = 0;
		}
		else if (_holdingWallGums.Count > 0 && Mathf.Abs(rb.linearVelocityY) < WALL_JUMP_ALLOWANCE_VERTICAL_VELOCITY)
		{
			rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetTrigger(JumpAnimHash);
			_timeSinceJump = 0;
			
			_holdingWallGums.Clear();
			anim.SetBool(IsOnWallGumAnimHash, false);
		}
	}

	private void CheckIsTouchingGround()
	{
		RaycastHit2D hit =
			Physics2D.CircleCast(transform.position, touchingDownCheckCircleRadius, Vector2.down, JUMP_ALLOWANCE_VERTICAL_DISTANCE, foothold);
		// Debug.DrawRay(transform.position, Vector2.down * (JUMP_ALLOWANCE_VERTICAL_DISTANCE + touchingDownCheckCircleRadius), Color.red, 10);
		// Debug.DrawRay(transform.position + Vector3.right * touchingDownCheckCircleRadius, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
		// Debug.DrawRay(transform.position - Vector3.right * touchingDownCheckCircleRadius, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
		
		bool prev = IsTouchingGround;
		IsTouchingGround = hit.collider != null;
		if (!prev && IsTouchingGround)
		{
			GameManager.Instance.landingSound.Play();
		}
		anim.SetBool(IsOnGroundAnimHash, IsTouchingGround);
	}

	#endregion

	#region throw

	private void ThrowGum(InputAction.CallbackContext context)
	{
		if(IsPointerOverUIObject())
        {
			return;
        }

		if (playerState.gumCount <= 0)
		{
			return;
		}

		--playerState.gumCount;
		gumCountText.text = playerState.gumCount.ToString();

		var spit = Instantiate(gumPrefab, gumSpawnPoint.position, Quaternion.identity, spawnedObjectsRoot);
		spit.AddForce(gumCannon.right * throwGumPower, ForceMode2D.Impulse);
		GameManager.Instance.spitSound.Play();
	}

	private void ThrowBubble(InputAction.CallbackContext context)
	{
		if (playerState.gumCount <= 0)
		{
			return;
		}

		--playerState.gumCount;
		gumCountText.text = playerState.gumCount.ToString();

		var bubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, spawnedObjectsRoot);
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

    public void SetGumDescending(bool isOnGum)
    {
	    rb.bodyType = isOnGum ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
	    anim.SetBool(IsGumDescendingAnimHash, isOnGum);
    }

    #region triggers

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Sticky"))
        {
			_holdingWallGums.Add(col.gameObject);
			anim.SetBool(IsOnWallGumAnimHash, true);
		}
    }

    private void OnTriggerExit2D(Collider2D col)
    {
		if (col.CompareTag("Sticky"))
		{
			_holdingWallGums.Remove(col.gameObject);
			if (_holdingWallGums.Count == 0)
			{
				anim.SetBool(IsOnWallGumAnimHash, false);
			}
		}
	}

	#endregion

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public void AddGum(int addCount)
    {
		playerState.gumCount += addCount;
		gumCountText.text = playerState.gumCount.ToString();
		GameManager.Instance.pickupSound.Play();
	}
}