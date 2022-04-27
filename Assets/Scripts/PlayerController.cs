using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private PlayerBrain myBrain;
    private void Awake()
    {
        myBrain = GetComponent<PlayerBrain>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        print(context.ReadValue<Vector2>());
        myBrain.MovementStick = context.ReadValue<Vector2>();
    }
}
