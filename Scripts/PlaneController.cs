using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class PlaneController : MonoBehaviour
{

    public float throttleIncrement = 0.1f;
    public float MaxThrust = 200f;
    public float responsiveness = 10f;
    public float lift = 135f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    private float responsiveModifier
    {
        get 
        { 
            return (rb.mass / 10f) * responsiveness;
        }
    }

    Rigidbody rb;
    [SerializeField] TextMeshProUGUI hud;
    [SerializeField] Transform propeller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void HandleInput()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space)) throttle += throttleIncrement * Time.deltaTime * 200;
        if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement * Time.deltaTime * 200;
        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    private void Update()
    {
        HandleInput();
        UpdateHud();

        propeller.Rotate(Vector3.forward * throttle);
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * MaxThrust * throttle);
        rb.AddTorque(transform.up * yaw * responsiveModifier);
        rb.AddTorque(transform.right * pitch * responsiveModifier);
        rb.AddTorque(-transform.forward * roll * responsiveModifier);

        rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);
    }

    private void UpdateHud()
    {
        hud.text = "Throttle: " + throttle.ToString("F0") + "%\n";
        hud.text += "AirSpeed: " + (rb.velocity.magnitude).ToString("F0") + "km/h\n";
        hud.text += "Altitude " + transform.position.y.ToString("F0") + " m\n";
    }

}
