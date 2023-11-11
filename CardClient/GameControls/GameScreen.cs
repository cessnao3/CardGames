using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

namespace CardClient.GameControls
{
    public partial class GameScreen : UserControl
    {
        List<Hand> PlayerHands { get; set; } = new();
        List<Card> CenterCards { get; set; } = new();

        List<GamePlayer> Players { get; set; } = new();
        List<List<GameCard>> PlayerHandControls { get; } = new();
        List<GameCard> PlayedGameCards { get; } = new();
        List<GameCard> CenterGameCards { get; } = new();
        List<Label> PlayerLables { get; } = new();

        List<int> Scores { get; set; } = new();

        const int NUM_PLAYERS = 4;
        int PlayerPosition { get; set; } = -1;
        int CurrentPlayerTurn { get; set; } = -1;

        int GameID { get; set; } = -1;

        public GameScreen()
        {
            InitializeComponent();

            // Setup the player hands and other per-player items
            PlayerHands = new List<Hand>();
            CenterCards = new List<Card>();
            for (int i = 0; i < NUM_PLAYERS; ++i)
            {
                // Define the hands
                PlayerHands.Add(new());

                // Setup the played-card cards
                GameCard center_card = new();
                center_card.SetFaceShown(true);
                PlayedGameCards.Add(center_card);
                Controls.Add(center_card);

                // Setup the player labels
                if (OperatingSystem.IsWindows())
                {
                    Label l = new()
                    {
                        Text = string.Empty,
                        BackColor = Color.PaleGreen,
                        Font = new(family: FontFamily.GenericMonospace, emSize: 9),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Visible = false
                    };

                    PlayerLables.Add(l);
                    Controls.Add(l);
                }
            }

            // Update the hands and the card locations
            SyncHandCards();
        }

        /// <summary>
        /// Defines the game ID for the current game window
        /// </summary>
        /// <param name="gameId">The Game ID to apply to the current game window</param>
        public void SetGameID(int gameId)
        {
            GameID = gameId;
        }

        public void UpdateFromStatus(MsgGameStatus status)
        {
            PlayerHands = status.Hands.ToList();
            CenterCards = status.CenterActionCards.ToList();

            PlayerPosition = -1;
            for (int i = 0; i < status.Players.Count; ++i)
            {
                if (status.Players[i].Equals(Network.GameComms.GetPlayer()))
                {
                    PlayerPosition = i;
                    break;
                }
            }

            for (int i = 0; i < status.PlayedCardsByPlayer.Count; ++i)
            {
                PlayedGameCards[i].SetCard(c: status.PlayedCardsByPlayer[i]);
            }

            Players = status.Players.ToList();

            CurrentPlayerTurn = status.CurrentPlayer;

            Scores = status.Scores.ToList();

            SyncHandCards();
            SyncCenterCards();
        }


        public void SyncCenterCards()
        {
            // Update the cards if needed
            bool update_needed = false;

            // Loop through the center cards to define output parameters
            for (int i = 0; i < CenterCards.Count; ++i)
            {
                GameCard game_card;
                Card card = CenterCards[i];

                if (i < CenterGameCards.Count)
                {
                    game_card = CenterGameCards[i];

                    if (game_card.BaseCard != null && !game_card.BaseCard.Equals(card))
                    {
                        game_card.SetCard(card);
                        update_needed = true;
                    }
                }
                else
                {
                    game_card = new GameCard(card: card);
                    Controls.Add(game_card);
                    CenterGameCards.Add(game_card);
                    game_card.Click += OnGameCardClick;
                    update_needed = true;
                }

                game_card.SetFaceShown(true);
            }

            // Remove extra cards
            while (CenterGameCards.Count > CenterCards.Count)
            {
                int end_i = CenterGameCards.Count - 1;
                GameCard gc = CenterGameCards[end_i];
                Controls.Remove(gc);
                CenterGameCards.RemoveAt(end_i);
                update_needed = true;
            }

            // Update the card locations as needed
            if (update_needed)
            {
                UpdateCenterCards();
            }
        }


        public void SyncHandCards()
        {
            // Don't update if the current player is undefined
            if (PlayerPosition < 0) return;

            // Define a boolean for a card update needed
            bool update_needed = false;

            for (int i = 0; i < PlayerHands.Count; ++i)
            {
                // Add the player lists if required
                if (PlayerHandControls.Count <= i)
                {
                    PlayerHandControls.Add(new List<GameCard>());
                    update_needed = true;
                }

                // Add the cards as required
                for (int j = 0; j < PlayerHands[i].Cards.Count; ++j)
                {
                    Card card = PlayerHands[i].Cards[j];

                    GameCard game_card;

                    if (j < PlayerHandControls[i].Count)
                    {
                        game_card = PlayerHandControls[i][j];

                        if (game_card.BaseCard != null && !game_card.BaseCard.Equals(card))
                        {
                            game_card.SetCard(c: card);
                            update_needed = true;
                        }
                    }
                    else
                    {
                        game_card = new GameCard(card: card);
                        Controls.Add(game_card);
                        PlayerHandControls[i].Add(game_card);
                        game_card.Click += OnGameCardClick;
                        update_needed = true;
                    }

                    game_card.SetFaceShown(i == PlayerPosition);
                }

                while (PlayerHandControls[i].Count > PlayerHands[i].Cards.Count)
                {
                    int end_i = PlayerHandControls[i].Count - 1;
                    GameCard gc = PlayerHandControls[i][end_i];
                    Controls.Remove(gc);
                    PlayerHandControls[i].RemoveAt(end_i);
                    update_needed = true;
                }
            }

            if (update_needed)
            {
                UpdateCardLocations();
            }
        }

