using UnityEngine;
using System.Collections.Generic;

namespace CardGame.Data
{
    [System.Serializable]
    public class PlayerModel
    {
        public int PlayerIndex; // 0, 1, 2, 3
        public string PlayerName; // e.g., "Player 1"
        public List<CardData> HandCards = new List<CardData>();
        public List<CardData> CollectedCards = new List<CardData>();
        
        public bool HasCardsRemaining => HandCards.Count > 0;
        public int Score => CollectedCards.Count;

        public PlayerModel(int index, string name)
        {
            PlayerIndex = index;
            PlayerName = name;
        }
    }
}
