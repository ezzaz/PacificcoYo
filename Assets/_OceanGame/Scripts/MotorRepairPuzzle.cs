using System.Collections;
using UnityEngine;

public class MotorRepairPuzzle : MonoBehaviour
{
    public static MotorRepairPuzzle Instance;

    [Header("Puzzle Requirements")]
    public SnapZone[] materialZones;
    public GameObject brokenMotorVisual;
    public GameObject repairedMotorObject; 

    [Header("Effects")]
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


        if (soundFeedback != null && repairSuccessSound != null)
        {
            soundFeedback.PlayOneShot(repairSuccessSound);
        }

        SpawnDialogueText("¡SÍ! El motor está completamente reparado y listo para ser montado.");

        yield return new WaitForSeconds(1.5f);

        if (brokenMotorVisual != null)
        {
            brokenMotorVisual.SetActive(false);
        }

        if (repairedMotorObject != null)
        {
            repairedMotorObject.SetActive(true);
            
            ObjectGrabbable grabbable = repairedMotorObject.GetComponent<ObjectGrabbable>();
            if (grabbable == null)
            {
                grabbable = repairedMotorObject.AddComponent<ObjectGrabbable>();
            }
            grabbable.enabled = true;
        }

        foreach (var zone in materialZones)
        {
            if (zone != null)
            {
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
        ft.displayDuration = 4.0f;
    }
}