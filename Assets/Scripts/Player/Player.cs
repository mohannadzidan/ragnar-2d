using Type = System.Type;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(IsoCharacterAnimator))]
[RequireComponent(typeof(Navigator))]
[RequireComponent(typeof(PlayerInventory))]
[DisallowMultipleComponent()]
public class Player : MonoBehaviour
{

    public LivingEntity entity
    {
        get;
        private set;
    }
    Navigator navigator;
    IsoCharacterAnimator isoAnim;
    LivingEntity lockedAttackEntity;
    Loot lockedLoot;
    PlayerInventory inventory;
    Toaster toaster;

    void Start()
    {
        // Get reference to CharacterAnimator component
        navigator = GetComponent<Navigator>();
        inventory = GetComponent<PlayerInventory>();
        isoAnim = GetComponent<IsoCharacterAnimator>();
        entity = GetComponent<LivingEntity>();
        toaster = Toaster.FindInScene();
        entity.OnDeath += () => Destroy(this);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            lockedAttackEntity = null;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, 1500f);
            var go = hit.collider?.gameObject;
            if (go != null)
            {
                Interact(go);
            }
            else
            {
                navigator.destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                AbortInteractions();
            }
        }
        else if (lockedAttackEntity != null)
        {
            navigator.GoTo(lockedAttackEntity.transform.position, 1, 1);
            if (navigator.reached)
            {
                if (!lockedAttackEntity.isDead)
                {

                    isoAnim.Attack();
                    isoAnim.LookAt(lockedAttackEntity.transform.position);
                }
                else
                {
                    lockedAttackEntity = null;
                    isoAnim.AbortAttack();
                }
            }

        }
        else if (lockedLoot != null)
        {
            navigator.GoTo(lockedLoot.transform.position, 1, 0);
            if (navigator.reached)
            {
                var maxFit = inventory.remainingWeight / lockedLoot.item.weight;
                Debug.Log($"{inventory.capacity} {inventory.totalWeight} :: {inventory.remainingWeight} / {lockedLoot.item.weight} = {maxFit}");
                if (maxFit > 0)
                {
                    foreach (Item item in lockedLoot.Pickup(maxFit))
                    {
                        Debug.Log("Took item w="+item.weight);
                        inventory.Put(item);
                    }
                }
                else
                {
                    toaster.Toast("Your inventory is full!");
                }
                lockedLoot = null;
            }
        }

    }


    void AbortInteractions()
    {
        lockedLoot = null;
        lockedAttackEntity = null;

    }
    void StartPickup(Loot lootable)
    {
        lockedLoot = lootable;
    }

    void StartAttacking(LivingEntity entity)
    {
        if (entity != null && entity != this && !entity.isDead)
        {
            lockedAttackEntity = entity;
        }
    }

    void Interact(GameObject go)
    {
        AbortInteractions();
        switch (go.tag)
        {
            case "Enemy":
                StartAttacking(go.GetComponent<LivingEntity>());
                break;
            case "Loot":
                StartPickup(go.GetComponent<Loot>());
                break;

        }
    }

    void AnimationEventHandler(string eventName)
    {
        Debug.Log($"eventName {eventName}");
        switch (eventName)
        {
            case "Attack":
                lockedAttackEntity?.Hit(Random.Range(18, 25));
                return;
            default: return;
        }
    }

    public static LivingEntity Entity()
    {
        return GameObject.Find("Player").GetComponent<LivingEntity>();
    }

    public bool Use(Item item)
    {
        Type type = item.GetType();
        if (type == typeof(Potion))
        {
            var potion = (Potion)item;
            return entity.Heal(potion.points);
        }
        else
        {
            return false;
        }
        return true;

    }
}
