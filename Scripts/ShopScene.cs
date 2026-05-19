using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class ShopScene
    {
        private ShopSystem shopSystem;
        private NarratorSystem narratorSystem;
        private Customers currentCustomer;

        private TextButton serveButton;
        private TextButton nextCustomerButton;
        private Label customerDialogueLabel;
        private TextBox priceBox;
        private ComboBox potionBox;
        private Label resultLabel;
        private Desktop shopDesktop;

        public void Load()
        {
            shopSystem = new ShopSystem();
            narratorSystem = new NarratorSystem();
            currentCustomer = new Customers();

            var panel = new Panel();

            customerDialogueLabel = new Label
            {
                Text = currentCustomer.CurrentDialogue,
                Left = 850,
                Top = 100,
                Width = 700,
                Height = 120
            };

            serveButton = new TextButton
            {
                Text = "Serve Customer",
                Left = 850,
                Top = 250,
                Width = 220,
                Height = 60
            };

            potionBox = new ComboBox
            {
                Left = 850,
                Top = 340,
                Width = 250,
                Height = 40,
                Visible = false
            };

            potionBox.Items.Add(new ListItem("Sleep Potion"));
            potionBox.Items.Add(new ListItem("Memory Potion"));
            potionBox.Items.Add(new ListItem("Love Potion"));
            potionBox.Items.Add(new ListItem("Anti-Curse Potion"));

            priceBox = new TextBox
            {
                Left = 850,
                Top = 400,
                Width = 250,
                Height = 40,
                Text = "10",
                Visible = false
            };

            var sellButton = new TextButton
            {
                Text = "Sell",
                Left = 850,
                Top = 460,
                Width = 220,
                Height = 60,
                Visible = false
            };

            resultLabel = new Label
            {
                Text = "",
                Left = 850,
                Top = 540,
                Width = 600,
                Height = 80
            };

            nextCustomerButton = new TextButton
            {
                Text = "Next Customer",
                Left = 850,
                Top = 640,
                Width = 220,
                Height = 60,
                Visible = false
            };

            serveButton.Click += (s, a) =>
            {
                potionBox.Visible = true;
                priceBox.Visible = true;
                sellButton.Visible = true;
            };

            sellButton.Click += (s, a) =>
            {
                string potion = potionBox.SelectedItem.Text;
                int price = int.Parse(priceBox.Text);

                resultLabel.Text = narratorSystem.GetSellResultText(
                    currentCustomer,
                    potion,
                    price
                );

                nextCustomerButton.Visible = true;
            };

            nextCustomerButton.Click += (s, a) =>
            {
                currentCustomer = new Customers();

                customerDialogueLabel.Text = currentCustomer.CurrentDialogue;
                resultLabel.Text = narratorSystem.GetWaitingText();

                priceBox.Text = "10";

                potionBox.Visible = false;
                priceBox.Visible = false;
                sellButton.Visible = false;
                nextCustomerButton.Visible = false;
            };

            panel.Widgets.Add(customerDialogueLabel);
            panel.Widgets.Add(serveButton);
            panel.Widgets.Add(potionBox);
            panel.Widgets.Add(priceBox);
            panel.Widgets.Add(sellButton);
            panel.Widgets.Add(resultLabel);
            panel.Widgets.Add(nextCustomerButton);

            shopDesktop = new Desktop();
            shopDesktop.Root = panel;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw()
        {
            shopDesktop?.Render();
        }
    }
}