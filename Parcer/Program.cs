using HtmlAgilityPack;
using IngServer.DataBase;
using IngServer.DataBase.Models;

namespace Parcer;

public class Program
{
    private static readonly string _url = "https://kinlong.ru";
    private static List<string> Links = new List<string>();
    private static ApplicationContext _applicationContext;

    public Program(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    
    public static async Task Main()
    {
        // using StreamWriter stream =
        //     new StreamWriter("C:\\Users\\Kirill\\source\\repos\\Inzh-Server\\Parcer\\Links.txt");
        //
        // var html = await GetHtml(_url + "/catalog");
        //
        // var document = new HtmlDocument();
        // document.LoadHtml(html);
        //
        // var menuHtml = document.DocumentNode
        //     .SelectSingleNode("//div[contains(concat(' ', @class, ' '), 'cat_panel')]").InnerHtml;
        // document.LoadHtml(menuHtml);
        //
        // var categories = document.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), 'product')]")
        //     .ToList();
        // foreach (var category in categories)
        // {
        //     var categoryDocument = new HtmlDocument();
        //     categoryDocument.LoadHtml(category.InnerHtml);
        //
        //     var link = categoryDocument.DocumentNode.SelectSingleNode("//a").Attributes.AttributesWithName("href")
        //         .First().Value;
        //
        //     var categoryPage = await GetHtml(_url + link);
        //     categoryDocument.LoadHtml(categoryPage);
        //
        //     var pagesNode = categoryDocument.DocumentNode
        //         .SelectSingleNode("//ul[contains(concat(' ', @class, ' '), 'pagination')]");
        //     
        //     if(pagesNode is null)
        //         await ProcessPage(link);
        //     else
        //     {
        //         var pagesDocument = new HtmlDocument();
        //         pagesDocument.LoadHtml(pagesNode.InnerHtml);
        //
        //         var pages = pagesDocument.DocumentNode.SelectNodes("//li");
        //
        //         var counter = 1;
        //         
        //         foreach (var page in pages)
        //         {
        //             var isSuccess = int.TryParse(page.SelectSingleNode("//span").InnerHtml.Trim('\n').Trim(), out var number);
        //             if(!isSuccess)
        //                 continue;
        //             
        //             var href = link + $"index.php?PAGEN_2={counter}"; 
        //
        //             await ProcessPage(href);
        //             
        //             counter++;
        //         }
        //     }
        // }
        //
        // var finalLinks = Links.Distinct();

        using StreamReader stream = new StreamReader("C:\\Users\\Kirill\\source\\repos\\Inzh-Server\\Parcer\\Links.txt");
        var links = stream.ReadToEnd().Split("\n");

        foreach (var link in links)
        {
            await ParsePage(link);
        }
        
        stream.Close();
    }

    public static Task<string> GetHtml(string url)
    {
        var client = new HttpClient();

        return client.GetStringAsync(url);
    }

    public static async Task ProcessPage(string link)
    {
        var productPage = await GetHtml(_url + link);

        var productDocument = new HtmlDocument();
        productDocument.LoadHtml(productPage);
                    
        var catPanel =
            productDocument.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), 'product')]");

        foreach (var node in catPanel)
        {
            var nodeDocument = new HtmlDocument();
            nodeDocument.LoadHtml(node.InnerHtml);

            var nodeLink = nodeDocument.DocumentNode.SelectSingleNode("//a").Attributes.AttributesWithName("href")
                .First().Value!;
            
            Links.Add(nodeLink);

            //stream.WriteLine(nodeLink);

            Console.WriteLine(nodeLink);
        }
    }

    public static async Task ParsePage(string link)
    {
        var html = await GetHtml(_url + link);
        
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var characteristicsNode =
            document.DocumentNode.SelectNodes("//ul[contains(concat(' ', @class, ' '), 'characters')]//li");

        foreach (var node in characteristicsNode)
        {
            var characteriscicsDocument = new HtmlDocument();
            characteriscicsDocument.LoadHtml(node.InnerHtml);

            var spans = characteriscicsDocument.DocumentNode.SelectNodes("//span");
            var name = spans[0].InnerHtml;
            var value = spans[1].InnerHtml;

            var smt = _applicationContext.Characteristics.First();
        }
    }
}