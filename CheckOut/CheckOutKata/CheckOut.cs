using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace CheckOutKata
{
    public class CheckOut
    {
        #region Private Attributes 
        private static ObservableCollection<KeyValuePair<string, decimal>> _items;
        private static ObservableCollection<Offers> _offers;
        private static ObservableCollection<Basket> _basket;

        #endregion

        #region Public Attributes 
        public static ObservableCollection<KeyValuePair<string, decimal>> Items => _items;
        public static ObservableCollection<Offers> Offers => _offers;
        public static ObservableCollection<Basket> Basket => _basket;

        #endregion

        private static void Main()
        {
            Initialise();
            MenuNavigation();        
        }

        #region Private Methods      
        private static void Initialise()
        {
            _items = new ObservableCollection<KeyValuePair<string, decimal>>();
            _basket = new ObservableCollection<Basket>();
            _offers = new ObservableCollection<Offers>();

            _items.Clear();
            _basket.Clear();
            _offers.Clear();

            _items.Add(new KeyValuePair<string, decimal>("A99", 0.50m));
            _items.Add(new KeyValuePair<string, decimal>("B15", 0.30m));
            _items.Add(new KeyValuePair<string, decimal>("C40", 0.60m));

            _offers.Add(new Offers { Barcode = "A99", QuantityRequired = 3, OfferPrice = 1.30m});
            _offers.Add(new Offers { Barcode = "B15", QuantityRequired = 2, OfferPrice = 0.45m});       
        }

        private static void MenuNavigation()
        {
            while (true)
            {
                if (_basket.Count < 1)
                {
                    Console.WriteLine("------------ Welcome to the Checkout Till ------------");
                    Console.WriteLine("------------------------------------------------------\n");
                    Console.WriteLine("Please Input a Barcode...");
                    var inputtedBarcode = Console.ReadLine();
                    Scan(inputtedBarcode);
                }
                else
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Would you like to add more items to Basket (B) or go to CheckOut (C)");
                    Console.WriteLine("Please Choose option B/C");
                    var inputtedOption = Console.ReadLine();

                    switch (inputtedOption)
                    {
                        case "B":
                            Console.WriteLine("Please input a Barcode..");
                            var inputtedBarcode = Console.ReadLine();
                            Scan(inputtedBarcode);
                            break;

                        case "C":
                            CheckOutFinish();
                            break;

                        default:
                            Console.WriteLine("Invalid option please try again.");
                            continue;
                    }
                }

                break;
            }
        }

        private static void Scan(string barcode)
        {
            if (_items.All(x => x.Key != barcode.ToUpper()))
            {
              Console.WriteLine("This Barcode is not within our system please try again!");
              var inputtedBarcode = Console.ReadLine();
              Scan(inputtedBarcode);
            }

            var item = _items.FirstOrDefault(x => x.Key == barcode.ToUpper());

            Console.WriteLine("Please input the quantity of {0} that your would like to add to your basket.", barcode);
            var inputtedStringQuantity = Console.ReadLine();

            if (int.TryParse(inputtedStringQuantity, out var inputtedQuantity))
            {           
                var basketItem = _basket.FirstOrDefault(x => x.Barcode == barcode);
                if (basketItem != null)
                {
                    basketItem.Quantity = basketItem.Quantity + inputtedQuantity;
                }
                else
                {
                    _basket.Add(new Basket { Barcode = barcode.ToUpper(), Quantity = inputtedQuantity, PriceOfItem = item.Value });                   
                }

                MenuNavigation();
            }
            else
            {
                Console.WriteLine("Please input a valid quantity.");
                Scan(barcode);
            }
        }

        private static void CheckOutFinish()
        {
            decimal total = 0;
            Console.WriteLine("\n");
            Console.WriteLine("-------------------- Your Basket --------------------");
            Console.WriteLine("-----------------------------------------------------\n");

            // Check For Any Discounts
            foreach (var offer in _offers)
            {
                foreach (var item in (_basket.Where(x => x.Barcode == offer.Barcode)))
                {
                    var numberOfDiscountsAchieved = item.Quantity / offer.QuantityRequired;
                    if (numberOfDiscountsAchieved >= 1)
                    {
                      Console.WriteLine("You have achieved {0} Offer(s) for {1}.", numberOfDiscountsAchieved, item.Barcode);
                    }

                    int remainderNotDiscounted = item.Quantity - (numberOfDiscountsAchieved * offer.QuantityRequired);
                    decimal costWithDiscount = numberOfDiscountsAchieved * offer.OfferPrice;
                    decimal costWithoutDiscount = remainderNotDiscounted * item.PriceOfItem;
                    decimal totalItemCost = costWithDiscount + costWithoutDiscount;
                    Console.WriteLine("Total Cost £{0} of {1} ", totalItemCost, item.Barcode);
                    Console.WriteLine("\n");
                    total += totalItemCost;
                }               
            }

            Console.WriteLine("Total Cost of all Items: £ {0}", total);
            Console.ReadLine();        
        }
        #endregion
    }
}
