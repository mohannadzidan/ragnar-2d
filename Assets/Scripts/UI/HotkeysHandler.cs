using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeysHandler : MonoBehaviour
{

    public KeyCode toggleInventory = KeyCode.I;
    
    GameObject inventory;
    // Start is called before the first frame update
    void Start()
    {

        inventory = transform.Find("Inventory")?.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toggleInventory)){
            inventory.SetActive(!inventory.activeSelf);
        }
        
    }
}
