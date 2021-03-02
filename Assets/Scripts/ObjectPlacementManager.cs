using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacementManager : MonoBehaviour
{
    // Public
    public GameObject placementIndicator;
    public GameObject objectToPlace;

    // Private Obj
    private Camera mainCamera;
    private Vector3 placementIndicatorPosition;
    private Quaternion placementIndicatorRotation;

    private bool isObjectPlaced = false;

    // Ray Cast Attr
    private Vector3[] surroundingPoints;
    private float raycastBorder = 0.2F;
    private Vector3 _raycastCenter = new Vector3(0.5F, 0.5F, 0);
    private Ray _centerRay;
    private RaycastHit _centerHit;
    private RaycastHit _rcHit;
    private Ray _rcRay;
    private int _rcOnPlainCnt;


    void Start()
    {   
        mainCamera = Camera.main;
        mainCamera.enabled = true;
        surroundingPoints = new [] {
            new Vector3(raycastBorder, -raycastBorder, 0) + _raycastCenter, 
            new Vector3(raycastBorder, 0, 0) + _raycastCenter, 
            new Vector3(raycastBorder, raycastBorder, 0) + _raycastCenter,
            new Vector3(0, -raycastBorder, 0) + _raycastCenter, 
            new Vector3(0, raycastBorder, 0) + _raycastCenter,
            new Vector3(-raycastBorder, -raycastBorder, 0) + _raycastCenter, 
            new Vector3(-raycastBorder, 0, 0) + _raycastCenter, 
            new Vector3(-raycastBorder, raycastBorder, 0) + _raycastCenter
        };
    }

    // Update is called once per frame
    void Update()
    {
        _centerRay = mainCamera.ViewportPointToRay(_raycastCenter);
        if (Physics.Raycast(_centerRay, out _centerHit) && isPlain(_centerHit))
        {
            var cameraForward = mainCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, -90, cameraForward.z).normalized;

            placementIndicatorPosition = new Vector3(_centerHit.point.x, _centerHit.point.y + 0.1f, _centerHit.point.z);
            placementIndicatorRotation = Quaternion.LookRotation(cameraBearing);
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementIndicatorPosition, placementIndicatorRotation);
        }
        else
        {
            placementIndicator.SetActive(false);
            print("I'm not looking at a plane!");
        }
            
    }

    public void placeObject()
    {
        if (!isObjectPlaced)
        {
            Vector3 v = new Vector3(placementIndicatorPosition.x, placementIndicatorPosition.y + 0.4F, placementIndicatorPosition.z);
            Instantiate(objectToPlace, v, placementIndicatorRotation);
            isObjectPlaced = true;
        }
    }

    private bool isPlain(RaycastHit hit)
    {
        _rcOnPlainCnt = 0;
        foreach(Vector3 v in surroundingPoints)
        {
            _rcRay = mainCamera.ViewportPointToRay(v);
            if (Physics.Raycast(_rcRay, out _rcHit))
            {
                if (Mathf.Abs(_rcHit.point.y - hit.point.y) < 0.001)
                {
                    ++_rcOnPlainCnt;
                }
            }
        }

        if (_rcOnPlainCnt >= 4)
        {
            return true;
        }
        
        return false;
    }
}
