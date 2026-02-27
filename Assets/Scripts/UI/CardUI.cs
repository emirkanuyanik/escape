using UnityEngine;
using UnityEngine.UI;
using CardGame.Data;

namespace CardGame.UI
{
    public class CardUI : MonoBehaviour
    {
        public CardData Data { get; private set; }
        
        [SerializeField] private Text nameText;
        [SerializeField] private Text clubText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Button clickButton;

        // Optionally store index to pass back to PlayerController
        public int HandIndex { get; private set; }
        private Controllers.PlayerController _owner;

        public void Initialize(CardData data, int index, Controllers.PlayerController owner)
        {
            Data = data;
            HandIndex = index;
            _owner = owner;

            UpdateVisuals();

            if (clickButton != null)
            {
                clickButton.onClick.RemoveAllListeners();
                clickButton.onClick.AddListener(OnCardClicked);
            }
        }

        private void UpdateVisuals()
        {
            if (Data == null) return;

            if (nameText != null) nameText.text = Data.DisplayName;
            if (clubText != null) clubText.text = Data.Club.ToString();
            if (portraitImage != null && Data.PlayerPortrait != null)
            {
                portraitImage.sprite = Data.PlayerPortrait;
            }
        }

        private void OnCardClicked()
        {
            if (_owner != null)
            {
                _owner.PlayCard(HandIndex);
            }
        }
    }
}
