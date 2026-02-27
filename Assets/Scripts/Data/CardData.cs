using UnityEngine;
using System.Collections.Generic;

namespace CardGame.Data
{
    public enum ClubType
    {
        Fenerbahce,
        Galatasaray,
        Besiktas,
        Trabzonspor
    }

    [System.Serializable]
    public class CardData
    {
        public string PlayerID;
        public string DisplayName;
        public ClubType Club;
        public Sprite PlayerPortrait;

        public CardData() { }

        public CardData(string id, string name, ClubType club)
        {
            PlayerID = id;
            DisplayName = name;
            Club = club;
        }

        public static Color GetClubColor(ClubType club)
        {
            switch (club)
            {
                case ClubType.Fenerbahce: return new Color(1f, 0.85f, 0f);      // Yellow
                case ClubType.Galatasaray: return new Color(0.9f, 0.15f, 0.15f); // Red
                case ClubType.Besiktas: return new Color(0.15f, 0.15f, 0.15f);   // Black
                case ClubType.Trabzonspor: return new Color(0.1f, 0.3f, 0.7f);   // Dark blue
                default: return Color.gray;
            }
        }

        public static Color GetClubTextColor(ClubType club)
        {
            switch (club)
            {
                case ClubType.Fenerbahce: return new Color(0f, 0.1f, 0.4f);    // Navy text
                case ClubType.Galatasaray: return Color.white;
                case ClubType.Besiktas: return Color.white;
                case ClubType.Trabzonspor: return Color.white;
                default: return Color.white;
            }
        }

        public static List<CardData> GenerateFullDeck()
        {
            List<CardData> deck = new List<CardData>();

            // Fenerbahce Players
            deck.Add(new CardData("FB_Dzeko", "Edin Dzeko", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Tadic", "Dusan Tadic", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Szymanski", "Sebastian Szymanski", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Irfan", "Irfan Can Kahveci", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Osayi", "Osayi-Samuel", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Livakovic", "Dominik Livakovic", ClubType.Fenerbahce));
            deck.Add(new CardData("FB_Batshuayi", "Michy Batshuayi", ClubType.Fenerbahce));

            // Galatasaray Players
            deck.Add(new CardData("GS_Icardi", "Mauro Icardi", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Muslera", "Fernando Muslera", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Mertens", "Dries Mertens", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Ziyech", "Hakim Ziyech", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Torreira", "Lucas Torreira", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Nelsson", "Victor Nelsson", ClubType.Galatasaray));
            deck.Add(new CardData("GS_Kerem", "Kerem Akturkoglu", ClubType.Galatasaray));

            // Besiktas Players
            deck.Add(new CardData("BJK_Gedson", "Gedson Fernandes", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Muci", "Ernest Muci", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Rafa", "Rafa Silva", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Cenk", "Cenk Tosun", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Aboubakar", "Vincent Aboubakar", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Ersin", "Ersin Destanoglu", ClubType.Besiktas));
            deck.Add(new CardData("BJK_Atakan", "Atakan Uner", ClubType.Besiktas));

            // Trabzonspor Players
            deck.Add(new CardData("TS_Ugurcan", "Ugurcan Cakir", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Visca", "Edin Visca", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Bakasetas", "Anastasios Bakasetas", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Trezeguet", "Trezeguet", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Maxi", "Maxi Gomez", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Bardakci", "Abdulkadir Bardakci", ClubType.Trabzonspor));
            deck.Add(new CardData("TS_Orsic", "Mislav Orsic", ClubType.Trabzonspor));

            // DUPLICATES for matching (each player appears 2x total = 56 cards)
            List<CardData> duplicates = new List<CardData>();
            foreach (var card in deck)
            {
                duplicates.Add(new CardData(card.PlayerID, card.DisplayName, card.Club));
            }
            deck.AddRange(duplicates);

            return deck;
        }
    }
}
