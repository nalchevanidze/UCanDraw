﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceMarker : MonoBehaviour
{
    public ARRaycastManager arraycastManager;
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public GameObject objectToPlace;

    _GM gameManager;


    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        gameManager = FindObjectOfType<_GM>();
    }

   
    void Update()
    {
        if (Camera.current)
        {
            UpdatePlacementpose();
            UpdatePlacementIndicator();
        }

        if (Input.GetKeyDown(KeyCode.Space) || placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        if (gameManager.planeSet)
            return;

        System.Random rnd = new System.Random();
        GameObject gm;
        if (placementPoseIsValid)
            gm = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        else
        {
            gm = Instantiate(objectToPlace);
            gm.transform.position = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble());
        }
        gameManager.AddMarker(gm);

    }

    private void UpdatePlacementIndicator()
    {
       if (placementPoseIsValid && !gameManager.planeSet)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

        }
       else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementpose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arraycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing); 
        }

    }
}
