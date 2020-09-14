namespace Managers
{
    public abstract class BaseSaveManager
    {
        public SaveManager.NewSaveData GameSaveData { get; protected set; }

        public bool HasToSave = false;

        public abstract void Load();
        public abstract void Save();
        
        public void UnlockAllOutfits()
        {
            foreach (CharacterPreset preset in GameManager.Instance.GetCharactersPresets().GetCharacters())
            {
                if (!GameSaveData.UnlockedOutfits.Contains(preset.Name))
                {
                    GameSaveData.UnlockedOutfits.Add(preset.Name);
                }
            }
        }

        public void AddTokenAcquiredFromLevel(string worldName, int levelIndex)
        {
            SaveManager.WorldTokens world = GameSaveData.TokensByLevels.Find(i => i.WorldName == worldName);

            if (world != null)
            {
                if (world.NbTokenAcquired.ContainsKey(levelIndex))
                {
                    world.NbTokenAcquired[levelIndex] += 1;
                }
                else
                {
                    world.NbTokenAcquired.Add(levelIndex, 1);
                }
            }
            else
            {
                GameSaveData.TokensByLevels.Add(new SaveManager.WorldTokens(worldName, levelIndex, 1));
            }
        }

        public bool IsTokenAlreadyAcquired(string worldName, int levelIndex)
        {
            SaveManager.WorldTokens world = GameSaveData.TokensByLevels.Find(i => i.WorldName == worldName);
            return world != null && world.NbTokenAcquired.ContainsKey(levelIndex) && world.NbTokenAcquired[levelIndex] > 0;
        }

        public bool IsTokenAlreadyAcquiredInCurrentLevel()
        {
            return IsTokenAlreadyAcquired(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName, GameManager.Instance.GetLevelSelector().GetCurrentStep() - 1);
        }

        public virtual void ReconcileSaveWithSteam() { }
    }
}