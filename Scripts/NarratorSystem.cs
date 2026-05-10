using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaCat.Scripts
{
    public class NarratorSystem
    {
        public string GetSellResultText(Customers customer, string offeredPotion, int price)
        {
            if (offeredPotion == customer.WantedPotion)
            {
                if (price <= customer.MaxPrice)
                {
                    return $"Narrator: You sold the {offeredPotion} for {price} gold. The customer leaves happily.";
                }

                return $"Narrator: The potion was correct, but {price} gold was too expensive. The customer got annoyed.";
            }

            return $"Narrator: You tried to sell the customer a {offeredPotion} instead of {customer.WantedPotion}, so they got angry.";
        }

        public string GetWaitingText()
        {
            return "Narrator: The customer is waiting for your offer...";
        }

        public string GetNextCustomerText()
        {
            return "Narrator: A new customer enters the shop.";
        }
    }
}
    