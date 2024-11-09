using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField]
    private Slider sensibilteSlider;

    private Vector2 lookInput = Vector2.zero;
    private float xRotation = 0f;

    private float smoothPitchVelocity = 0f;
    private float currentXRotation = 0f;

    /// <summary>
    /// Permet d'update la sensibilté de la caméra
    /// </summary>
    /// <param name="newSensibilite">valeur du slider</param>
    public void UpdateSensibilite(float newSensibilite)
    {
        sensitivity = sensibilteSlider.value;
    }

    /// <summary>
    /// Cache et bloque la souris
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(sensibilteSlider != null ) 
            sensitivity = sensibilteSlider.value;
    }

    /// <summary>
    /// OnLook Input Values
    /// </summary>
    /// <param name="context"></param>
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Late update pour la caméra
    /// </summary>
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
