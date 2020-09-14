using UnityEngine;

public class BackgroundManager : MonoBehaviour {

    public GameObject[] BackgroundPrefabs = null;
    protected SpriteRenderer[] currentBackgrounds = new SpriteRenderer[9];
    SpriteRenderer[] oldBackgrounds = new SpriteRenderer[9];
   // protected Camera currentCam = null;

    Camera cachedMainCamera;

    public virtual void Awake()
    {
        
        cachedMainCamera = Camera.main;
        
        currentBackgrounds[4] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(cachedMainCamera.transform.position.x, cachedMainCamera.transform.position.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpawnNewBackgrounds();
    }

    public virtual void Update()
    {
        for (int i = 0; i < currentBackgrounds.Length; ++i)
        {
            if (i % 2 != 0 && IsInsideBackground(i, cachedMainCamera.WorldToScreenPoint(cachedMainCamera.transform.position), currentBackgrounds[i].bounds))
            {
                GetNeededPreviousBackgrounds(i);
                SpawnNewBackgrounds();
                break;
            }
        }
	}

    void GetNeededPreviousBackgrounds(int index)
    {
        oldBackgrounds = currentBackgrounds;
        currentBackgrounds = new SpriteRenderer[9];

        int newIndex = 0;
        for (int i = -4; i < 5; ++i)
        {
            int oldRow = newIndex / 3;
            int oldColumn = newIndex % 3;
            int row = (index + i) / 3;
            int column = (index + i) % 3;
            if (index + i > -1 && index + i < 9 && row > -1 && row < 3 && column > -1 && column < 3 && (oldRow == row || oldColumn == column))
            {
                currentBackgrounds[newIndex] = oldBackgrounds[index + i];
                oldBackgrounds[index + i] = null;
            }
            newIndex++;
        }

        for (int i = 0; i < oldBackgrounds.Length; ++i)
        {
            if (oldBackgrounds[i] != null)
            {
                Destroy(oldBackgrounds[i].transform.parent.gameObject);
            }
        }
    }

    bool IsInsideBackground(int backgroundIndex, Vector3 positionInScreen, Bounds bounds)
    {
        Vector3 origin = cachedMainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0f));
        Vector3 extent = cachedMainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, 0f));

        switch (backgroundIndex)
        {
            case 1:
                return positionInScreen.y > origin.y;
            case 3:
                return positionInScreen.x < extent.x;
            case 5:
                return positionInScreen.x > origin.x;
            case 7:
                return positionInScreen.y < extent.y;
            default:
                return false;
        }
    }

    void SpawnNewBackgrounds()
    {
        for (int i = 0; i < currentBackgrounds.Length; ++i)
        {
            if (currentBackgrounds[i] == null)
            {
                switch (i)
                {
                    case 0:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x - currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y + currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 1:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x, currentBackgrounds[4].transform.parent.position.y + currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 2:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x + currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y + currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 3:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x - currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 5:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x + currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 6:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x - currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y - currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 7:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x, currentBackgrounds[4].transform.parent.position.y - currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case 8:
                        currentBackgrounds[i] = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Length)], new Vector3(currentBackgrounds[4].transform.parent.position.x + currentBackgrounds[4].bounds.size.x, currentBackgrounds[4].transform.parent.position.y - currentBackgrounds[4].bounds.size.y, 0f), Quaternion.identity, transform).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                }
            }
        }
    }
}
