using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private InputController input;
    [SerializeField, Range(0f, 100f)]
    private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)]
    private float maxAirAcceleration = 20f;
    [SerializeField, Range(0.05f, 0.5f)]
    private float _wallStickTime = 0.25f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 _velocity;
    private Rigidbody2D _body;
    private CollisionDataRetriever _collisionDataRetriever;
    private WallInteractor _wallInteractor;

    private float _maxSpeedChange, _acceleration, _wallStickCounter;
    private bool _onGround;

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        _wallInteractor = GetComponent<WallInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = input.RetrieveMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(maxSpeed - _collisionDataRetriever.Friction, 0f);

    }

    private void FixedUpdate()
    {
        _onGround = _collisionDataRetriever.OnGround;
        _velocity = _body.velocity;

        _acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        _maxSpeedChange = _acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, _maxSpeedChange);

        #region Wall Stick
        if(_collisionDataRetriever.OnWall && !_collisionDataRetriever.OnGround && !_wallInteractor.WallJumping)
        {
            if(_wallStickCounter > 0)
            {
                _velocity.x = 0;

                if (input.RetrieveMoveInput() == _collisionDataRetriever.ContactNormal.x)
                {
                    _wallStickCounter -= Time.deltaTime;
                }
                else
                {
                    _wallStickCounter = _wallStickTime;
                }
            }
            else
            {
                _wallStickCounter = _wallStickTime;
            }
        }
        #endregion

        _body.velocity = _velocity;
    }
}
