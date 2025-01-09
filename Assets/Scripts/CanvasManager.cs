using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginLabs
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] MenuType m_ActiveCanvasType = MenuType.None;

        List<Menu_> canvasControllerList = new List<Menu_>();

        private void OnValidate()
        {
            GetComponentsInChildren(true, canvasControllerList);

            foreach (Menu_ cc in canvasControllerList)
                cc.gameObject.SetActive(cc.MenuType == m_ActiveCanvasType);
        }

        private void Start()
        {
            GetComponentsInChildren<Menu_>(true, canvasControllerList);
            canvasControllerList.ForEach(x => x.gameObject.SetActive(false));

            UpdateCursorState(); // Initialize cursor state
        }

        private void Update()
        {
            // Ensure cursor state is maintained based on the active menu type
            UpdateCursorState();
        }

        public void SwitchCanvas(MenuType _type)
        {
            m_ActiveCanvasType = _type;

            foreach (Menu_ canvasController in canvasControllerList)
                canvasController.gameObject.SetActive(canvasController.MenuType == _type);

            UpdateCursorState();
        }

        public MenuType activeCanvasType { get { return m_ActiveCanvasType; } }

        /// <summary>
        /// Updates the cursor visibility and lock state based on the active menu.
        /// </summary>
        private void UpdateCursorState()
        {
            // Check if the active menu is a menu type where the cursor should be visible
            if (m_ActiveCanvasType == MenuType.LevelSelect || m_ActiveCanvasType == MenuType.Teleporting || m_ActiveCanvasType == MenuType.MainMenu || m_ActiveCanvasType == MenuType.MarbleMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Player_Runtime.IsInMenu = true; // Update runtime state
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Player_Runtime.IsInMenu = false; // Update runtime state
            }
        }
    }
}
