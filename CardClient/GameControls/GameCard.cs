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

namespace CardClient.GameControls
{
    public partial class GameCard : UserControl
    {
        public Card? BaseCard { get; private set; }
        bool CardShown { get; set; } = false;

        public GameCard(Card? card = null)
        {
            InitializeComponent();

            LblSpecialLabel.Parent = PicCard;
            SetCard(card);
        }

        public void SetCard(Card? c)
        {
            BaseCard = c;

            if (c != null && c.IsSpecial())
            {
                LblSpecialLabel.Text = CardGameLibrary.GameParameters.GameAction.action_database[c.Data].name;
                LblSpecialLabel.Visible = true;
            }
            else
            {
                LblSpecialLabel.Visible = false;
            }

            UpdatePicture();
        }

        public void SetFaceShown(bool face_shown)
        {
            CardShown = face_shown;
            UpdatePicture();
        }

        public void UpdatePicture()
        {
            Bitmap? bmp_to_set;

            if (BaseCard != null && BaseCard.IsSpecial())
            {
                bmp_to_set = Properties.Resources.card_blank;
            }
            else if (BaseCard == null || !CardShown)
            {
                bmp_to_set = Properties.Resources.card_back;
            }
            else
            {
                string card_name = $"{BaseCard.CardSuit.ToString().ToLower()}_{BaseCard.CardValue.ToString().ToLower()}";
                bmp_to_set = (Bitmap?)Properties.Resources.ResourceManager.GetObject(card_name);
            }

            PicCard.Image = bmp_to_set;

            Visible = BaseCard != null;
        }

        public void SetWidth(int width)
        {
            Size = new Size(width, (int)(width * HeightRatio()));
            UpdatePicture();
        }

        public static double HeightRatio()
        {
            return 1.5;
        }

        private void PicCard_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
        }

        private void LblSpecialLabel_Click(object sender, EventArgs e)
        {
            PicCard_Click(sender, e);
        }
    }
}
