using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaCat.Scripts
{
    public class ShopSystem
    {
        public string SellPotion(Customers customer, string potionName, int price)
        {
            if (potionName == customer.WantedPotion)
            {
                if (price <= customer.MaxPrice)
                {
                    return $"Narrator: You sold {potionName} successfully.";
                }
                else
                {
                    return $"Narrator: The potion was correct, but too expensive.";
                }
            }

            return $"Narrator: You tried to sell {potionName} instead of {customer.WantedPotion}.";
        }
    }
}