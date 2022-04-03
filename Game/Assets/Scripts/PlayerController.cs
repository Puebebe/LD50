using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody leftSki, rightSki;
    [SerializeField]
    private int force = 10;

    private GameActions controls;

    private void Awake()
    {
        controls = new GameActions();
        controls.Enable();
        if (!rightSki)
            rightSki = leftSki;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        var leftInput = controls.Player.MoveLeft.ReadValue<Vector2>();
        var rightInput = controls.Player.MoveRight.ReadValue<Vector2>();
        var leftInputForce = new Vector3(leftInput.x * force, 0, leftInput.y * force);
        var rightInputForce = new Vector3(rightInput.x * force, 0, rightInput.y * force);

        // Player should not "slide up"
        if (leftSki.velocity.z <= 0)
        {
            leftSki.velocity = new Vector3(leftSki.velocity.x, leftSki.velocity.y, 0);
            leftInputForce.z = 0;
        }

        if (rightSki.velocity.z <= 0)
        {
            rightSki.velocity = new Vector3(rightSki.velocity.x, rightSki.velocity.y, 0);
            rightInputForce.z = 0;
        }

        leftSki.AddRelativeForce(leftInputForce);
        rightSki.AddRelativeForce(rightInputForce);
        
        leftSki.AddRelativeTorque(new Vector3(0, leftInput.x / 10, 0), ForceMode.VelocityChange);
        rightSki.AddRelativeTorque(new Vector3(0, rightInput.x / 10, 0), ForceMode.VelocityChange);

        //Debug.Log(leftSki.rotation.eulerAngles);
    }
}
