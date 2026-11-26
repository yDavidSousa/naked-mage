using UnityEngine;

public struct MotorBody
{
    public Vector2 Size;
    public Vector2 CenterOffset;
}

[System.Serializable]
public struct MotorSettings
{
    public float MaxSpeed;
    public float Acceleration;
    public float Gravity;
    public float MaxFallSpeed;
    public float JumpSpeed;
}

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerCharacterView : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _bodyCollider;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Space(10f)]
    [SerializeField] private MotorSettings _motorSettings = new()
    {
        MaxSpeed = 7f,
        Acceleration = 45f,
        Gravity = 35f,
        MaxFallSpeed = 18f,
        JumpSpeed = 12f
    };

    public MotorSettings Settings => _motorSettings;

    private static readonly int MovingId = Animator.StringToHash("VelocityX");
    private static readonly int GroundedId = Animator.StringToHash("Grounded");
    private static readonly int RisingId = Animator.StringToHash("VelocityY");

    private void Reset()
    {
        _bodyCollider = GetComponent<BoxCollider2D>();
    }

    public MotorBody ReadBody()
    {
        if (_bodyCollider == null)
        {
            Debug.LogError("Missing body collider on " + gameObject.name);
        }

        Vector3 scale = transform.lossyScale;
        Vector2 localSize = _bodyCollider.size;

        return new MotorBody
        {
            Size = new Vector2(localSize.x * Mathf.Abs(scale.x), localSize.y * Mathf.Abs(scale.y)),
            CenterOffset = transform.TransformVector(_bodyCollider.offset)
        };
    }

    public void ApplyPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void SetFacing(bool facingLeft)
    {
        _spriteRenderer.flipX = facingLeft;
    }

    public void ApplyAnimation(Vector2 velocity, bool grounded)
    {
        if (_animator == null)
        {
            Debug.LogError("Missing animator on " + gameObject.name);
        }

        _animator.SetBool(GroundedId, grounded);
        _animator.SetFloat(MovingId, Mathf.Abs(velocity.x));
        _animator.SetFloat(RisingId, velocity.y);
    }
}