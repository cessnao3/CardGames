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
        List<Hand> player_hands;
        List<Card> center_cards;

        List<GamePlayer> players;
        List<List<GameCard>> player_hand_controls = new List<List<GameCard>>();
        List<GameCard> played_game_cards = new List<GameCard>();
        List<GameCard> center_game_cards = new List<GameCard>();
        List<Label> player_labels = new List<Label>();

        List<int> scores;

        const int num_players = 4;
        int player_position = -1;
        int current_player_turn = -1;

        int game_id = -1;

        public GameScreen()
        {
            InitializeComponent();

            // Setup the player hands and other per-player items
            player_hands = new List<Hand>();
            center_cards = new List<Card>();
            for (int i = 0; i < num_players; ++i)
            {
                // Define the hands
                player_hands.Add(new Hand());

                // Setup the played-card cards
                GameCard center_card = new GameCard();
                center_card.SetFaceShown(true);
                played_game_cards.Add(center_card);
                Controls.Add(center_card);

                // Setup the player labels
                Label l = new Label()
                {
                    Text = string.Empty,
                    BackColor = Color.PaleGreen,
                    Font = new Font(FontFamily.GenericMonospace, 9),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Visible = false
                };
                player_labels.Add(l);
                Controls.Add(l);
            }

            // Update the hands and the card locations
            SyncHandCards();
        }

        /// <summary>
        /// Defines the game ID for the current game window
        /// </summary>
        /// <param name="game_id">The Game ID to apply to the current game window</param>
        public void SetGameID(int game_id)
        {
            this.game_id = game_id;
        }

        public void UpdateFromStatus(MsgGameStatus status)
        {
            player_hands = status.Hands;
            center_cards = status.CenterActionCards;

            player_position = -1;
            for (int i = 0; i < status.Players.Count; ++i)
            {
                if (status.Players[i].Equals(Network.GameComms.GetPlayer()))
                {
                    player_position = i;
                    break;
                }
            }

            for (int i = 0; i < status.PlayedCardsByPlayer.Count; ++i)
            {
                played_game_cards[i].SetCard(c: status.PlayedCardsByPlayer[i]);
            }

            players = status.Players;

            current_player_turn = status.CurrentPlayer;

            scores = status.Scores;

            SyncHandCards();
            SyncCenterCards();
        }


        public void SyncCenterCards()
        {
            // Update the cards if needed
            bool update_needed = false;

            // Loop through the center cards to define output parameters
            for (int i = 0; i < center_cards.Count; ++i)
            {
                GameCard game_card;
                Card card = center_cards[i];

                if (i < center_game_cards.Count)
                {
                    game_card = center_game_cards[i];

                    if (!game_card.base_card.Equals(card))
                    {
                        game_card.SetCard(card);
                        update_needed = true;
                    }
                }
                else
                {
                    game_card = new GameCard(card: card);
                    Controls.Add(game_card);
                    center_game_cards.Add(game_card);
                    game_card.Click += onGameCardClick;
                    update_needed = true;
                }

                game_card.SetFaceShown(true);
            }

            // Remove extra cards
            while (center_game_cards.Count > center_cards.Count)
            {
                int end_i = center_game_cards.Count - 1;
                GameCard gc = center_game_cards[end_i];
                Controls.Remove(gc);
                center_game_cards.RemoveAt(end_i);
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
            if (player_position < 0) return;

            // Define a boolean for a card update needed
            bool update_needed = false;

            for (int i = 0; i < player_hands.Count; ++i)
            {
                // Add the player lists if required
                if (player_hand_controls.Count <= i)
                {
                    player_hand_controls.Add(new List<GameCard>());
                    update_needed = true;
                }

                // Add the cards as required
                for (int j = 0; j < player_hands[i].cards.Count; ++j)
                {
                    Card card = player_hands[i].cards[j];

                    GameCard game_card;

                    if (j < player_hand_controls[i].Count)
                    {
                        game_card = player_hand_controls[i][j];

                        if (!game_card.base_card.Equals(card))
                        {
                            game_card.SetCard(c: card);
                            update_needed = true;
                        }
                    }
                    else
                    {
                        game_card = new GameCard(card: card);
                        Controls.Add(game_card);
                        player_hand_controls[i].Add(game_card);
                        game_card.Click += onGameCardClick;
                        update_needed = true;
                    }

                    game_card.SetFaceShown(i == player_position);
                }

                while (player_hand_controls[i].Count > player_hands[i].cards.Count)
                {
                    int end_i = player_hand_controls[i].Count - 1;
                    GameCard gc = player_hand_controls[i][end_i];
                    Controls.Remove(gc);
                    player_hand_controls[i].RemoveAt(end_i);
                    update_needed = true;
                }
            }

            if (update_needed)
            {
                UpdateCardLocations();
            }
        }

        public void onGameCardClick(object sender, EventArgs e)
        {
            if (sender is GameCard)
            {
                GameCard gc = (GameCard)sender;

                List<Card> cards_to_check = new List<Card>();
                cards_to_check.AddRange(player_hands[player_position].cards);
                cards_to_check.AddRange(center_cards);

                foreach (Card check_card in cards_to_check)
                {
                    if (check_card != null && check_card.Equals(gc.base_card))
                    {
                        Network.GameComms.SendMessage(new MsgGamePlay()
                        {
                            Card = gc.base_card,
                            GameID = game_id,
                            Player = players[player_position]
                        });
                        Console.WriteLine(string.Format(
                            "Card {0:s} clicked",
                            gc.base_card.ToString()));
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

                    GamePlayer p = players[player_loc];
            return string.Format(
                "{0:} {1:}{2:}{4:}{3:  0}",
                dir_str,
                p.ShortName(),
                (current_player_turn == player_loc) ? "*" : " ",
                scores[player_loc],
                Environment.NewLine);
        }

        private Size LabelSize()
        {
            return new Size((int)(1.1 * CardWidth()), 36);
        }

        private void UpdateCenterCards()
        {
            int card_incr = CardWidth() + 2;
            int x_center = Width / 2;
            int x_loc = x_center - card_incr * center_game_cards.Count / 2;

            int y_loc = Height / 2 - CardHeight();

            foreach (GameCard gc in center_game_cards)
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
            List<GameCard> game_cards = player_hand_controls[player_loc];

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

            Label l = player_labels[player_loc];
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

            GameCard pool_card = played_game_cards[player_loc];
            pool_card.SetWidth(CardWidth());
            pool_card.Location = new Point(
                x_center,
                y_loc);
        }

        private void UpdateCardVertical(
            int player_loc,
            bool is_left)
        {
            List<GameCard> game_cards = player_hand_controls[player_loc];

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

            Label l = player_labels[player_loc];
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

            GameCard pool_card = played_game_cards[player_loc];
            pool_card.SetWidth(CardWidth());
            pool_card.Location = new Point(
                x_loc,
                y_center);
        }

        private void UpdateCardLocations()
        {
            if (player_position < 0) return;

            UpdateCardHorizontal(
                player_position,
                is_top: false);
            UpdateCardHorizontal(
                (player_position + 2) % player_hand_controls.Count,
                is_top: true);

            UpdateCardVertical(
                (player_position + 1) % player_hand_controls.Count,
                is_left: true);

            UpdateCardVertical(
                (player_position + 3) % player_hand_controls.Count,
                is_left: false);

            UpdateCenterCards();
        }

        private void GameScreen_SizeChanged(object sender, EventArgs e)
        {
            UpdateCardLocations();
        }
    }
}
