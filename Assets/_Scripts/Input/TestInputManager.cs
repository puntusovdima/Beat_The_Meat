using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputManager : MonoBehaviour
{
    public static TestInputManager Instance { get; private set; }
    private PlayerInputActions _playerInputActions;
    

    public Action FirePerformed, JumpPerformed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
    }
    
    private void Start()
    {
        _playerInputActions.Player.Fire.performed += FireOnPerformed;
        _playerInputActions.Player.Jump.performed += JumpOnPerformed;
    }
    
    private void FireOnPerformed(InputAction.CallbackContext obj)
    {
        FirePerformed?.Invoke();
    }
    
    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        JumpPerformed?.Invoke();
    }
    
}
