using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreMetricsDisplayMostDeath : ScoreMetricsDisplay
{
    public ScoreRecapHead CharacterHead = null;
    public Image BackgroundNobodyDied = null;
    public Image Background = null;
    public Text Description = null;
    public Text NobodyDiedWowText = null;
    public Text NobodyDiedText = null;
    public Text NbDeath = null;
    public float TimeBetweenNumbers = 0.2f;

    MetricsManager.PlayerMetrics playerMetrics;
    float timerNumbers = 0f;
    int currentNb = 0;
    int nbToReach = 0;

    SoundModule sound = null;

    void Awake()
    {
        sound = GetComponent<SoundModule>();

        Description.transform.localScale = new Vector3(0f, 0f, 0f);
        NbDeath.transform.localScale = new Vector3(0f, 0f, 0f);
        NobodyDiedText.transform.localScale = new Vector3(0f, 0f, 0f);

        Description.gameObject.SetActive(false);
        NbDeath.gameObject.SetActive(false);
        NobodyDiedText.gameObject.SetActive(false);
        CharacterHead.gameObject.SetActive(false);
    }

    void Start()
    {
        playerMetrics = GameManager.Instance.GetMetricsManager().GetPlayerWithMostDeaths();
        if (playerMetrics == null)
        {
            sound.PlayOneShot("CrowdCheer");
            NobodyDiedText.gameObject.SetActive(true);
            NobodyDiedWowText.gameObject.SetActive(true);
            BackgroundNobodyDied.gameObject.SetActive(true);
            for (int i = 0; i < fireworks.Length; ++i)
            {
                var firework = fireworks[i];
                if (firework != null)
                {
                    firework.StartFireworks();
                }
            }
            step = 3;
        }
        else
        {
            Background.gameObject.SetActive(true);
            CharacterHead.gameObject.SetActive(true);
            CharacterHead.SetOutfit(playerMetrics.CorrespondingPlayer.GetCurrentCharacterOutfit());
            nbToReach = playerMetrics.NbDeath;
            GoToNextStep();
        }
    }

    public override void Update()
    {
        if (step == 2)
        {
            timerNumbers = Mathf.Min(timerNumbers + Time.deltaTime, TimeBetweenNumbers);
            if (timerNumbers == TimeBetweenNumbers)
            {
                currentNb++;
                NbDeath.text = currentNb.ToString();

                timerNumbers = 0f;
                if (currentNb == nbToReach)
                {
                    sound.PlayOneShot("CrowdBoo");
                    GoToNextStep();
                }
            }
        }
        else if (step == 3)
        {
            base.Update();
        }
    }

    public override void GoToNextStep()
    {
        if (step == 0)
        {
            Description.gameObject.SetActive(true);
        }
        else if (step == 1)
        {
            NbDeath.text = "0";
            NbDeath.gameObject.SetActive(true);
            Description.gameObject.SetActive(true);
        }

        base.GoToNextStep();
    }
}
