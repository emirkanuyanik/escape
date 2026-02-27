using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardGame.Data;

namespace CardGame.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI References")]
        public Text turnIndicatorText;
        public GameObject gameOverPanel;
        public Text winnerText;
        
        [Header("Center Area")]
        public Transform centerPileContainer;
        public GameObject defaultCardPrefab; // Fallback if no specific prefab
        
        // This is a simplified UI manager that updates text based on game state events.
        // In a full implementation, this would instantiate CardUI prefabs for players' hands.

        public void UpdateTurnDisplay(int playerIndex, string playerName)
        {
            if (turnIndicatorText != null)
            {
                turnIndicatorText.text = $"Current Turn: {playerName} (P{playerIndex + 1})";
            }
        }

        public void UpdateCenterPile(CardData topCard)
        {
            if (centerPileContainer == null || defaultCardPrefab == null) return;

            // Instantiate a visual representation of the card in the center
            GameObject newCardObj = Instantiate(defaultCardPrefab, centerPileContainer);
            CardUI cardUI = newCardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(topCard, -1, null); // -1 index and null owner since it's in center
            }
        }

        public void ClearCenterPile()
        {
            if (centerPileContainer == null) return;

            foreach (Transform child in centerPileContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void ShowGameOver(List<PlayerModel> winners)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            if (winnerText != null)
            {
                if (winners.Count == 1)
                {
                    winnerText.text = $"{winners[0].PlayerName} Wins with {winners[0].Score} cards!";
                }
                else
                {
                    // Tie
                    winnerText.text = $"DRAW! {winners[0].Score} cards each.";
                }
            }
        }
    }
}
