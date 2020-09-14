using UnityEngine;

public class RespawnableItemCheckpoint : MonoBehaviour {

    public RespawnableItem.ItemType ItemType = RespawnableItem.ItemType.ANIMAL;
    public Transform posRespawnable = null;

    RespawnableItem currentRespawnableItem = null;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible") && currentRespawnableItem == null)
        {
            RespawnableItem respawnableItem = collision.GetComponent<RespawnableItem>();
            if (respawnableItem != null && respawnableItem.Type == ItemType)
            {
                currentRespawnableItem = respawnableItem;
                Vector3 itemPos = currentRespawnableItem.transform.position;
                currentRespawnableItem.transform.parent.SetPositionAndRotation(posRespawnable.position, posRespawnable.rotation);
                currentRespawnableItem.transform.position = itemPos;
                if (currentRespawnableItem.GetCurrentCheckpoint() != null)
                {
                    currentRespawnableItem.GetCurrentCheckpoint().ResetCheckpoint();
                }
                currentRespawnableItem.SetCurrentCheckpoint(this);
            }
        }
    }

    public void ResetCheckpoint()
    {
        currentRespawnableItem = null;
    }
}
