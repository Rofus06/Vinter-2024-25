using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.StartGame();
    }
}

// ---------- KLASSER ---------- :)
class Game
{
    private int CurrentLevel { get; set; } = 1;
    private Player Player { get; set; }
    private Shop Shop { get; set; }
    private Jury Jury { get; set; }

    public void StartGame()
    {
        Console.WriteLine("Välkommen till matlagningsspelet!");
        while (CurrentLevel <= 3) // 3 nivåer
        {
            InitializeLevel();
            PlayLevel();
            CurrentLevel++;
        }
        Console.WriteLine("Spelet är slut! Tack för att du spelade!");
    }

    private void InitializeLevel()
    {
        // Sätt budget och jurypreferenser baserat på nivå
        decimal budget = CurrentLevel switch { 1 => 100, 2 => 150, 3 => 200, _ => 100 };
        Player = new Player(budget);
        Shop = new Shop();
        Jury = new Jury(CurrentLevel);

        Console.WriteLine($"\n=== NIVÅ {CurrentLevel} ===");
        Console.WriteLine($"Budget: {budget} kr");
        Console.WriteLine($"Juryns favoritsmaker: {string.Join(", ", Jury.PreferredTastes)}");
    }

    private void PlayLevel()
    {
        Shop.DisplayIngredients();

        // Spelaren köper ingredienser
        while (true)
        {
            Console.Write("Vilken ingrediens vill du köpa? (skriv 'klar' när du är färdig): ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (input == "klar") break;

            try
            {
                Ingredient chosenIngredient = Shop.BuyIngredient(input);
                if (chosenIngredient == null)
                {
                    Console.WriteLine("Ingen sådan ingrediens finns.");
                    continue;
                }

                if (Player.CanAfford(chosenIngredient.Price))
                {
                    Player.AddIngredient(chosenIngredient);
                    Console.WriteLine($"Du har lagt till {chosenIngredient.Name}.");
                }
                else
                {
                    Console.WriteLine("Du har inte råd.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel: {ex.Message}");
            }
        }

        // Jury som Bedömer rätten
        Dish dish = new Dish { Ingredients = Player.Inventory };
        int score = Jury.EvaluateDish(dish);
        Console.WriteLine($"Din rätt fick {score} poäng!\n");
        Console.ReadLine();
    }
}

// Subklasser av "Ingredient"
class Vegetable : Ingredient
{
    public Vegetable(string name, decimal price, List<string> tastes) : base(name, price, tastes) { }
}

class Spice : Ingredient
{
    public Spice(string name, decimal price, List<string> tastes) : base(name, price, tastes) { }
}

class Ingredient
{
    public string Name { get; }
    public decimal Price { get; }
    public List<string> TasteProfile { get; }

    public Ingredient(string name, decimal price, List<string> tasteProfile)
    {
        Name = name;
        Price = price;
        TasteProfile = tasteProfile;
    }

    public virtual string GetDescription() // Virtual för framtida override (gör imorgon osv)
    {
        return $"{Name} ({Price} kr) - Smaker: {string.Join(", ", TasteProfile)}";
    }
}

class Shop //vad man ser i console
{
    private Dictionary<string, Ingredient> Ingredients { get; } = new Dictionary<string, Ingredient>();

    public Shop()
    {
        // Lägg till ingredienser med Dictionary
        AddIngredient(new Vegetable("Tomat", 20, new List<string> { "söt", "sur" }));
        AddIngredient(new Spice("Salt", 10, new List<string> { "salt" }));
        AddIngredient(new Vegetable("Lök", 25, new List<string> { "bitter", "söt" }));
    }

    private void AddIngredient(Ingredient ingredient)
    {
        Ingredients[ingredient.Name.ToLower()] = ingredient;
    }

    public void DisplayIngredients()
    {
        Console.WriteLine("\nButiken har:");
        foreach (var ingredient in Ingredients.Values)
        {
            Console.WriteLine(ingredient.GetDescription());
        }
    }

    public Ingredient BuyIngredient(string name)
    {
        Ingredients.TryGetValue(name.ToLower(), out Ingredient ingredient);
        return ingredient;
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

    public Jury(int level)
    {
        // Sätt jurypreferenser baserat på nivå
        PreferredTastes = level switch
        {
            1 => new List<string> { "söt", "salt" },
            2 => new List<string> { "bitter", "sur" },
            3 => new List<string> { "söt", "bitter", "salt" },
            _ => new List<string> { "söt" }
        };
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