        public void OnGameCardClick(object? sender, EventArgs e)
        {
            if (sender is GameCard gc)
            {
                List<Card> cards_to_check = new();
                cards_to_check.AddRange(PlayerHands[PlayerPosition].Cards);
                cards_to_check.AddRange(CenterCards);

                foreach (Card check_card in cards_to_check)
                {
                    if (check_card != null && gc.BaseCard != null && check_card.Equals(gc.BaseCard))
                    {
                        Network.GameComms.SendMessage(new MsgGamePlay(GameID, Players[PlayerPosition], gc.BaseCard));
                        Console.WriteLine($"Card {gc.BaseCard} clicked");
                        SyncHandCards();
                        break;
                    }
                }
            }
        }

        private int CardWidth()
        {
            return Width / 16;
        }

        private int CardHeight()
        {
            return (int)(CardWidth() * GameCard.HeightRatio());
        }

        private string PlayerString(int player_loc)
        {
            string dir_str = "";
            switch (player_loc)
            {
                case 0:
                    dir_str = "N";
                    break;
                case 1:
                    dir_str = "E";
                    break;
                case 2:
                    dir_str = "S";
                    break;
                case 3:
                    dir_str = "W";
                    break;
            }

            GamePlayer p = Players[player_loc];
            return $"{dir_str} {p.ShortName}{((CurrentPlayerTurn == player_loc) ? '*' : ' ')}{Environment.NewLine}{Scores[player_loc]}";
        }

        private Size LabelSize()
        {
            return new Size((int)(1.1 * CardWidth()), 36);
        }

        private void UpdateCenterCards()
        {
            int card_incr = CardWidth() + 2;
            int x_center = Width / 2;
            int x_loc = x_center - card_incr * CenterGameCards.Count / 2;

            int y_loc = Height / 2 - CardHeight();

            foreach (GameCard gc in CenterGameCards)
            {
                gc.SetWidth(CardWidth());
                gc.Location = new Point(
                    x_loc,
                    y_loc);
                x_loc += card_incr;
            }
        }

        private void UpdateCardHorizontal(
            int player_loc,
            bool is_top)
        {
            List<GameCard> game_cards = PlayerHandControls[player_loc];

            int card_incr = CardWidth() + 2;
            int x_center = Width / 2;
            int x_loc = x_center - card_incr * game_cards.Count / 2;
            int x_loc_lbl = x_center - card_incr * 13 / 2;

            int y_loc;
            if (is_top)
            {
                y_loc = (int)(CardHeight() * 0.1);
            }
            else
            {
                y_loc = (int)(Height - CardHeight() * 1.1);
            }

            Label l = PlayerLables[player_loc];
            l.Size = LabelSize();
            l.Location = new Point(
                x_loc_lbl,
                y_loc + (int)(is_top ? (1.1 * CardHeight()) : (-0.1 * CardHeight() - l.Height)));
            l.Text = PlayerString(player_loc);
            l.Visible = true;

            foreach (GameCard gc in game_cards)
            {
                gc.SetWidth(CardWidth());
                gc.Location = new Point(
                    x_loc,
                    y_loc);
                x_loc += card_incr;
            }

            // Increment a modified card length for the center pool / played cards
            y_loc += (int)((is_top ? 1 : -1) * CardHeight() * 1.1);

            GameCard pool_card = PlayedGameCards[player_loc];
            pool_card.SetWidth(CardWidth());
            pool_card.Location = new Point(
                x_center,
                y_loc);
        }

        private void UpdateCardVertical(
            int player_loc,
            bool is_left)
        {
            List<GameCard> game_cards = PlayerHandControls[player_loc];

            int y_center = Height / 2 - (int) (CardHeight() / 2.0);

            int card_incr = (int)(Height / 2.0 / 13.0);
            int y_loc = y_center - card_incr * game_cards.Count / 2;
            int y_loc_init = y_center - card_incr * 13 / 2;

            int x_loc;
            if (is_left)
            {
                x_loc = (int)(0.1 * CardWidth());
            }
            else
            {
                x_loc = (int)(Width - 1.1 * CardWidth());
            }

            Label l = PlayerLables[player_loc];
            l.Size = LabelSize();
            l.Location = new Point(
                is_left ? x_loc : Width - l.Width - (int)(0.1 * CardWidth()),
                y_loc_init - l.Height - (int)(0.1 * CardHeight()));
            l.Text = PlayerString(player_loc);
            l.Visible = true;

            foreach (GameCard gc in game_cards)
            {
                gc.SetWidth(CardWidth());
                gc.Location = new Point(
                    x_loc,
                    y_loc);
                y_loc += card_incr;
            }

            x_loc += (is_left ? 1 : -1) * (int)(CardWidth() * 1.1);

            GameCard pool_card = PlayedGameCards[player_loc];
            pool_card.SetWidth(CardWidth());
            pool_card.Location = new Point(
                x_loc,
                y_center);
        }

        private void UpdateCardLocations()
        {
            if (PlayerPosition < 0) return;

            UpdateCardHorizontal(
                PlayerPosition,
                is_top: false);
            UpdateCardHorizontal(
                (PlayerPosition + 2) % PlayerHandControls.Count,
                is_top: true);

            UpdateCardVertical(
                (PlayerPosition + 1) % PlayerHandControls.Count,
                is_left: true);

            UpdateCardVertical(
                (PlayerPosition + 3) % PlayerHandControls.Count,
                is_left: false);

            UpdateCenterCards();
        }

        private void GameScreen_SizeChanged(object sender, EventArgs e)
        {
            UpdateCardLocations();
        }
    }
}
