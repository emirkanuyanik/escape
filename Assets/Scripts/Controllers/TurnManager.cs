using UnityEngine;
using System.Collections.Generic;
using CardGame.Data;
using System.Linq;

namespace CardGame.Controllers
{
    public class TurnManager : MonoBehaviour
    {
        public int CurrentPlayerIndex { get; private set; }
        private List<PlayerModel> _players;

        public void InitializeTurns(List<PlayerModel> players, int startingPlayerIndex = 0)
        {
            _players = players;
            CurrentPlayerIndex = startingPlayerIndex;
            
            // Safety check in case the starting player has no cards (unlikely at start)
            ValidateCurrentPlayer();
        }

        public void AdvanceTurn()
        {
            if (_players == null || _players.Count == 0) return;

            int totalPlayers = _players.Count;
            // Loop sequentially
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % totalPlayers;
            
            ValidateCurrentPlayer();
        }

        private void ValidateCurrentPlayer()
        {
            if (_players == null || _players.Count == 0) return;

            int startIndex = CurrentPlayerIndex;
            bool allEmpty = true;

            // Check if ANY player has cards
            foreach (var player in _players)
            {
                if (player.HasCardsRemaining)
                {
                    allEmpty = false;
                    break;
                }
            }

            if (allEmpty)
            {
                // Let MatchManager handle game over
                return;
            }

            // Skip players with no cards
            while (!_players[CurrentPlayerIndex].HasCardsRemaining)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Count;
                
                // Safety breakout (should not hit if allEmpty check above works)
                if (CurrentPlayerIndex == startIndex) break;
            }
        }

        public PlayerModel GetCurrentPlayer()
        {
            if (_players != null && CurrentPlayerIndex >= 0 && CurrentPlayerIndex < _players.Count)
            {
                return _players[CurrentPlayerIndex];
            }
            return null;
        }
    }
}
