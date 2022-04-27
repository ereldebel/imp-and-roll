using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    public Vector2 MovementStick { get; set; }

    public Vector2 AimingStick { get; set; }
    [SerializeField] private float speed;
    private Rigidbody myRigid;

    public bool MovementByPush;
    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (MovementByPush)
        {
            if (MovementStick.sqrMagnitude > 0.1)
            {
                myRigid.AddForce(new Vector3(MovementStick.x*speed, 0, MovementStick.y*speed),ForceMode.Impulse);
            }
        }
        else
        {
            if (MovementStick.sqrMagnitude > 0.1)
            {
                myRigid.velocity = new Vector3(MovementStick.x*speed, 0, MovementStick.y*speed);
            }
            else
            {
                myRigid.velocity = Vector3.zero;
            }
        }
    }
}
