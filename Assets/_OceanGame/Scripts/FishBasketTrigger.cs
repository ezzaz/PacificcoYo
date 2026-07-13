using UnityEngine;

public class FishBasketTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.name.Contains("Pescado_Fresco_"))
        {
            // Disable its collider and physics so it rests nicely or is absorbed
            if (other.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
            }
            if (other.TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }

            // Move the fish slightly inside the basket for a neat visual resting position
            other.transform.parent = transform;
            other.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), 0.1f, Random.Range(-0.1f, 0.1f));
            other.transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            // Play a confirmation sound if there is an AudioSource, or log it
            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }

       
        }
    }
}