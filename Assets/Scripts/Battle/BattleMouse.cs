using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "StoreItem":
                BattleManager.Instance.SetSlotStore(collision.gameObject);
                break;

            case "TeamSlot":
                BattleManager.Instance.SetSlotTeam(collision.gameObject);
                break;

            case "EnemySlot":
                BattleManager.Instance.SetSlotEnemy(collision.gameObject);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "StoreItem":
                BattleManager.Instance.SetSlotStore(null);
                break;

            case "TeamSlot":
                BattleManager.Instance.SetSlotTeam(null);
                break;

            case "EnemySlot":
                BattleManager.Instance.SetSlotEnemy(null);
                break;
        }
    }
}
