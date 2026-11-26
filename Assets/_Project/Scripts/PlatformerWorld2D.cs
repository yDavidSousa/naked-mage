using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlatformerWorld2D : MonoBehaviour
{
    [System.Serializable]
    private struct OneWayPlatformSource
    {
        public BoxCollider2D Collider;
        public Vector2 Travel;
        [Min(0f)] public float Speed;
    }

    private struct OneWayPlatform
    {
        public Vector3 StartPosition;
        public float Left;
        public float Right;
        public float Top;
        public Vector2 Offset;
        public Vector2 Delta;
    }

    private struct MotorCommand
    {
        public float MoveX;
        public bool JumpPressed;
    }

    private struct MotorState
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool Grounded;
        public bool FacingLeft;
        public int SupportPlatformIndex;
    }

    private const float Skin = 0.01f;

    [SerializeField] private OneWayPlatformSource[] _oneWaySources;
    [SerializeField] private PlatformerCharacterView[] _views;
    [SerializeField] private LayerMask _solidMask;

    private MotorCommand[] _commands;
    private MotorState[] _states;
    private MotorBody[] _bodies;
    private MotorSettings[] _settings;

    private OneWayPlatform[] _oneWayPlatforms;
    private float _platformTime;

    private void Awake()
    {
        _commands = new MotorCommand[_views.Length];
        _states = new MotorState[_views.Length];
        _bodies = new MotorBody[_views.Length];
        _settings = new MotorSettings[_views.Length];

        for (int i = 0; i < _views.Length; i++)
        {
            _states[i].Position = _views[i].transform.position;
            _states[i].SupportPlatformIndex = -1;
            _bodies[i] = _views[i].ReadBody();
            _settings[i] = _views[i].Settings;
        }

        _oneWayPlatforms = new OneWayPlatform[_oneWaySources.Length];
        for (int i = 0; i < _oneWayPlatforms.Length; i++)
        {
            BoxCollider2D collider = _oneWaySources[i].Collider;
            Bounds bounds = collider.bounds;

            _oneWayPlatforms[i] = new OneWayPlatform
            {
                StartPosition = collider.transform.position,
                Left = bounds.min.x,
                Right = bounds.max.x,
                Top = bounds.max.y
            };
        }
    }

    private void Update()
    {
        if (_commands.Length == 0)
            return;

        Keyboard keyboard = Keyboard.current;
        float moveX = 0f;

        if (keyboard != null)
        {
            if (keyboard.spaceKey.wasPressedThisFrame)
                _commands[0].JumpPressed = true;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                moveX -= 1f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                moveX += 1f;
        }

        _commands[0].MoveX = moveX;
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;

        StepPlatforms(fixedDeltaTime);

        for (int i = 0; i < _states.Length; i++)
        {
            MotorCommand command = _commands[i];
            _commands[i].JumpPressed = false;

            StepMotor(ref _states[i], command, _settings[i], _bodies[i], _solidMask, _oneWayPlatforms, fixedDeltaTime);
        }

        ApplyPlatformPositions();
        for (int i = 0; i < _states.Length; i++)
        {
            _views[i].ApplyPosition(_states[i].Position);
            _views[i].SetFacing(_states[i].FacingLeft);
            _views[i].ApplyAnimation(_states[i].Velocity, _states[i].Grounded);
        }
    }

    private void LateUpdate()
    {
        if (_states.Length == 0)
            return;

        MotorState player = _states[0];
        int support = player.SupportPlatformIndex;

        if (support < 0)
            return;

        OneWayPlatform platform = _oneWayPlatforms[support];

        float platformSimX = platform.StartPosition.x + platform.Offset.x;
        float playerSceneX = _views[0].transform.position.x;
        float platformSceneX = _oneWaySources[support].Collider.transform.position.x;

        Debug.Log(
            $"Frame {Time.frameCount} | " +
            $"distância simulada: {player.Position.x - platformSimX:F4} | " +
            $"erro mago: {playerSceneX - player.Position.x:F4} | " +
            $"erro plataforma: {platformSceneX - platformSimX:F4} | " +
            $"delta plataforma: {platform.Delta.x:F4}");
    }

    private void ApplyPlatformPositions()
    {
        for (int i = 0; i < _oneWayPlatforms.Length; i++)
        {
            OneWayPlatform platform = _oneWayPlatforms[i];

            _oneWaySources[i].Collider.transform.position = platform.StartPosition + (Vector3)platform.Offset;
        }
    }

    private void StepPlatforms(float deltaTime)
    {
        _platformTime += deltaTime;

        for (int i = 0; i < _oneWayPlatforms.Length; i++)
        {
            OneWayPlatformSource source = _oneWaySources[i];
            ref OneWayPlatform platform = ref _oneWayPlatforms[i];

            float length = source.Travel.magnitude;
            Vector2 nextOffset = Vector2.zero;

            if (length > 0f && source.Speed > 0f)
            {
                float distance = Mathf.PingPong(_platformTime * source.Speed, length);
                nextOffset = source.Travel * (distance / length);
            }

            platform.Delta = nextOffset - platform.Offset;
            platform.Offset = nextOffset;
        }
    }

    private static void StepMotor(ref MotorState state, MotorCommand command, MotorSettings settings, MotorBody body, LayerMask solidMask, OneWayPlatform[] oneWayPlatforms, float deltaTime)
    {
        if (command.MoveX != 0f)
        {
            state.FacingLeft = command.MoveX < 0f;
        }

        float targetSpeed = command.MoveX * settings.MaxSpeed;
        bool reversing = state.Velocity.x * targetSpeed < 0f;
        if (settings.Acceleration <= 0f || reversing)
        {
            state.Velocity.x = targetSpeed;
        }
        else
        {
            state.Velocity.x = Mathf.MoveTowards(state.Velocity.x, targetSpeed, settings.Acceleration * deltaTime);
        }

        bool jumping = command.JumpPressed && state.Grounded;
        if (jumping)
        {
            state.Velocity.y = settings.JumpSpeed;
        }
        else
        {
            state.Velocity.y = Mathf.Max(state.Velocity.y - settings.Gravity * deltaTime, -settings.MaxFallSpeed);
        }

        Vector2 startPosition = state.Position;
        int previousSupport = state.SupportPlatformIndex;

        if (!jumping && previousSupport >= 0)
        {
            Vector2 carry = oneWayPlatforms[previousSupport].Delta;

            MoveAxis(ref state.Position, carry.x, Vector2.right, body, solidMask);
            MoveAxis(ref state.Position, carry.y, Vector2.up, body, solidMask);
        }

        if (MoveAxis(ref state.Position, state.Velocity.x * deltaTime, Vector2.right, body, solidMask))
        {
            state.Velocity.x = 0f;
        }

        float moveY = state.Velocity.y * deltaTime;
        Vector2 positionAfterX = state.Position;

        bool hitSolid = MoveAxis(ref state.Position, moveY, Vector2.up, body, solidMask);
        bool groundedOnSolid = hitSolid && moveY < 0f;
        int supportIndex = FindOneWaySupport(startPosition, positionAfterX, ref state.Position, body, solidMask, oneWayPlatforms, jumping ? previousSupport : -1, groundedOnSolid);

        state.SupportPlatformIndex = supportIndex;
        state.Grounded = groundedOnSolid || supportIndex >= 0;

        if (hitSolid || supportIndex >= 0)
        {
            state.Velocity.y = 0f;
        }
    }

    private static bool MoveAxis(ref Vector2 position, float displacement, Vector2 axis, MotorBody body, LayerMask solidMask)
    {
        if (displacement == 0f)
        {
            return false;
        }

        Vector2 direction = axis * Mathf.Sign(displacement);
        float wantedDistance = Mathf.Abs(displacement);

        Vector2 castCenter = position + body.CenterOffset;
        RaycastHit2D hit = Physics2D.BoxCast(castCenter, body.Size, 0f, direction, wantedDistance + Skin, solidMask);

        float allowedDistance = wantedDistance;
        bool blocked = false;

        if (hit.collider != null)
        {
            float distanceBeforeObstacle = Mathf.Max(0f, hit.distance - Skin);
            allowedDistance = Mathf.Min(wantedDistance, distanceBeforeObstacle);
            blocked = distanceBeforeObstacle <= wantedDistance;
        }

        position += direction * allowedDistance;
        return blocked;
    }

    private static int FindOneWaySupport(
        Vector2 startPosition,
        Vector2 positionAfterX,
        ref Vector2 resolvedPosition,
        MotorBody body,
        LayerMask solidMask,
        OneWayPlatform[] platforms,
        int ignoredPlatform,
        bool groundedOnSolid)
    {
        const float tolerance = 0.001f;

        float feetOffset = body.CenterOffset.y - body.Size.y * 0.5f;
        float halfWidth = body.Size.x * 0.5f;

        float startFeet = startPosition.y + feetOffset;
        float endFeet = resolvedPosition.y + feetOffset;
        float startCenterX = startPosition.x + body.CenterOffset.x;
        float endCenterX = resolvedPosition.x + body.CenterOffset.x;

        int supportIndex = -1;
        float highestTop = float.NegativeInfinity;
        Vector2 landingPosition = resolvedPosition;

        for (int i = 0; i < platforms.Length; i++)
        {
            if (i == ignoredPlatform)
            {
                continue;
            }

            OneWayPlatform platform = platforms[i];
            Vector2 previousOffset = platform.Offset - platform.Delta;

            float oldTop = platform.Top + previousOffset.y;
            float newTop = platform.Top + platform.Offset.y;

            if (newTop <= highestTop || startFeet < oldTop - tolerance)
            {
                continue;
            }

            float startGap = startFeet - oldTop - Skin;
            float endGap = endFeet - newTop - Skin;
            float closingDistance = startGap - endGap;

            if (endGap > tolerance || closingDistance <= 0f)
            {
                continue;
            }

            if (groundedOnSolid && endGap >= -tolerance)
            {
                continue;
            }

            float contactTime = Mathf.Clamp01(startGap / closingDistance);
            float contactCenterX = Mathf.Lerp(startCenterX, endCenterX, contactTime);
            float contactOffsetX = Mathf.Lerp(previousOffset.x, platform.Offset.x, contactTime);

            if (contactCenterX + halfWidth <= platform.Left + contactOffsetX ||
                contactCenterX - halfWidth >= platform.Right + contactOffsetX)
            {
                continue;
            }

            if (endCenterX + halfWidth <= platform.Left + platform.Offset.x ||
                endCenterX - halfWidth >= platform.Right + platform.Offset.x)
            {
                continue;
            }

            float targetY = newTop + Skin - feetOffset;
            Vector2 candidate = positionAfterX;

            MoveAxis( ref candidate, targetY - candidate.y, Vector2.up, body, solidMask);

            if (Mathf.Abs(candidate.y - targetY) > tolerance)
            {
                continue;
            }

            supportIndex = i;
            highestTop = newTop;
            landingPosition = candidate;
        }

        resolvedPosition = landingPosition;
        return supportIndex;
    }
}