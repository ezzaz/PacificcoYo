using System.Collections;
using UnityEngine;

public class MotorRepairPuzzle : MonoBehaviour
{
    public static MotorRepairPuzzle Instance;

    [Header("Puzzle Requirements")]
    public SnapZone[] materialZones; // Needs 4 SnapZones for Battery, Cable, Gears, Fuel
    public GameObject brokenMotorVisual;
    public GameObject repairedMotorObject; // This is the motor that becomes grabbable after repair

    [Header("Effects")]
    public ParticleSystem repairParticles;
    public AudioSource soundFeedback;
    public AudioClip repairSuccessSound;

    private bool isRepaired = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (repairedMotorObject != null)
        {
            repairedMotorObject.SetActive(false);
            
            // Set layer to Pickupable (6)
            repairedMotorObject.layer = 6;

            Rigidbody rb = repairedMotorObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; 
                rb.useGravity = false;
            }

            repairedMotorObject.tag = "MotorRepaired";
        }

        if (brokenMotorVisual != null)
        {
            brokenMotorVisual.SetActive(true);
        }
    }

    public void CheckRepairProgress()
    {
        if (isRepaired) return;

        // Verify if all 4 material zones are occupied
        bool allSnapped = true;
        foreach (var zone in materialZones)
        {
            if (zone == null || !zone.ocupado)
            {
                allSnapped = false;
                break;
            }
        }

        if (allSnapped)
        {
            StartCoroutine(RepairSuccessRoutine());
        }
    }

    private IEnumerator RepairSuccessRoutine()
    {
        isRepaired = true;
        Debug.Log("Motor Repair Puzzle SOLVED!");

        // 1. Play beautiful spark feedback
        if (repairParticles != null)
        {
            repairParticles.Play();
        }

        if (soundFeedback != null && repairSuccessSound != null)
        {
            soundFeedback.PlayOneShot(repairSuccessSound);
        }

        // Show a 3D float-aside Edith Finch dialogue about repair
        SpawnDialogueText("¡SÍ! El motor está completamente reparado y listo para ser montado.");

        yield return new WaitForSeconds(1.5f);

        // 2. Transition visuals
        if (brokenMotorVisual != null)
        {
            brokenMotorVisual.SetActive(false);
        }

        if (repairedMotorObject != null)
        {
            repairedMotorObject.SetActive(true);
            
            // Expose as grabbable object
            ObjectGrabbable grabbable = repairedMotorObject.GetComponent<ObjectGrabbable>();
            if (grabbable == null)
            {
                grabbable = repairedMotorObject.AddComponent<ObjectGrabbable>();
            }
            grabbable.enabled = true;
        }

        // Deactivate all snap zone outline visuals for clean view
        foreach (var zone in materialZones)
        {
            if (zone != null)
            {
                // Simple cleanup of children (ghost visuals)
                for (int i = zone.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(zone.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    private void SpawnDialogueText(string msg)
    {
        GameObject textGo = new GameObject("Dialogue3D_Repaired");
        FloatingText3D ft = textGo.AddComponent<FloatingText3D>();
        ft.textToShow = msg;
        ft.followCamera = true; // float on the side of player
        ft.displayDuration = 4.0f;
    }
}