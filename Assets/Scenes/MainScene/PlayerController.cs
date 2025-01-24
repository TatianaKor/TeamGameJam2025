using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float JUMP_ALLOWANCE_VERTICAL_DISTANCE = 1.7f;
    [SerializeField] private float JUMP_ALLOWANCE_VERTICAL_VELOCITY = 0.1f;
    [SerializeField] private float jumpPower = 4;
    [SerializeField] private LayerMask foothold;

    [Header("Movement")]
    [SerializeField] private float movementAcceleration = 2;
    
    [Header("Mandatory")]
    [SerializeField] private Rigidbody2D rb;
    
    private InputActionMap _inputActionMap;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _throwGum;

    void Awake()
    {
        _inputActionMap = InputSystem.actions.FindActionMap("Player");
        _move = _inputActionMap.FindAction("Move");
        _jump = _inputActionMap.FindAction("Jump");
        _jump.performed += Jump;
        _throwGum = _inputActionMap.FindAction("ThrowGum");
    }

    private void OnEnable()
    {
        _inputActionMap.Enable();
    }

    private void OnDisable()
    {
        _inputActionMap.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        var input = _move.ReadValue<float>();
        // rb.AddForceX(input * movementAcceleration * Time.deltaTime);
        rb.linearVelocityX = input * movementAcceleration;
    }

    private bool IsTouchingGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, JUMP_ALLOWANCE_VERTICAL_DISTANCE, foothold);
        // Debug.DrawRay(transform.position, Vector2.down * JUMP_ALLOWANCE_VERTICAL_DISTANCE, Color.red, 10);
        return hit.collider != null;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (Mathf.Abs(rb.linearVelocityY) < JUMP_ALLOWANCE_VERTICAL_VELOCITY && IsTouchingGround())
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
