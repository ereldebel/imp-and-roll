using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float maxLoadingShotTime;
    private PlayerBrain myBrain;
    private float holdShootTimer = 0;
    private void Awake()
    {
        myBrain = GetComponent<PlayerBrain>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        myBrain.MovementStick = context.ReadValue<Vector2>();
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        myBrain.AimingStick = context.ReadValue<Vector2>();
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            holdShootTimer = 0;
        }
        if (context.canceled)
        {
            myBrain.ShootBall(holdShootTimer < maxLoadingShotTime ? holdShootTimer : maxLoadingShotTime);//If player held the button for "too long" give max instead
            holdShootTimer = 0;
        }

    }

    private void Update()
    {
        holdShootTimer += Time.deltaTime;
    }
}
