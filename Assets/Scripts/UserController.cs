using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserController : MonoBehaviour
{
    public float moveSpeed = 0;
    public float panSpeed = 0;

    private GameObject user;
    private ObjectPlacementManager placementManager;

    private Vector2 movementVector;
    private float movementX;
    private float movementY;
    private float movementZ;
    private float movementRad;
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
        if (Application.isEditor)
        {
            Vector3 rotateValue = new Vector3(rotationYAxis, rotationXAxis * -1, 0);
            Debug.Log(rotateValue);
            Debug.Log(transform.rotation.eulerAngles);
            transform.eulerAngles = transform.eulerAngles - (rotateValue * rotationSpeed);

            Vector3 movement = new Vector3(movementX, movementY, movementZ);
            user.transform.Translate(movement * translationSpeed, Space.World);
        }
    }

    private void OnMove(InputValue movementValue)
    {
        if (Application.isEditor)
        {
            movementVector = movementValue.Get<Vector2>();
            if (movementVector.x == 0 && movementVector.y == 0)
            {
                movementX = 0;
                movementZ = 0;
                return;
            }

            movementRad = Mathf.Atan2(movementVector.x, movementVector.y) + transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            movementX = Mathf.Sin(movementRad);
            movementZ = Mathf.Cos(movementRad);
        }
    }

    private void OnLook(InputValue movementValue)
    {
        if (Application.isEditor)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();
            rotationXAxis = movementVector.x;
            rotationYAxis = movementVector.y;
        }
    }

    private void OnElevate(InputValue movementValue)
    {
        if (Application.isEditor)
        {
            movementY = movementValue.Get<float>();
        }
    }

    private void OnFire()
    {
        placementManager.placeObject();
        Debug.Log("Fired");
    }

}
