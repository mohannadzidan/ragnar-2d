using UnityEngine;

[RequireComponent(typeof(LivingEntity))]
public class DropLootOnDeath : MonoBehaviour
{

    public LootTable table;
    public GameObject lootPrefab;
    LivingEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<LivingEntity>();
        entity.OnDeath += Drop;

    }


    void Drop()
    {
        foreach (var record in table.rates)
        {
            if (Random.value < record.rate)
            {
                GameManager.current.CreateLoot(transform.position, record.item, 1);
            }
        }

    }
}
