using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float gravity = 9.8f;
    private float verticalRotation = 0;
    private Vector3 velocity;
    private bool isGrounded;
    private CharacterController controller;
    public bool isHiding = false;
    public AudioClip footstepSFX;
    private AudioSource audioSource;

    public float stepInterval = 0.5f; // Time between footstep sounds
    private float stepTimer = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = footstepSFX;
        audioSource.loop = true;
        audioSource.playOnAwake = false;


    }

    // Update is called once per frame
    void Update()
    {
        // Mouse look (always allow camera movement)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(0, mouseX, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        if (!isHiding) // Only allow position movement if not hiding
        {
            // Movement
            float moveDirectionY = velocity.y;
            velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * speed;
            velocity.y = moveDirectionY;

            // Check if the player is grounded
            isGrounded = controller.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small value to keep the player grounded
            }

            // Jump
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);
            }

            // Apply gravity
            velocity.y -= gravity * Time.deltaTime;

            // Move the player
            controller.Move(velocity * Time.deltaTime);

            // Footstep SFX logic
            bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

            if (controller.isGrounded && isMoving)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause(); // Use .Stop() if you want to restart the sound on resume
                }
            }

        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
}
