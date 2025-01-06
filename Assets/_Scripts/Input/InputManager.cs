using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private TestInputActions _inputActions;
    private bool isPaused = false;

    //Aquí ponemos todas las acciones puntuales que vaya a usar nuestra aplicación
    public Action JumpPerformed,
        FirePerformed,
        PausePerformed,
        UnPausePerformed;

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
        
        _inputActions = new TestInputActions();
        _inputActions.Player.Enable();
    }

    private void Start()
    {
        _inputActions.Player.Jump.performed += JumpOnPerformed;
        _inputActions.Player.Fire.performed += FireOnPerformed;
        _inputActions.Player.Pause.performed += PauseOnPerformed;
        _inputActions.UI.UnPause.performed += UnPauseOnPerformed;
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {

        isPaused = true;
        PausePerformed?.Invoke();
        SwitchPlayerToUI();
        
    }

    private void UnPauseOnPerformed(InputAction.CallbackContext obj)
    {
        
        
        UnPausePerformed?.Invoke();
        isPaused = false;
        SwitchUIToPlayer();
        

    }

    private void FireOnPerformed(InputAction.CallbackContext obj)
    {
        FirePerformed?.Invoke();
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        JumpPerformed?.Invoke();
    }

    public float GetHorizontalMovement()
    {
        return _inputActions.Player.HorizontalMove.ReadValue<float>();
    }
    
    public float GetVerticalMovement()
    {
        return _inputActions.Player.VerticalMove.ReadValue<float>();
    }

    public void SwitchUIToPlayer()
    {
        Debug.Log("SWITCHED FROM UI TO PLAYER");
        _inputActions.UI.Disable();
        _inputActions.Player.Enable();
    }

    private void SwitchPlayerToUI()
    {
        Debug.Log("SWITCHED FROM PLAYER TO UI");
        _inputActions.Player.Disable();
        _inputActions.UI.Enable();
    }
}
