using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField]
    private InputController input;
    [SerializeField, Range(0f, 10f)]
    private float jumpHeight = 3f;
    [SerializeField, Range(0, 5)]
    private int maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)]
    private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)]
    private float upwardMovementMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)]
    private float _coyoteTime = 0.2f;
    [SerializeField, Range(0f, 0.3f)]
    private float _jumpBufferTime = 0.2f;

    private Rigidbody2D _body;
    private CollisionDataRetriever _ground;
    private Vector2 _velocity;

    private int _jumpPhase;
    private float _defaultGravityScale, _coyoteCounter, _jumpSpeed, _jumpBufferCounter;

    private bool _desiredJump, _onGround, _isJumping;
    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<CollisionDataRetriever>();

        _defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        _desiredJump |= input.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.OnGround;
        _velocity = _body.velocity;

        if (_onGround && _body.velocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if (!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if (_body.velocity.y > 0f && input.RetrieveJumpHoldInput())
        {
            _body.gravityScale = upwardMovementMultiplier;
        }
        else if (_body.velocity.y < 0f || !input.RetrieveJumpHoldInput())
        {
            _body.gravityScale = downwardMovementMultiplier;
        }
        else if (_body.velocity.y == 0f)
        {
            _body.gravityScale = _defaultGravityScale;
        }

        _body.velocity = _velocity;
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0f || (_jumpPhase < maxAirJumps && _isJumping))
        {
            if (_isJumping)
            {
                _jumpPhase++;
            }

            _jumpBufferCounter = 0;
            _coyoteCounter = 0;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            _isJumping = true;

            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }
            _velocity.y += _jumpSpeed;
        }
    }
}
