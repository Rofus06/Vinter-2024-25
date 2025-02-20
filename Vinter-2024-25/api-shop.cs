using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; //install in nuget Newtonsoft.Json

class ApiShop //fick hjälp av att sätta up api ifrån den här vid: https://www.youtube.com/watch?v=Flap5M630vk
{
    private static readonly string apiUrl = "https://67a9c2866e9548e44fc4bb32.mockapi.io/api/v1/FoodList"; //https://mockapi.io/projects/67a9c2866e9548e44fc4bb33#
    private List<Ingredient> ingredients = new List<Ingredient>();

    public async Task LoadIngredientsAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string response = await client.GetStringAsync(apiUrl);
                List<ApiIngredient> apiIngredients = JsonConvert.DeserializeObject<List<ApiIngredient>>(response); //hjälp: https://stackoverflow.com/questions/56926237/how-to-deserialize-a-json-response-to-an-object-list

                ingredients.Clear(); //clear om något är fel med api sakerna
                foreach (var apiIngredient in apiIngredients) //hämta api ingredients
                {
                    ingredients.Add(new Ingredient(apiIngredient.name, Convert.ToDecimal(apiIngredient.price), //omvandlat från string till decimal t.ex "10" till 10
                        new List<string>(apiIngredient.tastes.Split(',')))); //hämtar smaker och splitrar dom, t.ex "saltig,syrlig" till en array ["saltig", "syrlig"]
                } 

                Console.WriteLine("Ingredienser hämtade från API!");

                //Hur det skulle kunna see ut uttan converted:
                //{ "name": "Salt", "price": "10.5", "tastes": "saltig" },
                //{ "name": "Citron", "price": "5.0", "tastes": "syrlig" },
                //{ "name": "Honung", "price": "8.0", "tastes": "söt,klibbig" }

                //Hur det ser ut nu med converted:
                //Salt (10.5 kr) - Smaker: saltig
                //Citron (5.0 kr) - Smaker: syrlig
                //Honung (8.0 kr) - Smaker: söt, klibbig
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid hämtning av ingredienser: {ex.Message}");
            }
        }
    }

    public void DisplayIngredients()
    {
        Console.WriteLine("\nButiken har:");
        foreach (var ingredient in ingredients)
        {
            Console.WriteLine(ingredient.GetDescription());
        }
    }

    public Ingredient BuyIngredient(string name)
    {
        return ingredients.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); //jag använde chatgpt (för jag satt fast, visste ej hur jag skulle göra det här)
    }
}

class ApiIngredient 
{
    public string id { get; set; } //vad som ska vissas först (ngl behöver inte äns för jag använde inte den här)
    public string name { get; set; } //namnet på Ingredientsen
    public string price { get; set; } //pricet på Ingredientsen
    public string tastes { get; set; } //smaken på Ingredientsen
}
