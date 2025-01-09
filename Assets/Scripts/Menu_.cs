using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginLabs
{
    public enum MenuType
    {
        None = 0,
        GameUI,
        LevelSelect,
        Settings,
        LevelPreview,
        Teleporting,

        MainMenu,

        MarbleMenu,
    }

    public abstract class Menu_ : MonoBehaviour
    {

        public abstract MenuType MenuType { get; }

    }
}