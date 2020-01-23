using UnityEngine;
using System.Collections;

public class AngarCameraTest : MonoBehaviour
{
    public Transform target;


    private float distance = 5.0f;
    public float distanceZ = 0f;
    public float distanceY = 0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 12f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;
    #region Camera Zoom
    public float speedCam;
    public float defaultPOV = 50f;
    public float zoomRatio = 100f;
    #endregion
    float x = 0.0f;
    float y = 0.0f;

    private GameObject car;

    protected bool IsInRotationMode = false;


    //--------------------------------------
    // INITIALIZE
    //--------------------------------------


    // Use this for initialization
    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        speedCam = car.GetComponent<Rigidbody>().velocity.magnitude;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        distance = 2f;

    }


    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsInRotationMode = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsInRotationMode = false;
        }
    }
    #region Camera Zoom Function
    public void FixedUpdate()
    {
        GetComponent<Camera>().fieldOfView = defaultPOV + speedCam * zoomRatio * Time.deltaTime;
        //Debug.Log(GetComponent<Camera>().fieldOfView);
    }
    #endregion
    void LateUpdate()
    {
        /*if (target != null && IsInRotationMode) {
			x += Input.GetAxis ("Mouse X") * xSpeed * distance * 0.02f;
			y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

		}*/


        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);


        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

        /*RaycastHit hit;
		if (Physics.Linecast (target.position, transform.position, out hit)) {
			distance -=  hit.distance;
		}*/


        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        //transform.rotation = rotation;
        //transform.position = position;//Hrant comment

        transform.position = new Vector3(target.position.x, position.y + distanceY, distanceZ);




    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }


    //--------------------------------------
    // PUBLIC METHODS
    //--------------------------------------

    //--------------------------------------
    // GET / SET
    //--------------------------------------

    //--------------------------------------
    // EVENTS
    //--------------------------------------

    //--------------------------------------
    // PRIVATE METHODS
    //--------------------------------------

    //--------------------------------------
    // DESTROY
    //--------------------------------------
}
