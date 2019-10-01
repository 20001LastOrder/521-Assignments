using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for control the player
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

    private void Update()
    {
        // only let player move at certain state
        if (GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.GameFinished)
        {
            return;
        }

        // rotation
        Rotation();
    }

    //Fix update to make sure physics engine works properly
    void FixedUpdate()
    {
     
        // different move strategy based on the current status of the game
        if(GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze)
        {
            OutsideMovement();
        }
        else if(GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.InMaze)
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

    //Use grid based system after entering the maze
    private void GridBasedMovement()
    {
        if (_isJumping || _isMoving)
        {
            return;
        }

        // use WSAD to control the grid move
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            var moveDirection = transform.forward;
            string key = Input.inputString;
            if (key.Equals("A") || key.Equals("a"))
            {
                var z = moveDirection.z;
                moveDirection.z = moveDirection.x;
                moveDirection.x = -z;
            }else if (key.Equals("S") || key.Equals("s"))
            {
                moveDirection.x = -moveDirection.x;
                moveDirection.z = -moveDirection.z;

            }
            else if (key.Equals("D") || key.Equals("d"))
            {
                var z = moveDirection.z;
                moveDirection.z = -moveDirection.x;
                moveDirection.x = z;
            }

            Debug.Log(moveDirection);
            //Check the next direction to move based on current forward direction
            var currentCoord = new Tuple<int, int>((int)Math.Round(this.transform.position.x), (int)Math.Round(this.transform.position.z));
            Vector3 desiredPosition;
            if(Math.Abs(moveDirection.z) > Math.Abs(moveDirection.x)){
                desiredPosition = new Vector3(this.transform.position.x, this.transform.position.y, currentCoord.Item2 + Math.Sign(moveDirection.z));
            }
            else
            {
                desiredPosition = new Vector3(currentCoord.Item1 + Math.Sign(moveDirection.x), this.transform.position.y, this.transform.position.z);
            }

            var coord = new Tuple<int, int>((int)Math.Round(desiredPosition.x), (int)Math.Round(desiredPosition.z));

            // if the next grid is safe, and the player is not currently stuck by a tree, then move there
            if (MazeManager.Instance.CanMove(coord))
            {
                //Move Indicator to the next position
                MazeManager.Instance.NextGuidance(currentCoord, coord);
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
        // 0.01 for numerical issue
        while(Vector3.Distance(transform.position, desiredPosition) > 0.1f)
        {
            transform.position += direction * _gridMovementSpeed;
            yield return null;
        }

        //Update Maze Time
        MazeManager.Instance.ForwardTime();

        //check if the player is blocked by the tree, if it is then game over.
        var currentCoord = new Tuple<int, int>((int)Math.Round(this.transform.position.x), (int)Math.Round(this.transform.position.z));
        if (!MazeManager.Instance.CanMove(currentCoord))
        {
            GameFlowManager.Instance.GameLose();
            yield break;
        }
        _isMoving = false;
    }

    // reset everything for the player
    public void ResetPlayer()
    {
        _isJumping = false;
        _isMoving = false;
        GetComponent<Rigidbody>().useGravity = true;
        this.StopAllCoroutines();
    }

    public void frozenPlayer()
    {
        _gridMovementSpeed = 0;
        _speed = 0;
    }
}
