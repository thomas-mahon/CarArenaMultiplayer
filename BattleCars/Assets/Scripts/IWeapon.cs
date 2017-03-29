using UnityEngine;
using System.Collections;




public interface IWeapon{
    bool IsActive{get;}
    int Damage{get;}
    float LifeSpan{get;}
    IEnumerator DestroySelf(float LifeSpan);
}
