using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DroneManager : MonoBehaviour
{

    public DroneController droneController;

    public Button baseB;

    public Button flyB;
    public Button landB;
    public Button leftRotation;
    public Button rightRotation;


    public GameObject controlR;
    public GameObject controlL;
    public GameObject controlRotation;


/*
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;
    List<ARRaycastHit> hitResult = new List<ARRaycastHit>();
*/
    public GameObject drone;
    private PlacementIndicator placementIndicator;

    struct DroneAnimationControls
    {
        public bool moving;
        public bool interpolatingAsc;
        public bool interpolatingDesc;
        public float axis;
        public float direction;

    }

    DroneAnimationControls movingLeft;
    DroneAnimationControls movingBack;

    //Up down
    DroneAnimationControls movingUp;


    // Start is called before the first frame update
    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        flyB.onClick.AddListener(EventOnClickFlyButton);
        landB.onClick.AddListener(EventOnClickLandButton);
        leftRotation.onClick.AddListener(EventOnClicLeftRotationButton);
        rightRotation.onClick.AddListener(EventOnClicRightRotationButton);
    }

    // Update is called once per frame
    void Update()
    {
        //float moveX = Input.GetAxis("Horizontal");
        //float moveZ = Input.GetAxis("Vertical");

        UpdateControls(ref movingLeft);
        UpdateControls(ref movingBack);
        UpdateControls(ref movingUp);

        droneController.Move(movingLeft.axis * movingLeft.direction, 
                            movingBack.axis * movingBack.direction, 
                            movingUp.axis * movingUp.direction);   

        if(droneController.IsIdle() && placementIndicator.hitsRay)
        {
            PlaceDrone();
        }     

    }

    void PlaceDrone()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !placementIndicator.oneTouched)
        {
            placementIndicator.oneTouched = true;
            drone.transform.position = placementIndicator.transform.position;
            drone.transform.rotation = placementIndicator.transform.rotation;
            placementIndicator.visual.SetActive(false);
            drone.SetActive(true);

            baseB.gameObject.SetActive(true);
            flyB.gameObject.SetActive(true);
            landB.gameObject.SetActive(true);
        }
    }

/*
    void UpdateAR()
    {
        Vector2 positionScreenSpace = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        arRaycastManager.Raycast(positionScreenSpace, hitResult, TrackableType.PlaneWithinBounds);

        if(hitResult.Count > 0)
        {
            if(arPlaneManager.GetPlane(hitResult[0].trackableId).alignment == PlaneAlignment.HorizontalUp)
            {
                Pose pose = hitResult[0].pose;
                drone.transform.position = pose.position;
                drone.SetActive(true);
                arPlaneManager.planePrefab.SetActive(false);
                baseB.gameObject.SetActive(true);
                flyB.gameObject.SetActive(true);
                landB.gameObject.SetActive(true);
            }
        }
    }

*/
    void UpdateControls(ref DroneAnimationControls controls)
    {
        if(controls.moving || controls.interpolatingAsc || controls.interpolatingDesc)
        {
            if(controls.interpolatingAsc)
            {
                controls.axis += 0.05f;

                if(controls.axis >= 1.0f)
                {
                    controls.axis = 1.0f;
                    controls.interpolatingAsc = false;
                    controls.interpolatingDesc = true;
                }
            }
            else if(!controls.moving)
            {
                controls.axis -= 0.05f;
                if(controls.axis <= 0.0f)
                {
                    controls.axis = 0.0f;
                    controls.interpolatingDesc = false;
                }
            }
        }
    }

    void EventOnClickFlyButton()
    {
        if(droneController.IsIdle())
        {
            droneController.TakeOff();
            flyB.gameObject.SetActive(false);
            landB.gameObject.SetActive(true);
            controlR.SetActive(true);
            controlL.SetActive(true);
            //CYJu temp
            controlRotation.SetActive(true);
        }

    }
    void EventOnClickLandButton()
    {
        if(droneController.IsFlying())
        {
            droneController.Land();
            landB.gameObject.SetActive(false);
            flyB.gameObject.SetActive(true);
            controlR.SetActive(false);
            controlL.SetActive(false);
            //CYJu temp
            controlRotation.SetActive(false);
        }
    }

       //Left Rotation Button
    public void EventOnClicLeftRotationButton()
    {
        if(droneController.IsFlying())
        {
            droneController.leftClickRotation = true;
        }
    }

    //Right Rotation Button
    public void EventOnClicRightRotationButton()
    {
        if(droneController.IsFlying())
        {
            droneController.rightclickRotation = true;
        }
    }

     //Left Button
    public void EventOnLeftButtonPressed()
    {
        movingLeft.moving = true;
        movingLeft.interpolatingAsc = true;
        movingLeft.direction = -1.0f;
    }
    public void EventOnLeftButtonReleased()
    {
        movingLeft.moving = false;
    }

    //Right Button
    public void EventOnRightButtonPressed()
    {
        movingLeft.moving = true;
        movingLeft.interpolatingAsc = true;
        movingLeft.direction = 1.0f;
    }
    public void EventOnRightButtonReleased()
    {
        movingLeft.moving = false;
    }

    //Back Button
    public void EventOnBackButtonPressed()
    {
        movingBack.moving = true;
        movingBack.interpolatingAsc = true;
        movingBack.direction = -1.0f;
    }
    public void EventOnBackButtonReleased()
    {
        movingBack.moving = false;
    }

    //Forward Button
    public void EventOnForwardButtonPressed()
    {
        movingBack.moving = true;
        movingBack.interpolatingAsc = true;
        movingBack.direction = 1.0f;
    }
    public void EventOnForwardButtonReleased()
    {
        movingBack.moving = false;
    }

    //Up Button
    public void EventOnUpButtonPressed()
    {
        movingUp.moving = true;
        movingUp.interpolatingAsc = true;
        movingUp.direction = 1.0f;
    }
    public void EventOnUpButtonReleased()
    {
        movingUp.moving = false;
    }

        //Down Button
    public void EventOnDownButtonPressed()
    {
        movingUp.moving = true;
        movingUp.interpolatingAsc = true;
        movingUp.direction = -1.0f;
    }
    public void EventOnDownButtonReleased()
    {
        movingUp.moving = false;
    }


}
