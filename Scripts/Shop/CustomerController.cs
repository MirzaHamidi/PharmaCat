using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    public partial class CraftGreyboxSystem
    {
        private Rectangle GetPanjurRopeButton()
        {
            return new Rectangle(
                (int)panjurRopePosition.X + ropeButtonOffsetX,
                (int)panjurRopePosition.Y + ropeButtonOffsetY,
                ropeButtonWidth,
                ropeButtonHeight
            );
        }

        private void HandlePanjurButton(GameTime gameTime, Point mp)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (panjurButtonVisible && LeftPressed() && panjurButton.Contains(mp))
            {
                panjurOpening = true;
                panjurButtonVisible = false;
                
                itemsSlidingOut = true;

                currentEmotion = CustomerEmotion.Neutral;
            }

            if (panjurOpening)
            {
                panjurPosition.Y -= panjurSpeed * dt;

                if (panjurPosition.Y <= panjurOpenY)
                {
                    panjurPosition.Y = panjurOpenY;
                    panjurOpening = false;

                    ropeVisible = true;
                    ropeComingDown = true;
                    ropeButtonActive = false;
                }
            }

            if (ropeComingDown)
            {
                panjurRopePosition.Y += ropeSpeed * dt;

                if (panjurRopePosition.Y >= ropeDownY)
                {
                    panjurRopePosition.Y = ropeDownY;
                    ropeComingDown = false;

                    ropeButtonActive = true;
                }
            }

            if (ropeButtonActive && LeftPressed() && GetPanjurRopeButton().Contains(mp))
            {
                ropeButtonActive = false;
                ropeGoingUp = true;
                panjurClosing = true;
                
            }

            if (ropeGoingUp)
            {
                panjurRopePosition.Y -= ropeSpeed * dt;

                if (panjurRopePosition.Y <= ropeHiddenY)
                {
                    panjurRopePosition.Y = ropeHiddenY;
                    ropeGoingUp = false;
                    ropeVisible = false;
                }
            }

            if (panjurClosing)
            {
                panjurPosition.Y += panjurSpeed * dt;

                if (panjurPosition.Y >= panjurClosedY)
                {
                    panjurPosition.Y = panjurClosedY;
                    panjurClosing = false;
                    panjurButtonVisible = true;
                    
                    currentCustomer = null;
                    dialogue.Clear();
                    introDialogueStarted = false;
                    OnShopFinished?.Invoke();
                }
            }
        }

        public void RefreshFromInventory()
        {
            CreateJarsFromInventory();
            CreateGlassesFromInventory();

            currentCustomer = new Customers();
            currentCharacter = customerCharacters[random.Next(customerCharacters.Length)];
            currentEmotion = CustomerEmotion.Silhouette;

            slideOffset = 0f;
            itemsSlidingOut = false;
            panjurButtonVisible = true;
            ropeVisible = false;
            ropeButtonActive = false;
            panjurOpening = false;
            panjurClosing = false;
            panjurPosition.Y = panjurClosedY;
            panjurRopePosition.Y = ropeHiddenY;
            
            isCustomerLeaving = false;
            isCustomerEntering = false;
            customerLeaveTimer = 0f;
            customerXOffset = 0f;
            introDialogueStarted = false;
            dialogue.Clear();
        }

        private void HandleGlassDrag(Point mp)
        {
            if (currentCustomer == null || currentEmotion == CustomerEmotion.Silhouette || isCustomerLeaving || isCustomerEntering || dialogue.IsTyping) 
            {
                return;
            }

            if (LeftPressed())
            {
                foreach (var glass in glasses)
                {
                    if (glass.IsFilled && glass.Bounds.Contains(mp))
                    {
                        draggedGlass = glass;
                        break;
                    }
                }
            }

            if (draggedGlass != null && mouse.LeftButton == ButtonState.Pressed)
            {
                draggedGlass.DragPosition = new Vector2(mp.X - 50, mp.Y - 50);
            }

            if (draggedGlass != null && LeftReleased())
            {
                Rectangle dragRect = new Rectangle(
                    (int)draggedGlass.DragPosition.X, 
                    (int)draggedGlass.DragPosition.Y, 
                    draggedGlass.Bounds.Width, 
                    draggedGlass.Bounds.Height
                );
                
                Rectangle sellZone = new Rectangle(customerRect.X, customerRect.Y, customerRect.Width, 400);

                if (dragRect.Intersects(sellZone))
                {
                    if (OnSellAttempt != null)
                    {
                        OnSellAttempt(draggedGlass);
                    }
                    else
                    {
                        ResolveSale(draggedGlass, 25);
                    }
                }
                
                draggedGlass.DragPosition = Vector2.Zero;
                draggedGlass = null;
            }
        }

        public void ResolveSale(PotionGlassBox glass, int price)
        {
            if (isCustomerLeaving || isCustomerEntering || dialogue.IsTyping)
                return;

            if (currentCustomer == null || glass == null || !glass.IsFilled)
                return;

            bool correctPotion = currentCustomer.AcceptsPotion(glass.PotionName);
            bool affordable = price <= currentCustomer.MaxPrice;

            if (correctPotion && affordable)
            {
            currentEmotion = CustomerEmotion.Happy;
            inventory.AddMoney(price);
            inventory.RemovePotion(glass.PotionName, 1);

            currentCustomer.MarkPotionBought(glass.PotionName);
            glass.IsFilled = false;
            glass.PotionName = "";
            glass.FillColor = Color.Transparent;

            CreateGlassesFromInventory();

            StartResultDialogue("Thank you! This should solve my problem.", true);
            }
            else if (!correctPotion && persuasionActive)
            {
            int maxPersuasionPrice = currentCustomer.MaxPrice + 25;

            if (price <= maxPersuasionPrice)
            {
            currentEmotion = CustomerEmotion.Happy;
            inventory.AddMoney(price);
            inventory.RemovePotion(glass.PotionName, 1);

            currentCustomer.MarkPotionBought(glass.PotionName);
            glass.IsFilled = false;
            glass.PotionName = "";
            glass.FillColor = Color.Transparent;

            CreateGlassesFromInventory();

            persuasionActive = false;

            StartResultDialogue("Wrong potion sold with persuasion. You earned $" + price + ".", true);
            }
            else
            {
            currentEmotion = CustomerEmotion.Angry;
            persuasionActive = false;

            StartResultDialogue("Too expensive. Even persuasion could not save this scam.", false);
            }
            }
            else
            {
            currentEmotion = CustomerEmotion.Angry;

            if (!correctPotion)
            {
                StartResultDialogue("Is this a joke? That's not what I need!", false);
            }
            else
            {
        StartResultDialogue("This could help, but it is too expensive!", false);
            }
            }
        }

        private void StartIntroDialogue()
        {
            if (currentCustomer == null || introDialogueStarted)
            return;

            introDialogueStarted = true;

            string text =
            currentCustomer.CurrentDialogue +
            "\nSolutions: " +
            currentCustomer.GetAcceptablePotionText();

            dialogue.Start(text, DialogueMood.Intro, false);

            dialogueBubbleInitialized = false;
        }

        private void StartResultDialogue(string text, bool happyResult)
        {
        DialogueMood mood = happyResult ? DialogueMood.HappyResult : DialogueMood.AngryResult;

        dialogue.Start(text, mood, true);

        dialogueBubbleInitialized = false;
        }

        private Texture2D GetCurrentCustomerTexture()
        {
            if (string.IsNullOrEmpty(currentCharacter))
                return texCustBase["a"];

            if (dialogue.IsTyping)
            {
                if (dialogue.Mood == DialogueMood.Intro || dialogue.Mood == DialogueMood.HappyResult)
                    return dialogue.FaceToggle ? texCustHappy[currentCharacter] : texCustBase[currentCharacter];

                if (dialogue.Mood == DialogueMood.AngryResult)
                    return dialogue.FaceToggle ? texCustAngry[currentCharacter] : texCustBase[currentCharacter];
            }

            if (currentEmotion == CustomerEmotion.Happy)
                return texCustHappy[currentCharacter];

            if (currentEmotion == CustomerEmotion.Angry)
                return texCustAngry[currentCharacter];

            return texCustBase[currentCharacter];
        }

        private void DrawDialogueBubble(SpriteBatch sb, Rectangle currentCustRect, Color color)
{
    if (string.IsNullOrEmpty(dialogue.VisibleText))
        return;

    string measureText = dialogue.VisibleText + "        ";
string wrappedText = WrapText(measureText, DialogueBubbleMaxWidth - DialogueBubblePaddingX * 2);
    Vector2 textSize = font.MeasureString(wrappedText);

    int targetWidth = (int)MathHelper.Clamp(
        textSize.X + DialogueBubblePaddingX * 2,
        DialogueBubbleMinWidth,
        DialogueBubbleMaxWidth
    );

    int targetHeight = (int)MathHelper.Clamp(
        textSize.Y + DialogueBubblePaddingY * 2,
        DialogueBubbleMinHeight,
        DialogueBubbleMaxHeight
    );

    targetDialogueBubbleRect = new Rectangle(
        currentCustRect.Right - 55,
        currentCustRect.Top + 35,
        targetWidth,
        targetHeight
    );

    if (!dialogueBubbleInitialized)
    {
        currentDialogueBubbleRect = new Rectangle(
            targetDialogueBubbleRect.X,
            targetDialogueBubbleRect.Y,
            DialogueBubbleMinWidth,
            DialogueBubbleMinHeight
        );

        dialogueBubbleInitialized = true;
    }

    currentDialogueBubbleRect = LerpRectangle(
        currentDialogueBubbleRect,
        targetDialogueBubbleRect,
        0.27f);

    sb.Draw(pixel, currentDialogueBubbleRect, Color.Black * 0.72f);

    sb.Draw(pixel, new Rectangle(currentDialogueBubbleRect.X, currentDialogueBubbleRect.Y, currentDialogueBubbleRect.Width, 3), color);
    sb.Draw(pixel, new Rectangle(currentDialogueBubbleRect.X, currentDialogueBubbleRect.Bottom - 3, currentDialogueBubbleRect.Width, 3), color);
    sb.Draw(pixel, new Rectangle(currentDialogueBubbleRect.X, currentDialogueBubbleRect.Y, 3, currentDialogueBubbleRect.Height), color);
    sb.Draw(pixel, new Rectangle(currentDialogueBubbleRect.Right - 3, currentDialogueBubbleRect.Y, 3, currentDialogueBubbleRect.Height), color);
}

        private Rectangle LerpRectangle(Rectangle from, Rectangle to, float amount)
        {
        return new Rectangle(
        (int)MathHelper.Lerp(from.X, to.X, amount),
        (int)MathHelper.Lerp(from.Y, to.Y, amount),
        (int)MathHelper.Lerp(from.Width, to.Width, amount),
        (int)MathHelper.Lerp(from.Height, to.Height, amount)
        );
        }

        private string WrapText(string text, float maxLineWidth)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            string[] rawLines = text.Split('\n');
            string result = "";

            foreach (string rawLine in rawLines)
            {
                string[] words = rawLine.Split(' ');
                string line = "";

                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;

                    if (font.MeasureString(testLine).X > maxLineWidth)
                    {
                        result += line + "\n";
                        line = word;
                    }
                    else
                    {
                        line = testLine;
                    }
                }

                result += line + "\n";
            }

            return result.TrimEnd();
        }
    }

}
