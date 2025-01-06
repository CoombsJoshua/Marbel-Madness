using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using UnityEngine;

namespace OriginLabs
{
    public class Menu_Main : Menu_
    {
        public override MenuType MenuType => MenuType.MainMenu;

        public TextMeshProUGUI m_ConnectionStatus;
        public TextMeshProUGUI m_Username;

        public GameObject Marble;
        public Transform MarbleDisplayPosition; // Position to display the marble in the menu

        private GameObject displayedMarbleInstance;
        public GameObject ButtonContainer;

        private async void Start()
        {
            // Initialize Unity Services
            m_ConnectionStatus.text = "Initializing...";
            await UnityServices.InitializeAsync();

            // Attempt to auto-login
            m_ConnectionStatus.text = "Connecting...";
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                // Assign a random username upon first login
                if (AuthenticationService.Instance.IsSignedIn && !PlayerPrefs.HasKey("PlayerUsername"))
                {
                    string randomUsername = GenerateRandomUsername();
                    PlayerPrefs.SetString("PlayerUsername", randomUsername);
                }
            }

            // Check login status
            if (AuthenticationService.Instance.IsSignedIn)
            {
                m_ConnectionStatus.gameObject.SetActive(false); // Hide connection status
                string username = PlayerPrefs.GetString("PlayerUsername", "Player");
                m_Username.text = $"Welcome, {username}";
                ButtonContainer.SetActive(true);

                // Spawn a visual marble for customization later
                SpawnVisualMarble();
            }
            else
            {
                m_ConnectionStatus.text = "Failed to log in. Please restart.";
            }
        }

        private void SpawnVisualMarble()
        {
            if (Marble == null || MarbleDisplayPosition == null)
            {
                Debug.LogWarning("Marble prefab or display position is not assigned.");
                return;
            }

            // Destroy the previous marble instance if it exists
            if (displayedMarbleInstance != null)
            {
                Destroy(displayedMarbleInstance);
            }

            // Instantiate and position the marble in the menu
            displayedMarbleInstance = Instantiate(Marble, MarbleDisplayPosition.position, MarbleDisplayPosition.rotation);
        }

        private string GenerateRandomUsername()
        {
            string[] adjectives = { "Brave", "Swift", "Clever", "Mighty", "Bold" };
            string[] nouns = { "Fox", "Eagle", "Bear", "Lion", "Wolf" };

            string randomAdjective = adjectives[UnityEngine.Random.Range(0, adjectives.Length)];
            string randomNoun = nouns[UnityEngine.Random.Range(0, nouns.Length)];
            int randomNumber = UnityEngine.Random.Range(1, 1000);

            return $"{randomAdjective}{randomNoun}{randomNumber}";
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
