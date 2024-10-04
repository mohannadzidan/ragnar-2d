# Mob

# Inventory

# Item Drop

- loot is dropped when a mob is killed
- each mob has a drop table, defines each possible item and a probability of being drop
- Loot can be picked up by the player and it adds an item his inventory
- Loot can be expired after 1

# Exp

# Gold

# Shops

# Quests

# Systems

# Gold
Is dropped by the mobs, is used in trading



```mermaid

stateDiagram-v2
    classDef done fill:green,color:white

    state "Levelling and Experience" as xp
    state "Items System" as items
    state "Spawners" as spawners
    state "Loot" as loot
    state "Health Potions" as health_potions
    %% state "Heros" as heros
    state "Mobs" as mobs
    %% state "Gold System" as gold
    state "Health System" as health
    state "Path Finding" as path_finding
    state "Player" as player
    state "Items Stash" as items_stash
    state "Items Drop" as items_drop
    %% state "Items Pickup" as items_pickup
    %% state "Trade System" as items_trade
    state "Crafting System" as items_crafting
    state "Consumables" as consumables
    state "Inventory" as inventory
    state "Inventory/UI" as inventory_ui
    %% state "Currency" as currency
    %% state "Multiplayer" as multiplayer
    state "Inventory/UI/Item Tooltips" as inventory_ui_item_tooltips
    state "Weapons" as weapons
    state "Armor" as armor



    mobs:::done
    health:::done
    player:::done
    items:::done
    path_finding:::done
    loot:::done
    spawners:::done
    inventory:::done
    inventory:::done
    items_stash:::done
    items_pickup:::done
    inventory_ui:::done
    consumables:::done
    health_potions:::done

    items_pickup --> loot
    items_pickup --> inventory
    items_drop --> inventory
    %% as the dropped item will be transformed to loot
    items_drop --> loot 

    items_stash --> items

    loot --> items

    consumables --> inventory

    inventory --> items_stash

    inventory_ui --> inventory
    inventory_ui_item_tooltips --> inventory_ui
    inventory_ui_item_tooltips --> items

    health_potions --> consumables
    health_potions --> health
    health_potions --> loot


    player --> health
    player --> path_finding
    player --> inventory
    player --> xp


    mobs --> health
    mobs --> path_finding
    mobs --> spawners





```
