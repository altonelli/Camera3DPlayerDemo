using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Experimental.XR;

public class ObjectPlacementManager : MonoBehaviour
{
    // Public
    public GameObject placementIndicator;
    public GameObject objectToPlace;

    // Private Obj
    private Camera _camera;
    private Vector3 placementIndicatorPosition;
    private Quaternion placementIndicatorRotation;

    private bool isObjectPlaced = false;
    private bool placementPoseIsValid = false;

    // Ray Cast Attr
    private Vector3[] surroundingPoints;
    private float raycastBorder = 0.2F;
    private Vector3 _raycastCenter = new Vector3(0.5F, 0.5F, 0);
    private Ray _centerRay;
    private RaycastHit _centerHit;
    private RaycastHit _rcHit;
    private Ray _rcRay;
    private int _rcOnPlainCnt;


    // AR Stuff
    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;


    void Start()
    {   
        if (Application.isEditor)
        {
            _camera = GameObject.FindWithTag("EditorCamera").GetComponent<Camera>();
        }
        else
        {
            GameObject[] objs ;
            objs = GameObject.FindGameObjectsWithTag("EditorOnly");

            foreach(GameObject editorObj in objs) {
                editorObj.SetActive(false);
            }
            
            arOrigin = FindObjectOfType<ARSessionOrigin>();
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            _camera = GameObject.FindWithTag("ARCamera").GetComponent<Camera>();

            Debug.Log(arOrigin);
            Debug.Log(arRaycastManager);
            Debug.Log($"Camera {_camera.name}");
        }
        Debug.Log($"Camera {_camera.name}");
        _camera.enabled = true;
        placementIndicator.SetActive(false);

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
        _centerRay = _camera.ViewportPointToRay(_raycastCenter);
        if (Application.isEditor)
        {
            if (Physics.Raycast(_centerRay, out _centerHit))
            {
                placementPoseIsValid = isPlain(_centerHit);
                placementIndicatorPosition = new Vector3(_centerHit.point.x, _centerHit.point.y + 0.1f, _centerHit.point.z);                
            }
        }
        else
        {
            var hits = new List<ARRaycastHit>();
            arRaycastManager.Raycast(_centerRay, hits, TrackableType.Planes);
        
            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                Debug.Log($"plane hit: x - {hits[0].pose.position.x}; y - {hits[0].pose.position.y}; z - {hits[0].pose.position.z}");
                placementIndicatorPosition = hits[0].pose.position;
            }
        }

        if (placementPoseIsValid)
        {
            var cameraForward = _camera.transform.forward;
            // var y = Application.isEditor ? -90 : 0;
            // var cameraBearing = new Vector3(cameraForward.x, y, cameraForward.z).normalized;
            var cameraBearing = new Vector3(cameraForward.x, -90, cameraForward.z).normalized;
            placementIndicatorRotation = Quaternion.LookRotation(cameraBearing);

            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementIndicatorPosition, placementIndicatorRotation);
        }
        else
        {
            placementIndicator.SetActive(false);
            // print("I'm not looking at a plane!");
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
            _rcRay = _camera.ViewportPointToRay(v);
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
