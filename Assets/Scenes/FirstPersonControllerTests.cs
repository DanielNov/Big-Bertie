using UnityEngine;
//Header - aby to vyzeralo pekne v editore 
public class FirstPersonControllerTests : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;  //does the player have control of the character <Not set outside>
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f; //defaut walk speed and gravity
    [SerializeField] private float gravity = 30.0f;
    
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f; //Camera look limit
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;


    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; //Locked Cursor
        Cursor.visible = false;  
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook(); 
            ApplyFinalMovements();

        }

    }

    private void HandleMovementInput() //Keyboard Controller 
    {
        currentInput = new Vector2 (walkSpeed * Input.GetAxis("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));
        //Reset Vecotr Direction for next movement 
        float moveDirectionY  = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY; // moveDirection.y set to cached moveDirectionY
    }

    private void HandleMouseLook()
    {

    }

    private void ApplyFinalMovements()
    {

    }



}