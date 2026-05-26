using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class ShopScene
    {
        private InventorySystem inventory;
        private Customers currentCustomer;
        private Action goToJungle;

        private Desktop shopDesktop;

        private Label moneyLabel;
        private Label inventoryLabel;
        private Label customerDialogueLabel;
        private Label resultLabel;

        private ComboBox sellPotionBox;
        private ComboBox usePotionBox;
        private TextBox priceBox;

        private TextButton serveButton;
        private TextButton sellButton;
        private TextButton usePotionButton;
        private TextButton confirmUsePotionButton;
        private TextButton nextCustomerButton;
        
        // Ormana Geri Dönüş Butonu
        private TextButton returnToJungleButton;

        private Panel usePotionPanel;

        private bool persuasionActive;

        public void Load(InventorySystem inventory, Action goToJungle)
        {
            this.inventory = inventory;
            this.goToJungle = goToJungle;

            currentCustomer = new Customers();

            var panel = new Panel();

            moneyLabel = new Label
            {
                Left = 1400,
                Top = 40,
                Width = 500,
                Height = 40
            };

            inventoryLabel = new Label
            {
                Left = 50,
                Top = 40,
                Width = 400,
                Height = 800
            };

            customerDialogueLabel = new Label
            {
                Text = currentCustomer.CurrentDialogue,
                Left = 610, 
                Top = 150,
                Width = 700,
                Height = 120
            };

            resultLabel = new Label
            {
                Text = "",
                Left = 610,
                Top = 280,
                Width = 700,
                Height = 100
            };

            serveButton = new TextButton
            {
                Text = "Serve Customer",
                Left = 850, 
                Top = 400,
                Width = 220,
                Height = 55
            };

            sellPotionBox = new ComboBox
            {
                Left = 660,
                Top = 480,
                Width = 300,
                Height = 40,
                Visible = false
            };

            priceBox = new TextBox
            {
                Left = 980,
                Top = 480,
                Width = 150,
                Height = 40,
                Text = "10",
                Visible = false
            };

            sellButton = new TextButton
            {
                Text = "Sell Potion",
                Left = 850,
                Top = 550,
                Width = 220,
                Height = 55,
                Visible = false
            };

            nextCustomerButton = new TextButton
            {
                Text = "Next Customer",
                Left = 850,
                Top = 630,
                Width = 220,
                Height = 55,
                Visible = false
            };

            usePotionButton = new TextButton
            {
                Text = "Use Potion",
                Left = 600,
                Top = 850,
                Width = 220,
                Height = 55
            };

            returnToJungleButton = new TextButton
            {
                Text = "Return to Jungle",
                Left = 1100,
                Top = 850,
                Width = 220,
                Height = 55
            };

            CreateUsePotionPanel();

            serveButton.Click += (s, a) =>
            {
                RefreshSellPotionBox();

                sellPotionBox.Visible = true;
                priceBox.Visible = true;
                sellButton.Visible = true;
            };

            sellButton.Click += (s, a) =>
            {
                SellSelectedPotion();
            };

            usePotionButton.Click += (s, a) =>
            {
                RefreshUsePotionBox();
                usePotionPanel.Visible = true;
            };

            confirmUsePotionButton.Click += (s, a) =>
            {
                UseSelectedPotion();
            };

            nextCustomerButton.Click += (s, a) =>
            {
                currentCustomer = new Customers();
                persuasionActive = false;

                customerDialogueLabel.Text = currentCustomer.CurrentDialogue;
                resultLabel.Text = "";

                priceBox.Text = "10";
                sellPotionBox.Visible = false;
                priceBox.Visible = false;
                sellButton.Visible = false;
                nextCustomerButton.Visible = false;

                RefreshAllUI();
            };

            returnToJungleButton.Click += (s, a) =>
            {
                goToJungle?.Invoke();
            };

            panel.Widgets.Add(moneyLabel);
            panel.Widgets.Add(inventoryLabel);
            panel.Widgets.Add(customerDialogueLabel);
            panel.Widgets.Add(serveButton);
            panel.Widgets.Add(sellPotionBox);
            panel.Widgets.Add(priceBox);
            panel.Widgets.Add(sellButton);
            panel.Widgets.Add(usePotionButton);
            panel.Widgets.Add(returnToJungleButton);
            panel.Widgets.Add(resultLabel);
            panel.Widgets.Add(nextCustomerButton);
            panel.Widgets.Add(usePotionPanel);

            shopDesktop = new Desktop();
            shopDesktop.Root = panel;

            RefreshAllUI();
        }

        private void CreateUsePotionPanel()
        {
            usePotionPanel = new Panel
            {
                Left = 710, 
                Top = 350,
                Width = 500,
                Height = 300,
                Visible = false
            };

            var title = new Label
            {
                Text = "Use Potion",
                Left = 20,
                Top = 20,
                Width = 300,
                Height = 40
            };

            usePotionBox = new ComboBox
            {
                Left = 100,
                Top = 80,
                Width = 300,
                Height = 40
            };

            confirmUsePotionButton = new TextButton
            {
                Text = "Use Selected Potion",
                Left = 40,
                Top = 180,
                Width = 230,
                Height = 55
            };

            var closeButton = new TextButton
            {
                Text = "Close",
                Left = 290,
                Top = 180,
                Width = 150,
                Height = 55
            };

            closeButton.Click += (s, a) =>
            {
                usePotionPanel.Visible = false;
            };

            usePotionPanel.Widgets.Add(title);
            usePotionPanel.Widgets.Add(usePotionBox);
            usePotionPanel.Widgets.Add(confirmUsePotionButton);
            usePotionPanel.Widgets.Add(closeButton);
        }

        private void SellSelectedPotion()
        {
            if (sellPotionBox.SelectedItem == null)
            {
                resultLabel.Text = "Choose a potion first.";
                return;
            }

            string potionName = ExtractPotionName(sellPotionBox.SelectedItem.Text);

            if (!int.TryParse(priceBox.Text, out int price))
            {
                resultLabel.Text = "Price must be a number.";
                return;
            }

            if (!inventory.CraftedPotions.ContainsKey(potionName) || inventory.CraftedPotions[potionName] <= 0)
            {
                resultLabel.Text = "You don't have this potion.";
                RefreshAllUI();
                return;
            }

            bool correctPotion = potionName == currentCustomer.WantedPotion;

            if (correctPotion)
            {
                inventory.RemovePotion(potionName, 1);
                inventory.AddMoney(price);

                resultLabel.Text = "Correct potion sold. You earned $" + price + ".";
            }
            else if (persuasionActive)
            {
                int maxPersuasionPrice = currentCustomer.MaxPrice + 25;

                if (price <= maxPersuasionPrice)
                {
                    inventory.RemovePotion(potionName, 1);
                    inventory.AddMoney(price);

                    resultLabel.Text = "Wrong potion sold with persuasion. You earned $" + price + ".";
                    persuasionActive = false;
                }
                else
                {
                    resultLabel.Text = "Too expensive. Even persuasion could not save this scam.";
                    persuasionActive = false;
                }
            }
            else
            {
                resultLabel.Text = "Wrong potion. Customer refused it.";
            }

            nextCustomerButton.Visible = true;
            RefreshAllUI();

            if (!HasAnyPotion())
                goToJungle?.Invoke();
        }

        private void UseSelectedPotion()
        {
            if (usePotionBox.SelectedItem == null)
            {
                resultLabel.Text = "Choose a potion to use.";
                return;
            }

            string potionName = ExtractPotionName(usePotionBox.SelectedItem.Text);

            if (!inventory.RemovePotion(potionName, 1))
            {
                resultLabel.Text = "You don't have this potion.";
                RefreshAllUI();
                return;
            }

            // ==========================================
            // YENİ İKSİR ETKİLERİ SİSTEMİ BAŞLIYOR
            // ==========================================
            switch (potionName)
            {
                case "Sleep Potion":
                    resultLabel.Text = "You used Sleep Potion. The day ends.";
                    RefreshAllUI();
                    goToJungle?.Invoke();
                    return; // Günü bitirdiği için hemen return atıyoruz

                case "Persuasion Potion":
                    persuasionActive = true;
                    resultLabel.Text = "Persuasion active. You can push a wrong potion at a higher price.";
                    break;

                case "Charm Potion":
                    persuasionActive = true;
                    resultLabel.Text = "Extreme Charm! Your persuasion skills are through the roof.";
                    break;

                case "Clarity Potion":
                    inventory.AddMoney(50); // Direkt para ekler
                    resultLabel.Text = "Absolute clarity! You suddenly remembered where you hid $50.";
                    break;

                case "Wisdom Potion":
                    inventory.MortarLevel++; // Havan seviyesi atlatır
                    resultLabel.Text = "You feel incredibly wise. Your Mortar Level permanently increased!";
                    break;

                case "Holy Water Potion":
                    inventory.EmptyBottleCount += 3; // 3 tane boş şişe hediye eder
                    resultLabel.Text = "Holy light fills the room! You mysteriously found 3 Empty Bottles.";
                    break;

                case "Calm Potion":
                    resultLabel.Text = "Ah, a deep breath. You feel relaxed, but nothing special happened.";
                    break;

                default:
                    // Eğer henüz bir güç tanımlamadığımız bir iksir kullanılırsa bu yazı çıkar
                    resultLabel.Text = potionName + " used, but it has no shop effect yet.";
                    break;
            }

            usePotionPanel.Visible = false;
            RefreshAllUI();

            if (!HasAnyPotion())
                goToJungle?.Invoke();
        }

        private void RefreshAllUI()
        {
            UpdateMoneyText();
            UpdateInventoryText();
            RefreshSellPotionBox();
            RefreshUsePotionBox();
        }

        private void RefreshSellPotionBox()
        {
            sellPotionBox.Items.Clear();

            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value > 0)
                    sellPotionBox.Items.Add(new ListItem(potion.Key + " x" + potion.Value));
            }
        }

        private void RefreshUsePotionBox()
        {
            usePotionBox.Items.Clear();

            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value > 0)
                    usePotionBox.Items.Add(new ListItem(potion.Key + " x" + potion.Value));
            }
        }

        private void UpdateMoneyText()
        {
            moneyLabel.Text =
                "Money: $" + inventory.Money +
                "   Mortar Level: " + inventory.MortarLevel +
                "   Bottles: " + inventory.EmptyBottleCount;
        }

        private void UpdateInventoryText()
        {
            string text = "INVENTORY\n\nHERBS\n";

            foreach (var herb in inventory.CollectedHerbs)
            {
                text += herb.Key + ": " + herb.Value + "\n";
            }

            text += "\nPOTIONS\n";

            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value > 0)
                    text += potion.Key + ": " + potion.Value + "\n";
            }

            if (persuasionActive)
                text += "\nPersuasion: ACTIVE\n";

            inventoryLabel.Text = text;
        }

        private bool HasAnyPotion()
        {
            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value > 0)
                    return true;
            }

            return false;
        }

        private string ExtractPotionName(string text)
        {
            int index = text.LastIndexOf(" x");

            if (index <= 0)
                return text;

            return text.Substring(0, index);
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