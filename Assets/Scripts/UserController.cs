using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserController : MonoBehaviour
{
    public float moveSpeed = 0;
    public float panSpeed = 0;

    private Rigidbody rb;
    private GameObject user;
    private ObjectPlacementManager placementManager;

    private float movementX;
    private float movementY;
    private float movementZ;
    private float rotationXAxis;
    private float rotationYAxis;
    private float translationSpeed;
    private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        user = GameObject.FindGameObjectWithTag("User");
        translationSpeed = moveSpeed * Time.deltaTime;
        rotationSpeed = panSpeed * Time.deltaTime;

        placementManager = GetComponent<ObjectPlacementManager>();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(movementX, movementY, movementZ);
        user.transform.Translate(movement * translationSpeed, Space.World);

        Vector3 rotateValue = new Vector3(rotationYAxis, rotationXAxis * -1, 0);
        transform.eulerAngles = transform.eulerAngles - (rotateValue * rotationSpeed);
    }

    private void OnMove(InputValue movementValue)
    {
        // Debug.Log(movementValue);
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementZ = movementVector.y;
    }

    private void OnLook(InputValue movementValue)
    {
        // Debug.Log("OnLook");
        // Debug.Log(movementValue);
        Vector2 movementVector = movementValue.Get<Vector2>();
        rotationXAxis = movementVector.x;
        rotationYAxis = movementVector.y;
    }

    private void OnElevate(InputValue movementValue)
    {
        movementY = movementValue.Get<float>();
    }

    private void OnFire()
    {
        placementManager.placeObject();
        Debug.Log("Fired");
    }

}
