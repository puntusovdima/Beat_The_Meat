using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputManager : MonoBehaviour
{
    public static TestInputManager Instance { get; private set; }
    private PlayerInputActions _playerInputActions;
    

    public Action FirePerformed, JumpPerformed, PausePerformed, UnPausePerformed;

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
        _playerInputActions.Player.Pause.performed += PauseOnPerformed;
        _playerInputActions.UI.UnPause.performed += UnPauseOnPerformed;
    }
    
    private void FireOnPerformed(InputAction.CallbackContext obj)
    {
        FirePerformed?.Invoke();
    }
    
    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        JumpPerformed?.Invoke();
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        PausePerformed?.Invoke();
        SwitchPlayerToUI();
    }

    private void UnPauseOnPerformed(InputAction.CallbackContext obj)
    {
        UnPausePerformed?.Invoke();
        SwitchUIToPlayer();
    }

    public void SwitchUIToPlayer()
    {
        Debug.Log("SWITCHED FROM UI TO PLAYER TESTINPUTACTIONS");
        _playerInputActions.UI.Disable();
        _playerInputActions.Player.Enable();
    }

    private void SwitchPlayerToUI()
    {
        Debug.Log("SWITCHED FROM PLAYER TO UI TESTINPUTACTIONS");
        _playerInputActions.Player.Disable();
        _playerInputActions.UI.Enable();
    }

}
