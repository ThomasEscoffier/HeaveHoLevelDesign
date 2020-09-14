using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Managers;

public class SaveManager : BaseSaveManager
{
    //For future save fixes
    const string currentSaveVersion = "Student";

    [System.Serializable]
    public class WorldTokens
    {
        public string WorldName = "";
        public Dictionary<int, int> NbTokenAcquired = new Dictionary<int, int>();

        public WorldTokens(string worldName, int index, int nbToken)
        {
            WorldName = worldName;
            NbTokenAcquired.Add(index, nbToken);
        }
    }

    [System.Serializable]
    public class SaveAudio
    {
        public float Voices = 1f;
        public float SFX = 1f;
        public float Music = 1f;
    }

    [System.Serializable]
    public class SaveGraphic
    {
        public string FullScreenMode = "ExclusiveFullScreen";
        public bool IsFullScreen = true; //Register actual state of the game, not option
        public int ResolutionWidth = 0;
        public int ResolutionHeight = 0;
        public int DefaultResolutionWidth = 0;
        public int DefaultResolutionHeight = 0;
        public int Monitor = 1;

        public SaveGraphic()
        {
            GraphicSettings.FixUnsupportedResolution(Screen.currentResolution);
            ResolutionWidth = Screen.currentResolution.width;
            ResolutionHeight = Screen.currentResolution.height;
            DefaultResolutionWidth = Screen.currentResolution.width;
            DefaultResolutionHeight = Screen.currentResolution.height;
        }
    }

    [System.Serializable]
    public class NewSaveData
    {
        //For future save fixes
        public string saveVersion = currentSaveVersion;

        public SaveAudio AudioOptions = null;
        public SaveGraphic GraphicOptions = null;

        public List<WorldTokens> TokensByLevels = new List<WorldTokens>();
        public ScoreManager.LevelsBestScores BestScoresCoop = null;
        public ScoreManager.LevelsBestScores BestRunScoresCoop = null;
        public ScoreManager.LevelsBestScores BestScoresSolo = null;
        public ScoreManager.LevelsBestScores BestRunScoresSolo = null;
        public List<string> UnlockedOutfits = new List<string>();
        public int NbTokens = 0;
        public int NbTokenSpent = 0;

        public List<string> DefaultKeyboardMappingXML = new List<string>();
        public List<string> KeyboardMappingXML = new List<string>();

        public NewSaveData()
        {
            AudioOptions = new SaveAudio();
            GraphicOptions = new SaveGraphic();

            BestScoresCoop = new ScoreManager.LevelsBestScores();
            BestRunScoresCoop = new ScoreManager.LevelsBestScores();
            BestScoresSolo = new ScoreManager.LevelsBestScores();
            BestRunScoresSolo = new ScoreManager.LevelsBestScores();

            UnlockedOutfits.Add("");
            foreach (CharacterPreset preset in GameManager.Instance.GetCharactersPresets().CharactersPresets)
            {
                UnlockedOutfits.Add(preset.Name);
            }

            DefaultKeyboardMappingXML = KeyboardMapper.GetKeyboardMapXML();
        }
    }

    public static class SerializerGame
    {
        public static void SaveGameData(NewSaveData bestScores)
        {
            BinaryFormatter binary = new BinaryFormatter();
            string saveFileName = ("/Save/HeaveHoStudent.save");
            FileStream stream = File.Create(Application.persistentDataPath + saveFileName);
            binary.Serialize(stream, bestScores);
            stream.Close();
        }

        public static NewSaveData LoadGameData()
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream stream = null;
            NewSaveData save;

            string saveFileName = ("/Save/HeaveHoStudent.save");

            if (File.Exists(Application.persistentDataPath + saveFileName))
            {
                stream = File.OpenRead(Application.persistentDataPath + saveFileName);
                if (stream == null || stream.Length <= 0)
                {
                    stream.Close();
                    File.Delete(Application.persistentDataPath + saveFileName);
                    save = CreateFile();
                }
                else
                {
                    save = binary.Deserialize(stream) as NewSaveData;
                    if (save.saveVersion != currentSaveVersion)
                    {
                        stream.Close();
                        File.Delete(Application.persistentDataPath + saveFileName);
                        save = CreateFile();
                    }
                }
            }
            else
            {
                save = CreateFile();
            }
            if (stream != null)
            {
                stream.Close();
            }
            return save;
        }

        static NewSaveData CreateFile()
        {
            NewSaveData bestScores;
            if (!Directory.Exists(Application.persistentDataPath + "/Save/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Save/");
            }
            bestScores = new NewSaveData();
            SaveGameData(bestScores);
            return bestScores;
        }
    }

    public override void Load()
    {
        GameSaveData = SerializerGame.LoadGameData();

        FMOD.Studio.Bus musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/MUSIC");
        musicBus.setVolume(GameSaveData.AudioOptions.Music);
        FMOD.Studio.Bus sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/SFX");
        sfxBus.setVolume(GameSaveData.AudioOptions.SFX);
        FMOD.Studio.Bus voicesBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/VOIX");
        voicesBus.setVolume(GameSaveData.AudioOptions.Voices);
    }

    public override void Save()
    {
        SerializerGame.SaveGameData(GameSaveData);
    }
}
