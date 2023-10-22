using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerAfterImage afterImage;
        
        [Header("Property")]
        public float moveSpeed = 7f;
        public float maxJumpHeight = 5f;
        public float dashSpeed = 30f;
        [SerializeField]
        private float dashLength = 0.3f;
        
        [Header("Ground Check")]
        public Transform groundCheckPosition;
        public LayerMask groundCheckLayer;
        public Vector2 groundCheckSize;
        
        private InputManager _input;
        private Rigidbody2D _rigid;
        private Animator _animator;
        private SpriteRenderer _sprite;

        private Vector2 _respawnPos;

        private int _dashCount = 1;
        private int _jumpCount = 1;
        private float _startGravityScale;
        
        private bool _isDashing;
        private bool _wasDashing;
        private float _dashingTimeout;
        private Vector2 _dashInput;
        
        private bool _isJumping;
        private bool _wasJumping;
        private bool _isGrounded;
        private bool _wasGrounded;

        private Animator _previousCheckpoint;
        
        private static readonly int VelocityX = Animator.StringToHash("velocityX");
        private static readonly int VelocityY = Animator.StringToHash("velocityY");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int IsDashing = Animator.StringToHash("isDashing");
        private static readonly int IsChecked = Animator.StringToHash("isChecked");

        private void Awake()
        {
            _input = GetComponent<InputManager>();
            _rigid = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();

            _startGravityScale = _rigid.gravityScale;
            _respawnPos = transform.position;
        }

        private void Update()
        {
            _wasGrounded = _isGrounded;
            _isGrounded = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundCheckLayer);
            if (!_isDashing)
            {
                Move();
                Jump();
            }
            Dash();
            if (transform.position.y < -10f)
                OnDeath();
            
            _animator.SetBool(IsGrounded, _isGrounded);
            _animator.SetBool(IsDashing, _isDashing);
            _animator.SetFloat(VelocityX, Mathf.Abs(_rigid.velocity.x));
            _animator.SetFloat(VelocityY, _rigid.velocity.y);
        }

        private void Dash()
        {
            if (!_isDashing && !_wasDashing && _input.dash && _dashCount > 0)
            {
                _rigid.gravityScale = 0f;
                _isDashing = true;
                _dashCount -= 1;
                _dashingTimeout = dashLength;
                _dashInput = _input.move;
            }

            if (_isDashing)
            {
                PlayerAfterImage image = Instantiate(afterImage, transform.position, transform.rotation);
                image.StartAfterImage(_sprite.sprite);
                _rigid.velocity = _dashInput * dashSpeed;
                _dashingTimeout -= Time.deltaTime;
                if (_dashingTimeout <= 0)
                {
                    _isDashing = false;
                    _rigid.gravityScale = _startGravityScale;
                    _rigid.velocity = _dashInput;
                }
            }

            _wasDashing = _input.dash;
        }

        private void Jump()
        {
            if (!_wasGrounded && _isGrounded)
            {
                _isJumping = false;
                _jumpCount = 1;
                _dashCount = 1;
            }

            if (!_wasJumping && _input.jump && _jumpCount > 0)
            {
                _isJumping = true;
                _jumpCount = 0;
                _rigid.velocity = new Vector2(_rigid.velocity.x, Mathf.Sqrt(2f * maxJumpHeight * Mathf.Abs(Physics2D.gravity.y) * _rigid.gravityScale));
            }

            if (_wasJumping && !_input.jump && _isJumping && _rigid.velocity.y > 0)
            {
                _isJumping = false;
                _rigid.velocity = new Vector2(_rigid.velocity.x,  Mathf.Sqrt(0.2f * maxJumpHeight * Mathf.Abs(Physics2D.gravity.y) * _rigid.gravityScale));
            }

            _wasJumping = _input.jump;
        }

        private void Move()
        {
            switch (_input.move.x)
            {
                case > 0:
                    _sprite.flipX = false;
                    break;
                case < 0:
                    _sprite.flipX = true;
                    break;
            }

            _rigid.velocity = new Vector2(_input.move.x * moveSpeed, _rigid.velocity.y);
        }

        private void OnDeath()
        {
            transform.position = _respawnPos;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out JumpOrb jumpOrb) && _jumpCount == 0)
            {
                _jumpCount = 1;
                jumpOrb.Consume();
            }
            if (other.TryGetComponent(out DashOrb dashOrb) && _dashCount == 0)
            {
                _dashCount = 1;
                dashOrb.Consume();
            }
            if (other.CompareTag("Checkpoint"))
            {
                _respawnPos = other.transform.position;
                if (_previousCheckpoint)
                    _previousCheckpoint.SetBool(IsChecked, false);

                _previousCheckpoint = other.GetComponent<Animator>();
                _previousCheckpoint.SetBool(IsChecked, true);
            }
        }
    }
}
