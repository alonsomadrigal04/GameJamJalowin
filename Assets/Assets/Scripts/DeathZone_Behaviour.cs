using UnityEngine;

public class DeathZone_Behaviour : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NPC_Behaviour>(out var npc)) npc.Death();
    }
}