using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Baddie : Character, IDamageable
{
    // ********************************************************************************
    // Structs/enums
    private enum States : int
    {
        Roam,
        Punch,
        GetHit,
        Defeat,
        Taunt
    }

    private enum Animations : int
    {
        Idle,
        Walk,
        Punch,
        GetHit,
        Defeat,
        Taunt
    }

    // ********************************************************************************
    // Constants
    const int HealthMaximumBase = 15;
    const int HealthMaximumPerDifficulty = 2;
    const int PunchDamageBase = 4;
    const int PunchDamagePerDifficulty = 1;
    const float PunchCooldown = 0.25f;

    readonly Vector3 WalkSpeed = new(0.9f, 0.6f);

    // ********************************************************************************
    // Members
    private int _health = HealthMaximumBase;
    private int _healthMaximum = HealthMaximumBase;
    private int _punchDamage = PunchDamageBase;

    private float _stateCooldown = 0.0f;
    private Vector3 _knockbackDirection = Vector3.zero;
    private float _tauntTimer;
    private Vector3 _followOffset;
    private float _attackTimer = 0.0f;

    // ********************************************************************************
    // Properties
    private new States State { get { return (States)base.State; } }
    private new Animations Animation { get { return (Animations)base.Animation; } }

    public BoxCollider2D BodyCollider;
    public BoxCollider2D PunchCollider;
    public HealthBar HealthBar;
    public GameObject BloodSpray;
    public Character Target;

    // ********************************************************************************
    // Events
    public UnityEvent<Baddie> Defeated;

    // ********************************************************************************
    // Unity messages
    protected override void OnStart()
    {
        RegisterState((int)States.Roam, "Roam", Update_StateRoam);
        RegisterState((int)States.Punch, "Punch", Update_StatePunch);
        RegisterState((int)States.GetHit, "GetHit", Update_StateGetHit);
        RegisterState((int)States.Defeat, "Defeat", Update_Defeat);
        RegisterState((int)States.Taunt, "Taunt", Update_Taunt);

        RegisterAnimation((int)Animations.Idle, "Anim Idle", 0.0f);
        RegisterAnimation((int)Animations.Walk, "Anim Walk", 0.0f);
        RegisterAnimation((int)Animations.Punch, "Anim Punch", (6 / 12.0f) / PunchCooldown);
        RegisterAnimation((int)Animations.GetHit, "Anim GetHit", 0.0f);
        RegisterAnimation((int)Animations.Defeat, "Anim Defeat", 0.0f);
        RegisterAnimation((int)Animations.Taunt, "Anim Taunt", 0.0f);

        SpriteRenderer.color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 0.5f), 0.0f);

        if (UnityEngine.Random.value < 0.2)
            _tauntTimer = 0.5f;

        _followOffset.x = UnityEngine.Random.Range(-0.05f, 0.05f);
        _followOffset.y = UnityEngine.Random.Range(-0.05f, 0.05f);

        HealthBar.SetValue(1.0f);
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
    private void Update_StateGetHit()
    {
        transform.position += _knockbackDirection * Time.deltaTime;

        PlayAnimation(Animations.GetHit);

        if (_stateCooldown <= 0.0f)
        {
            if (_health <= 0)
            {
                SetState(States.Defeat);
                BodyCollider.enabled = false;
                Defeated?.Invoke(this);
                _stateCooldown = 3.0f;
            }
            else
            {
                SetState(States.Roam);
            }
        }
    }

    private void Update_StateRoam()
    {
        // Validate target
        UpdateTarget();

        if (Target == null)
        {
            PlayAnimation(Animations.Idle);
            return;
        }

        // Determine movement
        Vector3 moveAxis = Vector2.zero;

        var targetDelta = Target.transform.position - transform.position + _followOffset;

        SpriteRenderer.flipX = targetDelta.x < 0.0f;
        PunchCollider.transform.localPosition = new Vector3(
            Mathf.Sign(targetDelta.x) * Mathf.Abs(PunchCollider.transform.localPosition.x),
            PunchCollider.transform.localPosition.y,
            PunchCollider.transform.localPosition.z
            );

        if (Math.Abs(targetDelta.x) > 0.23)
        {
            moveAxis.x = Math.Sign(targetDelta.x);

        }
        else if (Math.Abs(targetDelta.x) < 0.15)
        {
            moveAxis.x = -Math.Sign(targetDelta.x);
        }

        if (Math.Abs(targetDelta.y) > 0.1)
        {
            moveAxis.y = Math.Sign(targetDelta.y);
        }

        // Count down for attack
        if ((Target == null) || (Math.Abs(targetDelta.x) > 0.3))
        {
            _attackTimer = 0.0f;
        }
        else
        {
            _attackTimer += Time.deltaTime;
        }

        // Perform action
        if (_attackTimer > 1.0f)
        {
            SetState(States.Punch);
            _stateCooldown = PunchCooldown;
            _attackTimer = 0.0f;
        }
        else if (moveAxis.sqrMagnitude > 0.0f)
        {
            Vector3 velocity = new(moveAxis.x * WalkSpeed.x, moveAxis.y * WalkSpeed.y, 0.0f);
            transform.position += velocity * Time.deltaTime;

            PlayAnimation(Animations.Walk);
        }
        else
        {
            PlayAnimation(Animations.Idle);
        }

        // Initiate taunt
        if (_tauntTimer > 0.0f)
        {
            _tauntTimer -= Time.deltaTime;

            if (_tauntTimer <= 0.0f)
            {
                SetState(States.Taunt);
                _stateCooldown = 1.0f;
            }
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

    private void Update_Defeat()
    {
        HealthBar.gameObject.SetActive(false);

        PlayAnimation(Animations.Defeat);

        if (_stateCooldown <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void Update_Taunt()
    {
        PlayAnimation(Animations.Taunt);

        if (_stateCooldown <= 0.0f)
        {
            SetState(States.Roam);
        }
    }

    // ********************************************************************************
    // Gameplay messages
    public void SetDifficulty(int difficulty)
    {
        _healthMaximum = HealthMaximumBase + (difficulty * HealthMaximumPerDifficulty);
        _health = _healthMaximum;

        _punchDamage = PunchDamageBase + (difficulty * PunchDamagePerDifficulty);
    }

    public void Damage(GameObject source, int amount)
    {
        if (
            (State == States.GetHit)
            || (State == States.Defeat)
            )
            return;

        _tauntTimer = 0.0f;

        _health -= amount;
        HealthBar.SetValue((float)_health / _healthMaximum);

        _knockbackDirection.x = 0.5f * (source.transform.position.x < transform.position.x ? 1.0f : -1.0f);

        GameObject spray = Instantiate(BloodSpray, transform);
        spray.transform.localPosition = 0.2f * Vector3.up;
        if (_knockbackDirection.x < 0.0f)
            spray.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        SetState(States.GetHit);
        _stateCooldown = 0.25f;

        if (source.TryGetComponent<Baddie>(out var baddie))
        {
            // Infight
            Target = baddie;
        }
        else if (source.TryGetComponent<Dude>(out var dude))
        {
            Target = dude;
        }
    }

    // ********************************************************************************
    // Animation events
    public void EventPunchConnect()
    {
        var target = Tools.GetClosestDamageable(PunchCollider);

        if (target == null)
            return;

        var targetComponent = target.GetComponent<IDamageable>();
        targetComponent.Damage(gameObject, _punchDamage);
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

    private void UpdateTarget()
    {
        if (Target == null)
        {
            // Nothing to do
            return;
        }

        if (Target.TryGetComponent<Dude>(out var dude))
        {
            // Target is the player
            if (!dude.IsAlive)
            {
                // Player defeated, find a random baddie
                var baddies = FindObjectsByType<Baddie>(FindObjectsSortMode.None).ToList();
                baddies.RemoveAll(b => b == this); // Don't target self
                baddies.RemoveAll(b => b.State == States.Defeat); // Don't target defeated baddies

                if (baddies.Count > 0)
                {
                    Target = baddies[UnityEngine.Random.Range(0, baddies.Count)];
                }
                else
                {
                    // Last survivor
                    Target = null;
                }

                return;
            }
        }
        else if (Target.TryGetComponent<Baddie>(out var baddie))
        {
            // Target is another baddie
            if (baddie.State == States.Defeat)
            {
                // Baddie defeated, reacquire player
                Target = FindObjectOfType<Dude>();
                return;
            }
        }
    }
}
