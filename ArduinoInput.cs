using System.IO.Ports;
using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM11", 9600);

    public bool isForwardPressed = false;
    public bool isBrakePressed = false;
    public bool isBackwardPressed = false;

    private int gx, gy, gz;

    // Simulated steering
    private float steeringAngle = 0f;
    public float maxSteeringAngle = 450f;         
    public float gyroToDegreeFactor = 0.05f;       
    public float deadzone = 50f;               
    public float returnSpeed = 2f;                 

    public float normalizedSteering = 0f;
    [SerializeField] CarControllerScript carController;

    private float odorSendInterval = 2.0f; // 2 seconds
    private float lastOdorSentTime = 0f;

    void Start()
    {
        stream.Open();
    }

    void Update()
    {
        if (stream.IsOpen)
        {

            if (Time.time - lastOdorSentTime >= odorSendInterval)
            {
               
                    stream.WriteLine("ODOR:" + carController.currentSpeed.ToString("F0")); 
                  
                
                lastOdorSentTime = Time.time;
            }

            try
            {
                string data = stream.ReadLine();
                string[] values = data.Split(',');

                if (data.Contains("Brake"))
                {
                    isBrakePressed = true;
                   
                }
                else
                {
                    isBrakePressed = false;
                }

                if (data.Contains("Backward"))
                {
                    isBackwardPressed = true;
                
                }
                else
                {
                    isBackwardPressed = false;
                }



                if (data.Contains("Forward"))
                {
                    isForwardPressed = true;

                }
                else
                {
                    isForwardPressed = false;
                }


                if (data.Contains("Forward"))
                {
                    isForwardPressed = true;

                }
                else
                {
                    isForwardPressed = false;
                }

                
                if (values.Length == 6)
                {
                    int ax = int.Parse(values[0]);
                    int ay = int.Parse(values[1]);
                    int az = int.Parse(values[2]);
                    gx = int.Parse(values[3]);
                    gy = int.Parse(values[4]);
                    gz = int.Parse(values[5]);

                    int gyroX = -gx; 

             

                  
                    if (Mathf.Abs(gyroX) < deadzone)
                    {
                        steeringAngle = Mathf.Lerp(steeringAngle, 0f, Time.deltaTime * returnSpeed);
                    }
                    else
                    {
                        steeringAngle += gyroX * gyroToDegreeFactor * Time.deltaTime;
                        steeringAngle = Mathf.Clamp(steeringAngle, -maxSteeringAngle, maxSteeringAngle);
                    }

                    normalizedSteering = steeringAngle / maxSteeringAngle;
                }
            }
            catch (System.Exception)
            {
                // Ignore bad lines or timeout
            }
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("Serial port closed on quit.");
        }
    }


    public float GetSteeringInput()
    {
        return normalizedSteering;
    }

    public bool IsAccelerating()
    {
        return isForwardPressed;
    }

    public bool isBraking()
    {
        return isBrakePressed;
    }

    public bool IsDeAccelerating()
    {
        return isBackwardPressed;
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("collide");

            if (stream.IsOpen)
            {
                stream.WriteLine("BUZZ");
            }
        }
    }

   

}
