using UnityEngine;
using UnityEngine.Events;

public class Dude : Character, IDamageable
{
    // ********************************************************************************
    // Structs/enums
    private enum States : int
    {
        Roam,
        Punch,
        GetHit,
        Pickup,
        Defeat
    }

    private enum Animations : int
    {
        Idle,
        Walk,
        Punch,
        GetHit,
        Pickup,
        Defeat
    }

    // ********************************************************************************
    // Constants
    const int HealthMaximum = 50;
    const int PunchDamage = 5;
    const float PunchCooldown = 0.25f;
    const float PickupCooldown = 0.25f;
    readonly Vector3 WalkSpeed = new(1.0f, 0.66f);

    // ********************************************************************************
    // Members
    private float _stateCooldown = 0.0f;
    private int _health = HealthMaximum;
    private Vector2 _axisMove = Vector2.zero;
    private Food _pickupFood = null;

    // ********************************************************************************
    // Properties
    private new States State { get { return (States)base.State; } }
    private new Animations Animation { get { return (Animations)base.Animation; } }

    public bool IsAlive { get { return _health > 0; } }

    public BoxCollider2D BodyCollider;
    public BoxCollider2D PunchCollider;
    public GameObject BloodSpray;

    // ********************************************************************************
    // Events
    public UnityEvent<float> HealthChanged;
    public UnityEvent Defeated;

    // ********************************************************************************
    // Unity messages
    protected override void OnStart()
    {
        RegisterState((int)States.Roam, "Roam", Update_StateRoam);
        RegisterState((int)States.Punch, "Punch", Update_StatePunch);
        RegisterState((int)States.GetHit, "GetHit", Update_StateGetHit);
        RegisterState((int)States.Pickup, "GetHit", Update_StatePickup);
        RegisterState((int)States.Defeat, "Defeat", Update_Defeat);

        RegisterAnimation((int)Animations.Idle, "Anim Idle", 0.0f);
        RegisterAnimation((int)Animations.Walk, "Anim Walk", 0.0f);
        RegisterAnimation((int)Animations.Punch, "Anim Punch", (6 / 12.0f) / PunchCooldown);
        RegisterAnimation((int)Animations.GetHit, "Anim GetHit", 0.0f);
        RegisterAnimation((int)Animations.Pickup, "Anim Pickup", (8 / 12.0f) / PickupCooldown);
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
    private void Update_StateRoam()
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

    private void Update_StatePunch()
    {
        PlayAnimation(Animations.Punch);

        if (_stateCooldown <= 0.0f)
        {
            SetState(States.Roam);
        }
    }

    private void Update_StateGetHit()
    {
        PlayAnimation(Animations.GetHit);

        if (_stateCooldown <= 0.0f)
        {
            if (_health <= 0)
            {
                SetState(States.Defeat);
                BodyCollider.enabled = false;
                Defeated?.Invoke();
                _stateCooldown = 3.0f;
            }
            else
            {
                SetState(States.Roam);
            }
        }
    }

    private void Update_StatePickup()
    {
        PlayAnimation(Animations.Pickup);

        if (_stateCooldown <= 0.0f)
        {
            SetState(States.Roam);
        }
    }

    private void Update_Defeat()
    {
        PlayAnimation(Animations.Defeat);
    }

    // ********************************************************************************
    // Input events
    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _axisMove = context.ReadValue<Vector2>();
    }

    public void OnPunch()
    {
        if (State != States.Roam)
            return;

        SetState(States.Punch);
        _stateCooldown = PunchCooldown;
    }

    public void OnPickupFood(Food food)
    {
        if (State != States.Roam)
            return;

        SetState(States.Pickup);
        _stateCooldown = PickupCooldown;
        _pickupFood = food;
    }

    // ********************************************************************************
    // Gameplay messages
    public void Damage(GameObject source, int amount)
    {
        if (
            (State != States.Roam)
            )
            return;

        GameObject spray = Instantiate(BloodSpray, transform);
        spray.transform.localPosition = 0.2f * Vector3.up;
        if (source.transform.position.x > transform.position.x)
            spray.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

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
        targetComponent.Damage(gameObject, PunchDamage);
    }

    public void EventPickup()
    {
        if (_pickupFood == null)
            return;

        ChangeHealth(20);
        Destroy(_pickupFood.gameObject);
        _pickupFood = null;
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

