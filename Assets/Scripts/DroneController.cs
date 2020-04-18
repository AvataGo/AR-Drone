using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{

    enum DroneState
    {
        DRONE_STATE_IDLE,
        DRONE_STATE_START_TAKINGOFF,
        DRONE_STATE_TAKEINGOFF,
        DRONE_STATE_MOVING_UP,
        DRONE_STATE_FLYING,
        DRONE_STATE_START_LANDING,
        DRONE_STATE_LANDING,
        DRONE_STATE_LANDED,
        DRONE_STATE_WAIT_ENDING_STOP
    }

    DroneState droneState;
    private Animator theAnim;

    //Drone Sound
    private AudioSource theAudio;
    [SerializeField]
    private AudioClip theClip;

    Vector3 moveDrone = new Vector3(0.0f, 0.0f, 0.0f);

    public float theSpeed = 0.75f;

    public bool leftClickRotation = false;
    public bool rightclickRotation = false;


    // Start is called before the first frame update
    void Start()
    {
        theAnim = GetComponent<Animator>();
        theAudio = GetComponent<AudioSource>();
        droneState = DroneState.DRONE_STATE_IDLE;       
    }

    private void AudioPlaySE()
    {
        theAudio.clip = theClip;
        theAudio.Play();
    }
    private void AudioPlayStop()
    {
        theAudio.Stop();
    }

    public bool IsIdle()
    {
        return (droneState == DroneState.DRONE_STATE_IDLE);
    }
    public void TakeOff()
    {
        droneState = DroneState.DRONE_STATE_START_TAKINGOFF;
    }

    public bool IsFlying()
    {
        return (droneState == DroneState.DRONE_STATE_FLYING);
    }

    public void Land()
    {
        droneState = DroneState.DRONE_STATE_START_LANDING;
    }

    public void Move(float moveX, float moveZ, float moveY)
    {
        moveDrone.x = moveX;
        moveDrone.z = moveZ;
        moveDrone.y = moveY;

        UpdateDrone();
    }
    private float DroneRotation()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;

        if(leftClickRotation)
        {
            rotation.y = rotation.y - 1.0f;        
        }
        if(rightclickRotation)
        {           
            rotation.y = rotation.y + 1.0f;
        }
        return rotation.y;
    }
    public void DroneStateFlying()
    {
        float angleZ = moveDrone.x * 60.0f * Time.deltaTime;
        float angleX = moveDrone.z * 60.0f * Time.deltaTime; 
        float angleY = DroneRotation();        

        Transform childTransform = this.gameObject.transform.GetChild(0);
        Vector3 childRotation = childTransform.localRotation.eulerAngles;        
        float childAngleZ = -30.0f * moveDrone.x * 60.0f * Time.deltaTime;
        float childAngleX = 30.0f * moveDrone.z * 60.0f * Time.deltaTime; 
        
        //Turn Drone
        transform.Translate(moveDrone.normalized * theSpeed * Time.deltaTime); 
        transform.localRotation = Quaternion.Euler(angleX, angleY, angleZ); 
        childTransform.localRotation = Quaternion.Euler(childAngleX,childRotation.y,childAngleZ);
    }

    void UpdateDrone()
    {

        switch (droneState)
        {
            case DroneState.DRONE_STATE_IDLE:
                break;

            case DroneState.DRONE_STATE_START_TAKINGOFF:                
                theAnim.SetBool("TakeOff", true);
                AudioPlaySE();
                droneState = DroneState.DRONE_STATE_TAKEINGOFF;
                break;

            case DroneState.DRONE_STATE_TAKEINGOFF:                
                if (theAnim.GetBool("TakeOff") == false)
                {
                    droneState = DroneState.DRONE_STATE_MOVING_UP;
                }
                break;

            case DroneState.DRONE_STATE_MOVING_UP:                
                if (theAnim.GetBool("MoveUp") == false)
                {
                    droneState = DroneState.DRONE_STATE_FLYING;
                }
                break;

            case DroneState.DRONE_STATE_FLYING:   
                DroneStateFlying();                                 
                break;

            case DroneState.DRONE_STATE_START_LANDING:                
                theAnim.SetBool("MoveDown", true);
                droneState = DroneState.DRONE_STATE_LANDING;
                break;

            case DroneState.DRONE_STATE_LANDING:                
                if (theAnim.GetBool("MoveDown") == false)
                {
                    droneState = DroneState.DRONE_STATE_LANDED;
                }
                break;            

            case DroneState.DRONE_STATE_LANDED:                
                theAnim.SetBool("Land", true);
                AudioPlayStop();
                droneState = DroneState.DRONE_STATE_WAIT_ENDING_STOP;
                break;

            case DroneState.DRONE_STATE_WAIT_ENDING_STOP:                
                if (theAnim.GetBool("Land") == false)
                {
                    droneState = DroneState.DRONE_STATE_IDLE;
                }
                break;
        }
    }

}
