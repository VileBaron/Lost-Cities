using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLight : MonoBehaviour {

    private void Update()
    {
        transform.position = Vector3.back * 3;

        if(LostCities.CURRENT_PLAYER == null)
        {
            return;
        }

        transform.position += LostCities.CURRENT_PLAYER.handSlotDef.pos;
    }
}
