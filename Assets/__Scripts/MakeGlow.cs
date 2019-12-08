using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGlow : MonoBehaviour
{
    void Start()
    {
        Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
        halo.enabled = false;
    }
    
   
    void OnMouseOver()
    {
        Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
        halo.enabled = true;
    }

    void OnMouseExit()
    {
        Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
        halo.enabled = false;
    }
}
