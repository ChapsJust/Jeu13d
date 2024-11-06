using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Body & Camera du Joueur")]
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private Transform cameraTransform;

    [Header("Options")]
    [SerializeField]
    private float sensitivity = 100f;
    [SerializeField]
    private float smoothing = 0.05f;

    private Vector2 lookInput = Vector2.zero;
    private float xRotation = 0f;

    private float smoothPitchVelocity = 0f;
    private float currentXRotation = 0f;

    //Cache et bloque la souris
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //OnLook Input Values
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //Late update pour la caméra
    private void LateUpdate()
    {
        if (lookInput != Vector2.zero)
        {
            float mouseX = lookInput.x * sensitivity * Time.deltaTime;
            playerBody.Rotate(Vector3.up * mouseX);


            float mouseY = lookInput.y * sensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref smoothPitchVelocity, smoothing);

            cameraTransform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        }
    }
}
