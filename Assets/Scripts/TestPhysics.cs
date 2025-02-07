using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPhysics : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Transform startingPoint;
    [SerializeField] private Transform you;
    private Rigidbody rigidBody;
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float offsetDistance = 0.75f;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput is not null) Debug.Log("got player input");
        playerInput.onActionTriggered += onActionTriggered;
        rigidBody = you.GetComponent<Rigidbody>();
        if (rigidBody is not null) Debug.Log("got rigidBody ");
    }

    private void onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Boing")
        {
            if (context.action.phase == InputActionPhase.Started)
            {
                FinishTransport(startingPoint);
            }
        }
    }

    private void FinishTransport(Transform portalTo)
    {
        Debug.Log("Finishing the transport");

        you.transform.rotation = portalTo.rotation;
        you.transform.position = portalTo.position + you.transform.forward * offsetDistance;

        //Rigidbody r = you.gameObject.GetComponentInChildren<Rigidbody>();
        //if (r == null) Debug.LogError("No rigidbody found!!!");
        rigidBody.AddForce(you.forward * launchForce, ForceMode.Impulse);
    }
}
