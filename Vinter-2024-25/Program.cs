using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Skapa en butik, en spelare och en jury
        Shop shop = new Shop();
        Player player = new Player(100); // Spelaren börjar med 100 kr
        Jury jury = new Jury(new List<string> { "söt", "salt" }); // Juryns preferenser

        // Visa tillgängliga ingredienser
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
            Console.ReadLine();
        }

        // Skapar en rätt och (försöker) bedöma den!
        Dish dish = new Dish { Ingredients = player.Inventory };
        int score = jury.EvaluateDish(dish);

        Console.WriteLine($"Din rätt fick {score} poäng av juryn!");
        Console.ReadLine();
    }
}

class Ingredient
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<string> TasteProfile { get; set; }

    public Ingredient(string name, decimal price, List<string> tasteProfile)
    {
        Name = name;
        Price = price;
        TasteProfile = tasteProfile;
    }

    public string GetDescription()
    {
        return $"{Name} ({Price} kr) - Smaker: {string.Join(", ", TasteProfile)}";
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
        AvailableIngredients.Add(new Ingredient("Tomat", 20, new List<string> { "söt", "sur" }));
        AvailableIngredients.Add(new Ingredient("Salt", 10, new List<string> { "salt" }));
        AvailableIngredients.Add(new Ingredient("Peppar", 15, new List<string> { "bitter" }));
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

class Dish
{
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public List<string> GetTasteProfile()
    {
        List<string> allTastes = new List<string>();
        foreach (var ingredient in Ingredients)
        {
            allTastes.AddRange(ingredient.TasteProfile);
        }
        return allTastes;
    }
}

class Jury
{
    public List<string> PreferredTastes { get; set; }

    public Jury(List<string> preferredTastes)
    {
        PreferredTastes = preferredTastes;
    }

    public int EvaluateDish(Dish dish)
    {
        int score = 0;
        foreach (var taste in dish.GetTasteProfile())
        {
            if (PreferredTastes.Contains(taste))
            {
                score++;
            }
        }
        return score;
    }
}