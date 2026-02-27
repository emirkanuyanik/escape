using UnityEngine;
using System.Collections.Generic;
using CardGame.Data;
using System.Linq;

namespace CardGame.Core
{
    public class MatchManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; }
        
        [Header("Dependencies")]
        public Controllers.TurnManager TurnManager;
        public Controllers.DeckManager DeckManager;
        public UI.UIManager UIManager;

        [Header("State")]
        public List<PlayerModel> Players = new List<PlayerModel>();
        public List<CardData> CenterPile = new List<CardData>();

        public void InitializeGame(int playerCount)
        {
            SetState(GameState.Initialization);

            // Generate Players
            Players.Clear();
            playerCount = Mathf.Clamp(playerCount, 2, 4);
            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(new PlayerModel(i, $"Player {i + 1}"));
            }

            // Deal Cards
            DeckManager.InitializeDeck();
            DeckManager.DealCards(Players);

            // Start Turns
            TurnManager.InitializeTurns(Players, 0);

            SetState(GameState.TurnStart);
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"Game State Changed: {CurrentState}");
            
            // Handle State Entry Logic
            switch (CurrentState)
            {
                case GameState.TurnStart:
                    HandleTurnStart();
                    break;
                case GameState.TurnEnd:
                    HandleTurnEnd();
                    break;
                case GameState.GameOver:
                    HandleGameOver();
                    break;
            }
        }

        private void HandleTurnStart()
        {
            // Update UI to show whose turn it is
            string currentPlayerName = Players[TurnManager.CurrentPlayerIndex].PlayerName;
            UIManager.UpdateTurnDisplay(TurnManager.CurrentPlayerIndex, currentPlayerName);

            // Give control to the player
            SetState(GameState.WaitingForInput);
        }

        // Called by PlayerController when a card is selected
        public void OnCardPlayed(Controllers.PlayerController player, CardData playedCard)
        {
            if (CurrentState != GameState.WaitingForInput) return;
            if (player.Model.PlayerIndex != TurnManager.CurrentPlayerIndex) return;

            SetState(GameState.Resolution);

            Debug.Log($"Player {player.Model.PlayerName} played {playedCard.DisplayName} ({playedCard.Club})");

            // Match Logic
            bool isMatch = false;

            if (CenterPile.Count > 0)
            {
                CardData previousTopCard = CenterPile[CenterPile.Count - 1];
                if (playedCard.PlayerID == previousTopCard.PlayerID) // EXACT MATCH RULE
                {
                    isMatch = true;
                }
            }

            // 1. Always add the played card to the center pile first
            CenterPile.Add(playedCard);

            if (isMatch)
            {
                Debug.Log("MATCH! Collecting center pile.");
                // Give center pile to player
                player.ReceiveCollectedCards(new List<CardData>(CenterPile));
                CenterPile.Clear();
                UIManager.ClearCenterPile();
            }

            // Update UI
            UIManager.UpdateCenterPile(playedCard);

            // Transition to Turn End
            SetState(GameState.TurnEnd);
        }

        private void HandleTurnEnd()
        {
            bool anyPlayerHasCards = Players.Any(p => p.HasCardsRemaining);

            if (!anyPlayerHasCards)
            {
                SetState(GameState.GameOver);
                return;
            }

            // Otherwise, advance turn
            TurnManager.AdvanceTurn();
            SetState(GameState.TurnStart);
        }

        private void HandleGameOver()
        {
            Debug.Log("GAME OVER");

            if (Players == null || Players.Count == 0) return;

            // Find max score
            int maxScore = Players.Max(p => p.Score);
            
            // Find all players with that score
            List<PlayerModel> winners = Players.Where(p => p.Score == maxScore).ToList();

            if (winners.Count == 1)
            {
                Debug.Log($"Winner: {winners[0].PlayerName} with {winners[0].Score} cards!");
            }
            else
            {
                string tiedNames = string.Join(" and ", winners.Select(w => w.PlayerName));
                Debug.Log($"DRAW between: {tiedNames} with {maxScore} cards!");
            }

            UIManager.ShowGameOver(winners);
        }
    }
}
