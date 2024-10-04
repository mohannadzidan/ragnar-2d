using UnityEngine;

public class UIBindings : MonoBehaviour
{

    public LivingEntity player;
    public Bar healthBar;
    // Start is called before the first frame update

    void Awake()
    {
        player.OnHealthChanged += (float health) =>
        {
            healthBar.value = player.health / (float)player.maximumHealth;
        };
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
