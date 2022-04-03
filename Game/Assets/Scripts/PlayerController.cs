using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameActions controls;
    private Rigidbody rb;
    private int force = 10;

    private void Awake()
    {
        controls = new GameActions();
        controls.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        var input = controls.Player.Move.ReadValue<Vector2>();
        var inputForce = new Vector3(input.x * force, 0, input.y * force);

        // Player should not "slide up"
        if (rb.velocity.z <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            inputForce.z = 0;
        }

        rb.AddForce(inputForce);
    }
}
