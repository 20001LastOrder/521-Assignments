using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;
    [SerializeField]
    private float _rotationSpeed = 5.00f;
    [SerializeField]
    private float _jumpSpeed = 0.1f;
    [SerializeField]
    private float _jumpHeight = 1f;

    [SerializeField]
    private float _gridMovementSpeed = 0.001f;
    [SerializeField]
    private float _gridJumpSpeed = 10f;
    private float _gridJumpHeight = 10f;

    [SerializeField]
    private GameObject _playerCamera;
    private  float x_rotation;
    private float y_rotation = 180f;
    private bool _isJumping = false;
    private bool _isMoving = false;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // different move strategy based on the current status of the game
        if(GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze)
        {
            OutsideMovement();
        }
        else
        {
            GridBasedMovement();
        }
    }

    private void OutsideMovement()
    {
        if (!_isJumping)
        {
            //movements
            var z = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
            var x = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;

            //move player forward
            transform.Translate(x, 0, z);

            //check for jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumping = true;
                StartCoroutine(Jump());
            }
        }


        // rotation
        Rotation();
    }

    private IEnumerator Jump()
    {
        GetComponent<Rigidbody>().useGravity = false;
        var groundHeight = transform.position.y;
        var height = GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze ? _jumpHeight : _gridJumpHeight;
        var speed = GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze ? _jumpSpeed : _gridJumpSpeed;

        while (transform.position.y < height)
        {
            var yPlacement = speed * Time.deltaTime;
            transform.Translate(0, yPlacement, 0);
            yield return null;
        }

        GetComponent<Rigidbody>().useGravity = true;
        // add a small value to deal with precision issue
        while (transform.position.y >= groundHeight + 0.01)
        {
            yield return null;
        }
        _isJumping = false;
    }

    private void Rotation()
    {
        // rotation
        x_rotation += _rotationSpeed * Input.GetAxis("Mouse Y");
        y_rotation += _rotationSpeed * Input.GetAxis("Mouse X");
        // Make x rotation within [-70, 70] range
        x_rotation = Mathf.Clamp(x_rotation, -70f, 70f);

        // Wrap y_rotation
        while (y_rotation < 0f)
        {
            y_rotation += 360f;
        }
        while (y_rotation >= 360f)
        {
            y_rotation -= 360f;
        }

        // Set orientation:
        transform.eulerAngles = new Vector3(0, y_rotation, 0f);
        _playerCamera.transform.eulerAngles = new Vector3(-x_rotation, y_rotation, 0);
    }

    private void GridBasedMovement()
    {
        Rotation();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isJumping = false;
            _isMoving = false;
            this.StopAllCoroutines();
            GameFlowManager.Instance.EnterMaze();
        }

        if (_isJumping || _isMoving)
        {
            return;
        }
        
        if(Input.GetKey(KeyCode.W))
        {

            //Check the next direction to move based on current forward direction
            Vector3 desiredPosition;
            if(Math.Abs(transform.forward.z) > Math.Abs(transform.forward.x)){
                desiredPosition = new Vector3(this.transform.position.x, this.transform.position.y, (int)(Math.Round(this.transform.position.z + Math.Sign(transform.forward.z))));
            }
            else
            {
                desiredPosition = new Vector3((int)(Math.Round(this.transform.position.x + Math.Sign(transform.forward.x))), this.transform.position.y, this.transform.position.z);
            }

            var coord = new Tuple<int, int>((int)Math.Round(desiredPosition.x), (int)Math.Round(desiredPosition.z));

            if (MazeManager.Instance.CanMove(coord))
            {
                _isMoving = true;
                var direction = (desiredPosition - this.transform.position).normalized;
                StartCoroutine(GridMove(direction, desiredPosition));
            }
        }

        //check for jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJumping = true;
            StartCoroutine(Jump());
        }

    }

    private IEnumerator GridMove(Vector3 direction, Vector3 desiredPosition)
    {
        Debug.Log(_gridMovementSpeed);
        // 0.01 for numerical issue
        while(Vector3.Distance(transform.position, desiredPosition) > 0.1f)
        {
            transform.position += direction * _gridMovementSpeed;
            yield return null;
        }
        _isMoving = false;

        //Update Maze Time
        MazeManager.Instance.ForwardTime();
    }
}
