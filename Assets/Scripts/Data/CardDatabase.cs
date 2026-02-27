using UnityEngine;
using System.Collections.Generic;

namespace CardGame.Data
{
    [CreateAssetMenu(fileName = "NewCardDatabase", menuName = "CardGame/CardDatabase")]
    public class CardDatabase : ScriptableObject
    {
        public List<CardData> Cards = new List<CardData>();
    }
}
