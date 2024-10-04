using Ragnar.Util;
using UnityEngine;

public class Hud : Singleton<Hud>
{

    LivingEntity player;
    public Bar healthBar
    {
        get;
        private set;
    }
    public Toaster toaster
    {
        get;
        private set;
    }
    // Start is called before the first frame update

    void Awake()
    {

        toaster = GameObject.Find("Toaster").GetComponent<Toaster>();
        player = GameObject.Find("Player").GetComponent<LivingEntity>();
        healthBar = transform.Find("HealthBar").GetComponent<Bar>();
        player.OnHealthChanged += (float health) =>
        {
            healthBar.value = player.health / (float)player.maximumHealth;
        };
    }

    public static Hud Instance()
    {
        return GameObject.Find("HUD")?.GetComponent<Hud>();
    }
}
