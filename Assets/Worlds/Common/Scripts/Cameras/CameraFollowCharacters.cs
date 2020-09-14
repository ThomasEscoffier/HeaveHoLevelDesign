using UnityEngine;

public class CameraFollowCharacters : MonoBehaviour {

    public CameraFollowCharactersTrigger CameraTriggers;
    public float Speed = 2f;
    public float RatioSpeedDistanceMax = 10f;
    public Vector2 MinPos = new Vector2();
    public Vector2 MaxPos = new Vector2();
    float distance = 0f;

    bool isActivated = true;

    void FixedUpdate()
    {
        if (!isActivated)
            return;

        if (CameraTriggers.GetCharacters().Count > 0)
        {
            Vector3 nextPosition = Vector3.zero;
            for (int i = 0; i < CameraTriggers.GetCharacters().Count; ++i)
            {
                if (CameraTriggers.GetCharacters()[i] != null)
                {
                    nextPosition += CameraTriggers.GetCharacters()[i].transform.position;
                }
            }
            nextPosition /= CameraTriggers.GetCharacters().Count;

            Vector3 newPosition = Vector3.Lerp(transform.position, nextPosition, Speed * Time.fixedDeltaTime);
            float x = Mathf.Clamp(newPosition.x, MinPos.x, MaxPos.x);
            float y = Mathf.Clamp(newPosition.y, MinPos.y, MaxPos.y);
            transform.position = new Vector3(x, y, transform.position.z);
            distance = 0f;
        }
        else if (GameManager.Instance.GetNbPlayersAlive() == 0)
        {
            RespawnPoint respawnPoint = GameManager.Instance.GetPlayers()[0].GetRespawnPoints()[0];
            if (respawnPoint != null)
            {
                Vector3 nextPosition = respawnPoint.transform.position;

                if (distance == 0)
                {
                    distance = Vector3.Distance(nextPosition, transform.position);
                }

                Vector3 newPosition = Vector3.Lerp(transform.position, nextPosition, Speed * (Mathf.Lerp(1f, RatioSpeedDistanceMax, Vector3.Distance(nextPosition, transform.position) / distance)) * Time.fixedDeltaTime);
                float x = Mathf.Clamp(newPosition.x, MinPos.x, MaxPos.x);
                float y = Mathf.Clamp(newPosition.y, MinPos.y, MaxPos.y);
                transform.position = new Vector3(x, y, transform.position.z);
            }
        }
        else
        {
            Character toFollow = GetPlayerToFollow();
            if (toFollow != null)
            {
                Vector3 nextPosition = toFollow.transform.position;
                Vector3 newPosition = Vector3.Lerp(transform.position, nextPosition, Speed * Time.fixedDeltaTime);
                float x = Mathf.Clamp(newPosition.x, MinPos.x, MaxPos.x);
                float y = Mathf.Clamp(newPosition.y, MinPos.y, MaxPos.y);
                transform.position = new Vector3(x, y, transform.position.z);
            }
            distance = 0f;
        }
    }

    Character GetPlayerToFollow()
    {
        bool areAllPlayersInsideCamera = true;
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            if (player.GetCurrentCharacter() != null && LevelManager.IsObjectInsideCamera(player.GetCurrentCharacter().gameObject))
            {
                areAllPlayersInsideCamera = false;
                break;
            }
        }
        if (!areAllPlayersInsideCamera)
        {
            return GetClosestCharacter();
        }
        return null;
    }

    Character GetClosestCharacter()
    {
        float closestDistance = float.MaxValue;
        Character closest = null;

        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            Character currentCharacter = player.GetCurrentCharacter();
            if (currentCharacter != null)
            {
                float currentDistance = Vector3.Distance(currentCharacter.transform.position, transform.position);
                if (closest == null || currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closest = currentCharacter;
                }
            }
        }

        return closest;
    }

    public void SetIsActivated(bool state)
    {
        isActivated = state;
    }
}
