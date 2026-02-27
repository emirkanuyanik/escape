using UnityEngine;
using System.Collections.Generic;
using CardGame.Data;
using System.Linq;

namespace CardGame.Controllers
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private CardDatabase cardDatabase;

        private List<CardData> _currentDeck = new List<CardData>();

        public void InitializeDeck()
        {
            if (cardDatabase == null || cardDatabase.Cards.Count == 0)
            {
                Debug.LogError("CardDatabase is missing or empty!");
                return;
            }

            // Create a working copy of the deck
            _currentDeck.Clear();
            foreach (var card in cardDatabase.Cards)
            {
                // Instantiate to avoid modifying original ScriptableObject data if needed
                CardData newCard = new CardData
                {
                    PlayerID = card.PlayerID,
                    DisplayName = card.DisplayName,
                    Club = card.Club,
                    PlayerPortrait = card.PlayerPortrait
                };
                _currentDeck.Add(newCard);
            }

            ShuffleDeck();
        }

        private void ShuffleDeck()
        {
            for (int i = 0; i < _currentDeck.Count; i++)
            {
                CardData temp = _currentDeck[i];
                int randomIndex = Random.Range(i, _currentDeck.Count);
                _currentDeck[i] = _currentDeck[randomIndex];
                _currentDeck[randomIndex] = temp;
            }
        }

        public void DealCards(List<PlayerModel> players)
        {
            if (players == null || players.Count == 0) return;

            int totalPlayers = players.Count;
            int currentPlayerIndex = 0;

            // Deal all cards one by one
            while (_currentDeck.Count > 0)
            {
                CardData dealtCard = _currentDeck[0];
                _currentDeck.RemoveAt(0);

                players[currentPlayerIndex].HandCards.Add(dealtCard);

                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            }
        }
    }
}
