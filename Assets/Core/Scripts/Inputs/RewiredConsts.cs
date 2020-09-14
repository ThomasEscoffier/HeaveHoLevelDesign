// <auto-generated>
// Rewired Constants
// This list was generated on 7/2/2019 5:55:16 PM
// The list applies to only the Rewired Input Manager from which it was generated.
// If you use a different Rewired Input Manager, you will have to generate a new list.
// If you make changes to the exported items in the Rewired Input Manager, you will
// need to regenerate this list.
// </auto-generated>

namespace RewiredConsts {
    public static partial class Action {
        // Default
        // Character
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Axis for left arms movements")]
        public const int Character_LeftArmMovementX = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Axis for left arms movements")]
        public const int Character_LeftArmMovementY = 4;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Axis for right arm movements")]
        public const int Character_RightArmMovementX = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Axis for right arm movements")]
        public const int Character_RightArmMovementY = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Left hand grabbing")]
        public const int Character_GrabLeft = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Right hand grabbing")]
        public const int Character_GrabRight = 3;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Resets the character")]
        public const int Character_Reset = 6;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "PointLeft")]
        public const int Character_PointLeft = 8;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "PointRight")]
        public const int Character_PointRight = 9;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "EnterGame")]
        public const int Character_EnterGame = 28;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Random Jump")]
        public const int Character_Jump = 31;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Get holding characters to release the player")]
        public const int Character_Impulse = 33;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Pause")]
        public const int Character_Pause = 35;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Angry face")]
        public const int Character_Angry = 37;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Character", friendlyName = "Joy face")]
        public const int Character_Joy = 38;
        // Cheats
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "NextLevel")]
        public const int Cheats_NextLevel = 7;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "ReloadLevel")]
        public const int Cheats_ReloadLevel = 10;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "Add new player in game without controller assigned")]
        public const int Cheats_AddPlayer = 17;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "ActivateDebugMode")]
        public const int Cheats_ActivateDebugMode = 18;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "ActivateShowChains")]
        public const int Cheats_ActivateShowChains = 26;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "ShowFindLevel")]
        public const int Cheats_ShowFindLevel = 27;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "UnlockEverything")]
        public const int Cheats_UnlockEverything = 29;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "Add collectible to unlock next outfit")]
        public const int Cheats_ObtainCollectible = 30;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "Make mini game rope appear")]
        public const int Cheats_StartRopeMiniGame = 34;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Cheats", friendlyName = "StartRandomEvent")]
        public const int Cheats_StartRandomEvent = 36;
        // Menu
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "NavHorizontal")]
        public const int Menu_NavHorizontal = 11;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "NavVertical")]
        public const int Menu_NavVertical = 12;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Confirm")]
        public const int Menu_Confirm = 13;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Cancel")]
        public const int Menu_Cancel = 14;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Special")]
        public const int Menu_Special = 15;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Skip")]
        public const int Menu_Skip = 16;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Remove element from character")]
        public const int Menu_Remove = 32;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Switch join L")]
        public const int Menu_JoinL = 39;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Menu", friendlyName = "Switch join R")]
        public const int Menu_JoinR = 40;
        // PlayerCheats
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "PlayerCheats", friendlyName = "ChangeControlPlayer")]
        public const int PlayerCheats_ChangeControlPlayer = 23;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "PlayerCheats", friendlyName = "BlockLeftHand")]
        public const int PlayerCheats_BlockLeftHand = 24;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "PlayerCheats", friendlyName = "BlockRightHand")]
        public const int PlayerCheats_BlockRightHand = 25;
    }
    public static partial class Category {
        public const int Default = 0;
        public const int Character = 1;
        public const int Menu = 2;
        public const int Cheats = 4;
    }
    public static partial class Layout {
        public static partial class Joystick {
            public const int Default = 0;
            public const int DefaultControl = 1;
            public const int AssistedControlLeft = 3;
        }
        public static partial class Keyboard {
            public const int Default = 0;
            public const int LayoutDefault = 1;
        }
        public static partial class Mouse {
            public const int Default = 0;
        }
        public static partial class CustomController {
            public const int Default = 0;
        }
    }
    public static partial class Player {
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "System")]
        public const int System = 9999999;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player0")]
        public const int Player0 = 0;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player1")]
        public const int Player1 = 1;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player2")]
        public const int Player2 = 2;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player3")]
        public const int Player3 = 3;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player4")]
        public const int Player4 = 4;
    }
}
