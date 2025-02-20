using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Game game = new Game();
        await game.StartGameAsync();
    }
}

class Game
{
    private int CurrentLevel { get; set; } = 1;
    private Player Player { get; set; }
    private ApiShop Shop { get; set; } = new ApiShop();
    private Jury Jury { get; set; }

    public async Task StartGameAsync()
    {
        Console.WriteLine("Välkommen till matlagningsspelet!");
        await Shop.LoadIngredientsAsync(); // Laddar Ingredienser från API

        while (CurrentLevel <= 3) //main loopen i levels
        {
            InitializeLevel(); //sätter up nivåerna 
            await PlayLevelAsync(); //spelaren väljer rätter
            CurrentLevel++; //lägger till 1 nivå ifrån vad som fanns (tills man kommer till 3)
        }
        Console.WriteLine("Spelet är slut! Tack för att du spelade!"); //när man är på nivå 3 och man är klar kommer det här up
        Console.ReadLine();
    }

    private void InitializeLevel()
    {
        decimal budget = CurrentLevel switch { 1 => 100, 2 => 150, 3 => 200, _ => 100 }; //_ vissar att om det är något fel och det går över går den till defult = 100
        Player = new Player(budget);
        Jury = new Jury(CurrentLevel);

        Console.WriteLine($"\n=== NIVÅ {CurrentLevel} ===");
        Console.WriteLine($"Budget: {budget} kr");
        Console.WriteLine($"Juryns favoritsmaker: {string.Join(", ", Jury.PreferredTastes)}");
    }

    private async Task PlayLevelAsync()
    {
        Shop.DisplayIngredients();

        while (true)
        {
            Console.Write("Vilken ingrediens vill du köpa? (skriv 'klar' när du är färdig): ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (input == "klar") break;

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

        Dish dish = new Dish { Ingredients = Player.Inventory };
        int score = Jury.EvaluateDish(dish);
        Console.WriteLine($"Din rätt fick {score} poäng!\n");
        Console.ReadLine();
    }
}

// ---------- NYA KLASSER ----------

// Klass för spelaren
class Player
{
    public decimal Budget { get; set; }
    public List<Ingredient> Inventory { get; set; } = new List<Ingredient>();

    public Player(decimal budget)
    {
        Budget = budget; //nyligen skapade spelaren får rätt budget
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

// Klass för ingredienser
class Ingredient
{
    public string Name { get; } //namnet på ingrediensen
    public decimal Price { get; } //priset på ingrediensen
    public List<string> TasteProfile { get; } //Taste...

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

// Klass för att representera en rätt
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

// Klass för Juryn
class Jury
{
    public List<string> PreferredTastes { get; set; }

    public Jury(int level)
    {
        PreferredTastes = level switch
        {
            1 => new List<string> { "söt", "salt" },
            2 => new List<string> { "bitter", "sur", "rökig" },
            3 => new List<string> { "söt", "bitter", "salt", "saftig" },
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
