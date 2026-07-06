using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [Header("Fishing Zone Settings")]
    public float radius = 15f;
    public string zoneName = "Zona de Pesca Abundante";
    public int maxFishes = 4;
    private int fishesCaught = 0;

    public bool RegisterFishCaught()
    {
        fishesCaught++;
        if (fishesCaught >= maxFishes)
        {
            return true; // Depleted!
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}