using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActionMap _inputActionMap;
    private InputAction move;
    private InputAction throwGum;
    
    void Awake()
    {
        _inputActionMap = InputSystem.actions.FindActionMap("Player");
        move = _inputActionMap.FindAction("Move");
        throwGum = _inputActionMap.FindAction("ThrowGum");
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
        
    }
}
