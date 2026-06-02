using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class CraftingScene
    {
        private Desktop craftingDesktop;

        private Panel choicePanel;
        private Panel sellPanel;

        private TextBox priceInput;
        private PotionGlassBox currentPotionGlass;

        public Action<PotionGlassBox, int> OnSellConfirmed;
        public Action<PotionGlassBox> OnUseConfirmed;

        public void Load() 
        {
        var panel = new Panel();

        CreateChoicePanel();
        CreateSellPanel();

        panel.Widgets.Add(choicePanel);
        panel.Widgets.Add(sellPanel);

        craftingDesktop = new Desktop();
        craftingDesktop.Root = panel;
        }
        private void CreateChoicePanel()
{
    choicePanel = new Panel
    {
        Left = 1920 / 2 - 170,
        Top = 1080 / 2 - 120,
        Width = 340,
        Height = 240,
        Visible = false,
        Background = new Myra.Graphics2D.Brushes.SolidBrush(Color.Black * 0.85f)
    };

    var title = new Label
    {
        Text = "Potion Action",
        Left = 20,
        Top = 20,
        Width = 280,
        Height = 35
    };

    var label = new Label
    {
        Text = "Use or sell this potion?",
        Left = 20,
        Top = 60,
        Width = 280,
        Height = 35
    };

    var useButton = new TextButton
    {
        Text = "Use Potion",
        Left = 50,
        Top = 105,
        Width = 240,
        Height = 35
    };

    useButton.Click += (s, a) =>
    {
        choicePanel.Visible = false;

        if (currentPotionGlass != null)
        {
            OnUseConfirmed?.Invoke(currentPotionGlass);
        }

        currentPotionGlass = null;
    };

    var sellButton = new TextButton
    {
        Text = "Sell Potion",
        Left = 50,
        Top = 145,
        Width = 240,
        Height = 35
    };

    sellButton.Click += (s, a) =>
    {
        choicePanel.Visible = false;
        OpenSellUI(currentPotionGlass);
    };

    var cancelButton = new TextButton
    {
        Text = "Cancel",
        Left = 50,
        Top = 185,
        Width = 240,
        Height = 35
    };

    cancelButton.Click += (s, a) =>
    {
        choicePanel.Visible = false;
        currentPotionGlass = null;
    };

    choicePanel.Widgets.Add(title);
    choicePanel.Widgets.Add(label);
    choicePanel.Widgets.Add(useButton);
    choicePanel.Widgets.Add(sellButton);
    choicePanel.Widgets.Add(cancelButton);
}

        private void CreateSellPanel()
{
    sellPanel = new Panel
    {
        Left = 1920 / 2 - 150,
        Top = 1080 / 2 - 100,
        Width = 300,
        Height = 220,
        Visible = false,
        Background = new Myra.Graphics2D.Brushes.SolidBrush(Color.Black * 0.85f)
    };

    var title = new Label
    {
        Text = "Offer Price",
        Left = 20,
        Top = 20,
        Width = 250,
        Height = 35
    };

    var lbl = new Label
    {
        Text = "Set your price ($):",
        Left = 20,
        Top = 60,
        Width = 250,
        Height = 35
    };

    priceInput = new TextBox
    {
        Text = "20",
        Left = 20,
        Top = 100,
        Width = 250,
        Height = 35
    };

    var sellBtn = new TextButton
    {
        Text = "Offer to Customer",
        Left = 20,
        Top = 145,
        Width = 250,
        Height = 35
    };

    sellBtn.Click += (s, a) =>
    {
        if (currentPotionGlass == null)
            return;

        if (int.TryParse(priceInput.Text, out int price))
        {
            OnSellConfirmed?.Invoke(currentPotionGlass, price);
            sellPanel.Visible = false;
            currentPotionGlass = null;
        }
    };

    var cancelBtn = new TextButton
    {
        Text = "Cancel",
        Left = 20,
        Top = 185,
        Width = 250,
        Height = 30
    };

    cancelBtn.Click += (s, a) =>
    {
        sellPanel.Visible = false;
        currentPotionGlass = null;
    };

    sellPanel.Widgets.Add(title);
    sellPanel.Widgets.Add(lbl);
    sellPanel.Widgets.Add(priceInput);
    sellPanel.Widgets.Add(sellBtn);
    sellPanel.Widgets.Add(cancelBtn);
}        


public void OpenPotionChoiceUI(PotionGlassBox glass)
{
    if (glass == null || !glass.IsFilled)
        return;

    currentPotionGlass = glass;

    sellPanel.Visible = false;
    choicePanel.Visible = true;
}
        
        
        public void OpenSellUI(PotionGlassBox glass)
{
    if (glass == null || !glass.IsFilled)
        return;

    currentPotionGlass = glass;

    priceInput.Text = "20";

    choicePanel.Visible = false;
    sellPanel.Visible = true;
}

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            craftingDesktop?.Render();
        }
    }
}