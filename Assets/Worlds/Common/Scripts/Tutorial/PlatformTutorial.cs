using UnityEngine;

public class PlatformTutorial : MonoBehaviour {

    public GameObject[] OKSigns;
    public GameObject ToDeactivateWhenStarted = null;

    protected Tutorial tutorial;

    CharacterPresetTryOn[] models;

    bool isCheckingNbPlayersHolding = false;
    int nbPlayers = 0;

    SoundModule soundModule = null;

    private void Awake()
    {
        soundModule = GetComponent<SoundModule>();
        models = FindObjectsOfType<CharacterPresetTryOn>();
    }

    public virtual void Update()
    {
		if (isCheckingNbPlayersHolding)
        {
            bool allPlayersGrabbing = true;
            for (int i = 0; i < models.Length; ++i)
            {
                if (!models[i].gameObject.activeSelf)
                    continue;

                if (models[i].GetLeftHand().GetIsHooked() && models[i].GetRightHand().GetIsHooked() && !OKSigns[GameManager.Instance.GetPlayerFromId(models[i].PlayerId).OrderInGame].activeSelf)
                {
                    OKSigns[GameManager.Instance.GetPlayerFromId(models[i].PlayerId).OrderInGame].SetActive(true);
                    soundModule.PlayOneShot("OK");
                }
                else if (!models[i].GetLeftHand().GetIsHooked() || !models[i].GetRightHand().GetIsHooked())
                {
                    allPlayersGrabbing = false;
                    if (OKSigns[GameManager.Instance.GetPlayerFromId(models[i].PlayerId).OrderInGame].activeSelf)
                    {
                        OKSigns[GameManager.Instance.GetPlayerFromId(models[i].PlayerId).OrderInGame].SetActive(false);
                    }
                }
            }

            if (allPlayersGrabbing)
            {
                foreach (CharacterPresetTryOn model in models)
                {
                    model.SetIsControlBlocked(true);
                }
                foreach (GameObject ok in OKSigns)
                {
                    ok.SetActive(false);
                }
                isCheckingNbPlayersHolding = false;
                tutorial.MoveCharactersUp();
            }
        }
	}

    public void SetTutorial(Tutorial tuto)
    {
        tutorial = tuto;
    }

    public void StartCheckingPlayers()
    {
        isCheckingNbPlayersHolding = true;
        nbPlayers = GameManager.Instance.GetPlayers().Count;

        if (ToDeactivateWhenStarted != null)
        {
            ToDeactivateWhenStarted.SetActive(false);
        }
    }

    public bool GetIsCheckingPlayers()
    {
        return isCheckingNbPlayersHolding;
    }
}
