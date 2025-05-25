using UnityEngine;

public class Food : MonoBehaviour
{
    // ********************************************************************************
    // Constants
    private readonly float Lifetime = 15f;

    // ********************************************************************************
    // Members
    private float _life = 0f;

    // ********************************************************************************
    // Properties
    public Sprite[] FoodSprites;
    public SpriteRenderer Sprite;
    public BoxCollider2D Collider;

    // ********************************************************************************
    // Unity messages
    public void Awake()
    {
        if (FoodSprites.Length > 0)
        {
            int index = Random.Range(0, FoodSprites.Length);
            Sprite.sprite = FoodSprites[index];
        }
    }

    public void Update()
    {
        _life += Time.deltaTime;
        var lifeRemaining = Lifetime - _life;

        if (_life > 0.333f && !Collider.enabled)
            Collider.enabled = true;

        if (lifeRemaining <= 3f)
        {
            var period = Mathf.Repeat(_life * 3f, 1f);

            if (period < 0.5f)
                Sprite.enabled = true;
            else
                Sprite.enabled = false;
        }

        if (lifeRemaining <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Dude>(out var dude))
        {
            dude.OnPickupFood(this);
        }
    }
}
