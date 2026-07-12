using UnityEngine;

public class SnapZone : MonoBehaviour
{
    [Header("Lugar exacto donde quedará la caja")]
    public Transform snapPoint;

    [Header("Tag que debe tener la caja")]
    public string objectTag = "Box";

    public bool ocupado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (ocupado)
            return;

        if (other.CompareTag(objectTag))
        {
            // Colocar en la posición exacta
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Desactivar el script que permite agarrar el objeto
            ObjectGrabbable grabbable = other.GetComponent<ObjectGrabbable>();

            if (grabbable != null)
            {
                grabbable.Drop();      // Por si estaba siendo agarrado
                grabbable.enabled = false;
            }

            ocupado = true;

            PuzzleManager.Instance.ComprobarPuzzle();
        }
    }
}