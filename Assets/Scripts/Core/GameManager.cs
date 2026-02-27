using UnityEngine;
using CardGame.Core;

namespace CardGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public MatchManager Match;

        // Configuration
        public int NumberOfPlayers = 4;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Validate references
            if (Match == null) Match = GetComponentInChildren<MatchManager>();
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            if (Match != null)
            {
                Match.InitializeGame(NumberOfPlayers);
            }
            else
            {
                Debug.LogError("MatchManager reference is missing in GameManager.");
            }
        }
    }
}
