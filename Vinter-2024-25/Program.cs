using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Skapa en shop/butik
        Shop shop = new Shop();
        Player player = new Player(100); // Spelaren börjar med 100 kr

        // Visar ingredienser som man kan köpa
        shop.DisplayIngredients();

        // Spelaren köper ingredienser
        Console.WriteLine("Vilken ingrediens vill du köpa? (skriv 'klar' när du är färdig)");
        while (true)
        {
            string input = Console.ReadLine();
            if (input.ToLower() == "klar") break;

            Ingredient chosenIngredient = shop.BuyIngredient(input);
            if (chosenIngredient != null && player.CanAfford(chosenIngredient.Price))
            {
                player.AddIngredient(chosenIngredient);
                Console.WriteLine($"Du har lagt till {chosenIngredient.Name} i ditt inventory.");
            }
            else
            {
                Console.WriteLine("Ogiltigt val eller du har inte tillräckligt med pengar.");
            }
        }

        // Visa spelarens inventory
        Console.WriteLine("\nDitt inventory:");
        foreach (var ingredient in player.Inventory)
        {
            Console.WriteLine(ingredient.GetDescription());
        }
        Console.ReadLine(); //för att vissa inventory efter "klar"
    }
}

class Ingredient
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Ingredient(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public string GetDescription()
    {
        return $"{Name} ({Price} kr)";
    }
}

class Player
{
    public decimal Budget { get; set; }
    public List<Ingredient> Inventory { get; set; } = new List<Ingredient>();

    public Player(decimal budget)
    {
        Budget = budget;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        Inventory.Add(ingredient);
        Budget -= ingredient.Price;
    }

    public bool CanAfford(decimal price)
    {
        return Budget >= price;
    }
}

class Shop
{
    public List<Ingredient> AvailableIngredients { get; set; } = new List<Ingredient>();

    public Shop()
    {
        // Lägg till några grundläggande ingredienser
        AvailableIngredients.Add(new Ingredient("Tomat", 20));
        AvailableIngredients.Add(new Ingredient("Salt", 10));
        AvailableIngredients.Add(new Ingredient("Peppar", 15));
    }

    public void DisplayIngredients()
    {
        Console.WriteLine("Tillgängliga ingredienser:");
        foreach (var ingredient in AvailableIngredients)
        {
            Console.WriteLine(ingredient.GetDescription());
        }
    }

    public Ingredient BuyIngredient(string name)
    {
        return AvailableIngredients.Find(i => i.Name.ToLower() == name.ToLower());
    }
}