using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ---------- Main Program ----------
class Program
{
    static async Task Main(string[] args)
    {
        Game game = new Game();
        await game.StartGameAsync();
    }
}

// ---------- BASKLASSER (för Arv) ---------- 

class Person
{
    public string Name { get; set; }

    public Person(string name)
    {
        Name = name;
    }
}

class Product
{
    public string Name { get; }
    public decimal Price { get; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public virtual string GetDescription()
    {
        return $"{Name} - {Price} kr";
    }
}

// ---------- SPELKLASSER ----------

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
        Player = new Player("Spelaren", budget);
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

// ---------- SPELARE ----------
class Player : Person
{
    public decimal Budget { get; private set; }
    public List<Ingredient> Inventory { get; private set; } = new List<Ingredient>();

    public Player(string name, decimal budget) : base(name)
    {
        Budget = budget;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        Inventory.Add(ingredient);
        Budget -= ingredient.Price;
    }

    public bool CanAfford(decimal price) => Budget >= price;
}

// ---------- INGREDIENSER ----------
class Ingredient : Product
{
    public List<string> TasteProfile { get; }

    public Ingredient(string name, decimal price, List<string> tasteProfile)
        : base(name, price)
    {
        TasteProfile = tasteProfile;
    }

    public override string GetDescription()
    {
        return $"{Name} ({Price} kr) - Smaker: {string.Join(", ", TasteProfile)}";
    }
}

// ---------- RÄTT ----------
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

// ---------- JURY ----------
class Jury : Person
{
    public List<string> PreferredTastes { get; set; }

    public Jury(int level) : base("Juryn")
    {
        PreferredTastes = level switch
        {
            1 => new List<string> { "söt", "salt" },
            2 => new List<string> { "bitter", "sur", "rökig" },
            3 => new List<string> { "söt", "bitter", "salt", "saftig" },
            _ => new List<string> { "söt" }
        };
    }

    public virtual int EvaluateDish(Dish dish)
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
