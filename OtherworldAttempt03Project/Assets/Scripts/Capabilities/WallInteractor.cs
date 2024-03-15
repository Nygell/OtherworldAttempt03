using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteractor : MonoBehaviour
{
    public bool WallJumping { get; private set; }

    [Header("Wall Slide")]
    [SerializeField]
    [Range(0.1f, 5f)]
    private float _wallSlideMaxSpeed;

    [Header("Wall Jump")]
    [SerializeField]
    private Vector2 _wallJumpClimb = new(4f, 12f);
    [SerializeField]
    private Vector2 _wallJumpBounce = new(4f, 12f);
    [SerializeField]
    private Vector2 _wallJumpLeap = new(14f, 12f);

    private CollisionDataRetriever _collisionDataRetriever;
    private Rigidbody2D _body2D;
    [SerializeField]
    private InputController _controller;

    private Vector2 _velocity;
    private bool _onWall, _onGround, _desiredJump;
    private float _wallDirectionX;
    void Start()
    {
        _collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        _body2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_onWall && !_onGround)
        {
            _desiredJump |= _controller.RetrieveJumpInput();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _collisionDataRetriever.EvaluateCollision(collision);

        if (_collisionDataRetriever.OnWall && !_collisionDataRetriever.OnGround && WallJumping)
        {
            _body2D.velocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        _velocity = _body2D.velocity;
        _onWall = _collisionDataRetriever.OnWall;
        _onGround = _collisionDataRetriever.OnGround;
        _wallDirectionX = _collisionDataRetriever.ContactNormal.x;

        #region Wall Slide
        if (_onWall)
        {
            if(_velocity.y < -_wallSlideMaxSpeed)
            {
                _velocity.y = -_wallSlideMaxSpeed;
            }
        }
        #endregion

        #region Wall Jump

        if((_onWall && _velocity.x == 0) || _onGround)
        {
            WallJumping = false;
        }

        if(_desiredJump)
        {
            if(-_wallDirectionX == _controller.RetrieveMoveInput())
            {
                _velocity = new(_wallJumpClimb.x * _wallDirectionX, _wallJumpClimb.y);
                WallJumping = true;
                _desiredJump = false;
            }
            else if (_controller.RetrieveMoveInput() == 0)
            {
                _velocity = new(_wallJumpBounce.x * _wallDirectionX, _wallJumpBounce.y);
                WallJumping = true;
                _desiredJump = false;
            }
            else
            {
                _velocity = new(_wallJumpLeap.x * _wallDirectionX, _wallJumpLeap.y);
                WallJumping = true;
                _desiredJump = false;
            }
        }
        #endregion

        _body2D.velocity = _velocity;
    }
}
