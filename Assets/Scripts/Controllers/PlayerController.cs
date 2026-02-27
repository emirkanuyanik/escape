using UnityEngine;
using CardGame.Data;
using System.Collections.Generic;
using CardGame.Core;

namespace CardGame.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerModel Model { get; private set; }
        
        // This would be assigned in inspector or instantiated dynamically
        // [SerializeField] private PlayerUI playerUI;

        private MatchManager _matchManager;

        public void Initialize(PlayerModel model, MatchManager matchManager)
        {
            Model = model;
            _matchManager = matchManager;
        }

        public void PlayCard(int cardIndex)
        {
            // Safety checks
            if (cardIndex < 0 || cardIndex >= Model.HandCards.Count) return;
            if (_matchManager.CurrentState != GameState.WaitingForInput) return;
            if (_matchManager.TurnManager.CurrentPlayerIndex != Model.PlayerIndex) return;

            CardData cardToPlay = Model.HandCards[cardIndex];
            Model.HandCards.RemoveAt(cardIndex);

            // Inform the MatchManager that a card has been played
            _matchManager.OnCardPlayed(this, cardToPlay);
        }

        public void ReceiveCollectedCards(List<CardData> cardsToCollect)
        {
            if (cardsToCollect == null || cardsToCollect.Count == 0) return;

            Model.CollectedCards.AddRange(cardsToCollect);
            Debug.Log($"Player {Model.PlayerName} collected {cardsToCollect.Count} cards. New score: {Model.Score}");
        }
    }
}
