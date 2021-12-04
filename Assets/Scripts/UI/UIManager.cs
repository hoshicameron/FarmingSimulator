using Misc;
using UnityEngine;
using UnityEngine.UI;
using _Player;

namespace UI
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        [SerializeField] private UIInventoryBar uiInventoryBar = null;
        [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement=null;
        [SerializeField] private GameObject pauseMenu = null;
        [SerializeField] private GameObject[] menuTabs= null;
        [SerializeField] private Button[] menuButtons=null;


        public bool PauseMenuOn { get; set; }

        protected override void Awake()
        {
            base.Awake();

            pauseMenu.SetActive(false);
        }

        private void Update()
        {
            PauseMenu();
        }

        private void PauseMenu()
        {
            // Toggle pause menu if escape is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PauseMenuOn)
                {
                    DisablePauseMenu();
                } else
                {
                    EnablePauseMenu();
                }
            }
        }

        private void EnablePauseMenu()
        {
            // Destroy any currently dragged items
            uiInventoryBar.DestroyCurrentlyDraggedItems();

            // Clear any currently selected items
            uiInventoryBar.ClearCurrentlySelectedItems();

            PauseMenuOn=true;
            Player.Instance.PlayerInputIsDisabled = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);

            // Trigger garbage Collector
            System.GC.Collect();

            // Highlight selected button
            HighLightButtonForSelectedTab();
        }

        private void HighLightButtonForSelectedTab()
        {
            for (int i = 0; i < menuTabs.Length; i++)
            {
                if (menuTabs[i].activeSelf)
                {
                    SetButtonColorToActive(menuButtons[i]);
                } else
                {
                    SetButtonColorToInactive(menuButtons[i]);
                }
            }
        }

        private void SetButtonColorToInactive(Button button)
        {
            ColorBlock colors = button.colors;

            colors.normalColor = colors.disabledColor;

            button.colors = colors;
        }

        private void SetButtonColorToActive(Button button)
        {
            ColorBlock colors = button.colors;

            colors.normalColor = colors.pressedColor;

            button.colors = colors;

        }

        public void DisablePauseMenu()
        {
            // Destroy any currently dragged items
            pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

            PauseMenuOn = false;
            Player.Instance.PlayerInputIsDisabled = false;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        public void SwitchPauseMenuTab(int tabNum)
        {
            for(int i=0; i< menuTabs.Length;i++)
            {
                if (i != tabNum)
                {
                    menuTabs[i].SetActive(false);
                } else
                {
                    menuTabs[i].SetActive(true);
                }
            }

            HighLightButtonForSelectedTab();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
