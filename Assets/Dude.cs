using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Dude : Character, IDamageable
{
    // ********************************************************************************
    // Structs/enums
    private enum States : int
    {
        Roam,
        Punch,
        Kick,
        GetHit,
        Defeat
    }

    private enum Animations : int
    {
        Idle,
        Walk,
        Punch,
        Kick,
        GetHit,
        Defeat
    }

    // ********************************************************************************
    // Constants
    const int HealthMaximum = 50;
    readonly Vector3 WalkSpeed = new(1.0f, 0.66f);
    readonly float PunchCooldown = 0.25f;
    readonly float KickCooldown = 0.5f;

    // ********************************************************************************
    // Members
    private float _stateCooldown = 0.0f;
    private int _health = HealthMaximum;
    private Vector2 _axisMove = Vector2.zero;

    // ********************************************************************************
    // Properties
    private new States State { get { return (States)base.State; } }
    private new Animations Animation { get { return (Animations)base.Animation; } }

    public bool IsAlive { get { return _health > 0; } }

    public BoxCollider2D PunchCollider;
    public BoxCollider2D KickCollider;

    // ********************************************************************************
    // Events
    public UnityEvent<float> HealthChanged;
    public UnityEvent Defeated;

    // ********************************************************************************
    // Unity messages
    protected override void OnStart()
    {
        //foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        //{
        //    //Debug.Log(clip.name);
        //}

        RegisterState((int)States.Roam, "Roam", Update_StateRoam);
        RegisterState((int)States.Punch, "Punch", Update_StatePunch);
        RegisterState((int)States.Kick, "Kick", Update_StateKick);
        RegisterState((int)States.GetHit, "GetHit", Update_StateGetHit);
        RegisterState((int)States.Defeat, "Defeat", Update_Defeat);

        RegisterAnimation((int)Animations.Idle, "Anim Idle", 0.0f);
        RegisterAnimation((int)Animations.Walk, "Anim Walk", 0.0f);
        RegisterAnimation((int)Animations.Punch, "Anim Punch", (6 / 12.0f) / PunchCooldown);
        RegisterAnimation((int)Animations.Kick, "Anim Kick", (6 / 12.0f) / KickCooldown);
        RegisterAnimation((int)Animations.GetHit, "Anim GetHit", 0.0f);
        RegisterAnimation((int)Animations.Defeat, "Anim Defeat", 0.0f);

        SetState(States.Roam);
    }

    protected override void OnUpdate()
    {
        _stateCooldown -= Time.deltaTime;

        if (_stateCooldown <= 0.0f)
            _stateCooldown = 0.0f;
    }

    // ********************************************************************************
    // State handlers
    void Update_StateRoam()
    {
        if (_axisMove.sqrMagnitude > 0.0f)
        {
            Vector3 velocity = new(_axisMove.x * WalkSpeed.x, _axisMove.y * WalkSpeed.y, 0.0f);
            transform.position += velocity * Time.deltaTime;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -1.3f, 1.3f),
                Mathf.Clamp(transform.position.y, -0.8f, 0.0f),
                transform.position.z
                );

            if (_axisMove.x < -0.01)
            {
                SpriteRenderer.flipX = true;

                PunchCollider.transform.localPosition = new Vector3(
                    -Mathf.Abs(PunchCollider.transform.localPosition.x),
                    PunchCollider.transform.localPosition.y,
                    PunchCollider.transform.localPosition.z
                    );
            }
            else if (_axisMove.x > 0.01)
            {
                SpriteRenderer.flipX = false;

                PunchCollider.transform.localPosition = new Vector3(
                    Mathf.Abs(PunchCollider.transform.localPosition.x),
                    PunchCollider.transform.localPosition.y,
                    PunchCollider.transform.localPosition.z
                    );
            }

            PlayAnimation(Animations.Walk);
        }
        else
        {
            PlayAnimation(Animations.Idle);
        }
    }

    void Update_StatePunch()
    {
        PlayAnimation(Animations.Punch);

        if (_stateCooldown <= 0.0f)
        {
            SetState(States.Roam);
        }
    }

    void Update_StateKick()
    {
        PlayAnimation(Animations.Kick);

        if (_stateCooldown <= 0.0f)
        {
            SetState(States.Roam);
        }
    }

    void Update_StateGetHit()
    {
        PlayAnimation(Animations.GetHit);

        if (_stateCooldown <= 0.0f)
        {
            if (_health <= 0)
            {
                SetState(States.Defeat);
                Defeated?.Invoke();
                _stateCooldown = 3.0f;
            }
            else
            {
                SetState(States.Roam);
            }
        }
    }

    private void Update_Defeat()
    {
        PlayAnimation(Animations.Defeat);
    }

    // ********************************************************************************
    // Input events
    public void OnMove(InputValue value)
    {
        _axisMove = value.Get<Vector2>();
    }

    public void OnPunch()
    {
        if (State != States.Roam)
            return;

        SetState(States.Punch);
        _stateCooldown = PunchCooldown;
    }

    public void OnKick()
    {
        if (State != States.Roam)
            return;

        SetState(States.Kick);
        _stateCooldown = KickCooldown;
    }

    // ********************************************************************************
    // Gameplay messages
    public void Damage(GameObject source, int amount)
    {
        if (
            (State != States.Roam)
            )
            return;

        ChangeHealth(-amount);

        SetState(States.GetHit);
        _stateCooldown = 0.25f;
    }

    // ********************************************************************************
    // Animation events
    public void EventPunchConnect()
    {
        var target = Tools.GetClosestDamageable(PunchCollider);

        if (target == null)
            return;

        var targetComponent = target.GetComponent<IDamageable>();
        targetComponent.Damage(gameObject, 4);
    }

    public void EventKickConnect()
    {
        var target = Tools.GetClosestDamageable(PunchCollider);

        if (target == null)
            return;

        var targetComponent = target.GetComponent<IDamageable>();
        targetComponent.Damage(gameObject, 6);
    }

    // ********************************************************************************
    // Utilities
    private void SetState(States state)
    {
        SetState((int)state);
    }

    private void PlayAnimation(Animations animation)
    {
        PlayAnimation((int)animation);
    }

    private void ChangeHealth(int value)
    {
        _health = Mathf.Clamp(_health + value, 0, HealthMaximum);
        HealthChanged?.Invoke((float)_health / HealthMaximum);
    }
}

