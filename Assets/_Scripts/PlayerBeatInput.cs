using UnityEngine;


public class PlayerBeatInput : MonoBehaviour
{
    [SerializeField] private PlayerBeatController playerBeatController;
    
    private void Start()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager.Instance is null; skipping input processing.");
            return;
        }

        playerBeatController = GetComponent<PlayerBeatController>();
        //InputManager.Instance.JumpPerformed += JumpPerformed;
        //InputManager.Instance.FirePerformed += FirePerformed;
        TestInputManager.Instance.FirePerformed += FirePerformed;
        TestInputManager.Instance.JumpPerformed += JumpPerformed;
    }

    private void FirePerformed()
    {
        playerBeatController.Attack();
    }

    private void JumpPerformed()
    {
        playerBeatController.Jump();
    }

    private void Update()
    {
        var playerInput = 
            Vector2.right * InputManager.Instance.GetHorizontalMovement() + 
            Vector2.up * InputManager.Instance.GetVerticalMovement();
        
        playerBeatController.MoveAction(playerInput.normalized);
    }
}