using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] MainManager _mainManager;
    [Header("Movement Vars")]
    [SerializeField] private float _walkingSpeed = 7.5f;
    Vector2 currentMovement;
    Vector3 moveDirection = Vector3.zero;

    [Header("Movement Anim")]
    [SerializeField] private Animator playerAnim;
    [SerializeField] private float animationSmoothTime = .1f;
    [SerializeField] private string isHoldingItem;
    [SerializeField] private Animator headRotatingAnim;
    [SerializeField] private string headRotating;
    private int moveYAnimationParamterId;
    private Vector2 currentAnimationBlendVector;
    private Vector2 animationVelocity;


    [Header("Camera Vars")]
    [SerializeField] private bool inverseCam;
    [SerializeField] private GameObject cineAimTarget;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private float modelRotSpeed = 1.5f;
    [SerializeField] private float mouseSensitivity = 1.5f;
    [Tooltip("Minimum Angle X that cam can move")]
    [SerializeField] private float minimum_X = -30f;
    [Tooltip("Maximum Angle X that cam can move")]
    [SerializeField] private float maximum_X = 30f;
    private float rotation_X, rotation_Y;
    private Transform cameraTransform;

    [Header("Raycast Info")]
    [SerializeField] private GameObject crosshairs;
    [SerializeField] private float raycastDistance;
    [SerializeField] private GameObject racycastPoint;
    [SerializeField] private LayerMask ingredientLayerMask;
    [SerializeField] private LayerMask stationLayerMask;
    [SerializeField] private LayerMask tubeLayerMask;


    [Header("Held Item")]
    [SerializeField] private Transform heldItemPos;
    [SerializeField] private float timeToWaitDrop;
    private GameObject heldItem;
    private Ingredient heldIngredient;
    private bool canPickUpItems;
    private bool canDropItems;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlaySounds("Background Music");
        _mainManager.onGamePauseCallBack += OnGamePause;
        _mainManager.onGameOverCallBack += OnGameEnd;
        cameraTransform = Camera.main.transform;
        canPickUpItems = true;
        canDropItems = true;
        moveYAnimationParamterId = Animator.StringToHash("MoveY");
    }

    // Update is called once per frame
    void Update()
    {
        if (_mainManager.canPlayerMove)
        {
            PlayerMovement();
            HandleRotation();
        }
    }

    private void FixedUpdate()
    {
        if (_mainManager.canPlayerMove) Raycasting();
    }

    private void OnDrawGizmos()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.blue;
        Vector3 direction = Camera.main.transform.forward * raycastDistance;
        Gizmos.DrawRay(Camera.main.ViewportToWorldPoint(crosshairs.transform.position), direction);
    }

    void Raycasting()
    {
        if (_inputHandler._pickUpItem)
        {
            GameObject returnedHitObject;
            if (Raycast(ingredientLayerMask, out returnedHitObject) && canPickUpItems && returnedHitObject != null)
            {
                PickUpItem(returnedHitObject.gameObject, returnedHitObject.GetComponent<IngredientPickUp>().ingredient);
                AudioManager.instance.PlaySounds("Pickup/Interact");
            }
            else if (Raycast(stationLayerMask, out returnedHitObject) && returnedHitObject != null)
            {
                Ingredient.Emotion emotionOfIngredient = returnedHitObject.GetComponent<IngredientSpawn>().StartSpawningItem();
                //Code to stop player movement for these idle animations is attached to the animation state in the controller (StateTester())
                switch (emotionOfIngredient)
                {
                    case Ingredient.Emotion.Angry:
                        playerAnim.SetTrigger("emotion_Anger");
                        break;
                    case Ingredient.Emotion.Sad:
                        playerAnim.SetTrigger("emotion_Sad");
                        break;
                    case Ingredient.Emotion.Happy:
                        playerAnim.SetTrigger("emotion_Happy");
                        break;
                    case Ingredient.Emotion.Inspired:
                        playerAnim.SetTrigger("emotion_Thinking");
                        break;
                    case Ingredient.Emotion.Common:
                        break;
                    default:
                        break;
                }
                AudioManager.instance.PlaySounds("Pickup/Interact");
            }
            else if (Raycast(tubeLayerMask, out returnedHitObject) && returnedHitObject != null)
            {
                if (heldIngredient != null && returnedHitObject.GetComponent<WishTube>().CanDropIngredient())
                {
                    playerAnim.SetBool(isHoldingItem, false);
                    heldItem.transform.parent = null;
                    returnedHitObject.GetComponent<WishTube>().InsertIngredient(heldItem, heldIngredient);
                    heldItem = null;
                    heldIngredient = null;
                    canPickUpItems = false;
                    LeanTween.delayedCall(timeToWaitDrop, () => canPickUpItems = true);
                    //Destroy(heldItem);
                }
                AudioManager.instance.PlaySounds("Pickup/Interact");
            }
            else if (heldItem != null && canDropItems)
            {
                DropHeldItem();
                AudioManager.instance.PlaySounds("Pickup/Interact");
            }
        }
    }

    bool Raycast(LayerMask layer, out GameObject hitGameObject)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportToWorldPoint(crosshairs.transform.position), Camera.main.transform.forward, out hit, raycastDistance, layer))
        {
            hitGameObject = hit.transform.gameObject;
            return true;
        }
        else
        {
            hitGameObject = null;
            return false;
        }
    }

    void PlayerMovement()
    {
        currentMovement = _inputHandler._currentMovement;

        //Basic Movement
        //float currentSpeedX = (_inputHandler.GetPlayerCanMove() ? _walkingSpeed : 0) * currentMovement.x;
        float currentSpeedY = (_inputHandler.GetPlayerCanMove() ? _walkingSpeed : 0) * currentMovement.y;

        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, currentMovement, ref animationVelocity, animationSmoothTime);

        //moveDirection = new Vector3(currentSpeedX, 0, currentSpeedY);
        moveDirection = new Vector3(0, 0, currentSpeedY);
        moveDirection = moveDirection.x * cameraTransform.right.normalized + moveDirection.z * cameraTransform.transform.forward.normalized;
        moveDirection.y = 0;

        // Move the controller
        transform.position += (moveDirection * Time.deltaTime);

        if (currentMovement.magnitude > 0)
        {
            //Rotate player model to face camera forward
            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, modelRotSpeed * Time.deltaTime);
        }


        //Set Anim floats
        playerAnim.SetFloat(moveYAnimationParamterId, currentAnimationBlendVector.y);
    }

    void HandleRotation()
    {
        rotation_X += _inputHandler._currentMousePos.y * (_inputHandler.GetPlayerCanMove() ? (inverseCam ? mouseSensitivity * -1f : mouseSensitivity) : 0);
        rotation_Y += _inputHandler._currentMousePos.x * (_inputHandler.GetPlayerCanMove() ? mouseSensitivity : 0);

        rotation_X = ClampAngle(rotation_X, minimum_X, maximum_X);

        Quaternion rotation = Quaternion.Euler(rotation_X, rotation_Y, 0);
        cineAimTarget.transform.rotation = rotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;   //if it goes below add 360  
        }

        if (angle > 360f)
        {
            angle -= 360f;  //if it goes above 360 subtract 
        }

        return Mathf.Clamp(angle, min, max);
    }

    public void PickUpItem(GameObject item, Ingredient ingredient)
    {
        playerAnim.SetBool(isHoldingItem, true);
        item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        item.transform.SetParent(heldItemPos.transform);
        item.transform.position = heldItemPos.position;
        heldItem = item;
        heldIngredient = ingredient;
        canPickUpItems = false;
        canDropItems = false;
        LeanTween.delayedCall(timeToWaitDrop, () => canDropItems = true);
    }

    public void DropHeldItem()
    {
        playerAnim.SetBool(isHoldingItem, false);
        heldItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        heldItem.transform.parent = null;
        heldItem = null;
        heldIngredient = null;
        canPickUpItems = false;
        LeanTween.delayedCall(timeToWaitDrop, () => canPickUpItems = true);
    }

    private void OnGamePause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            playerAnim.SetFloat(moveYAnimationParamterId, 0);
        }
    }

    private void OnGameEnd(MainManager.GameOverState gameOverState)
    {
        playerAnim.SetFloat(moveYAnimationParamterId, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WishTubeBtn"))
        {
            headRotatingAnim.SetTrigger(headRotating);
        }
    }
}
