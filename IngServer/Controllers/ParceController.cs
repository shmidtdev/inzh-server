using Cyrillic.Convert;
using HtmlAgilityPack;
using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/parce/[action]")]
public class ParceController
{
    private static readonly string _url = "https://kinlong.ru";
    
    private readonly ApplicationContext _applicationContext;

    public ParceController(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    
    [HttpGet]
    public async Task Mt()
    {
        var counter = 0;
        using StreamReader stream = new StreamReader("C:\\Users\\Kirill\\source\\repos\\Inzh-Server\\Parcer\\Links.txt");
        var links = stream.ReadToEnd().Split("\n").Skip(counter);

        
        foreach (var link in links)
        {
            counter++;
            Console.WriteLine(counter + link);
            await ParsePage(link);
        }
    }
    
    public static Task<string> GetHtml(string url)
    {
        var client = new HttpClient();

        return client.GetStringAsync(url);
    }

    public async Task ParsePage(string link)
    {
        var counter = 0;
        
        try
        {
            counter++;
            
            var html = await GetHtml(_url + link);

            var characteristics = AddCharacteristics(html);
            var category = AddCategories(html);
            var price = FindPrice(html);
            var oldPrice = FindPriceOld(html);
            var title = FindTitle(html);


            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = title,
                TitleEng = title.ToRussianLatin().Replace(',', '_').Replace(' ', '_').Replace(':', '_'),
                Category = category,
                Price = price,
                Images = null,
                Rating = 0,
                OldPrice = oldPrice,
                IsRecommended = false,
                ProductAvailability = ProductAvailability.InStock,
                Characteristics = characteristics
            };

            var otherProduct = _applicationContext.Products.FirstOrDefault(x =>
                x.TitleEng == title.ToRussianLatin().Replace(',', '_').Replace(' ', '_').Replace(':', '_'));

            if (otherProduct is null)
            {
                _applicationContext.Products.Add(product);
                _applicationContext.SaveChanges();
            }
        }
        catch
        {
            if (counter < 5)
            {
                await ParsePage(link);
            }
        }
    }

    private string FindTitle(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var titleNode =
            document.DocumentNode.SelectSingleNode("//h1[contains(concat(' ', @itemprop, ' '), 'name')]");

        var title = titleNode.InnerText.Trim();

        return title;
    }
    
    private double FindPrice(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var spanNodes =
            document.DocumentNode.SelectNodes("//span[contains(concat(' ', @class, ' '), 'value_p')]");

        if (spanNodes is null)
            return 0;

        var priceString = spanNodes.First().InnerText.Trim();
        var price = double.Parse(priceString);

        return price;
    }
    
    private double FindPriceOld(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var spanNodes =
            document.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), 'old-price')]");

        if (spanNodes is null)
            return 0;

        var priceString = spanNodes.First().InnerText.Trim().Trim("было".ToCharArray());
        var success = double.TryParse(priceString, out var price);
        if (!success)
            return 0;
        
        return price;
    }

    private Category AddCategories(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var breadcrumbNodes =
            document.DocumentNode.SelectNodes("//span[contains(concat(' ', @class, ' '), 'bx-breadcrumb-item')]");

        var relevantBreadcrumNodes = breadcrumbNodes.Take(breadcrumbNodes.Count - 1).Skip(1).ToList();

        var categories = new List<Category>();
        
        for (int i = relevantBreadcrumNodes.Count - 1; i >= 0; i--)
        {
            var isInDb = true;
            
            var name = relevantBreadcrumNodes[i].InnerText.Trim();
            var nameEng = name.ToRussianLatin().Replace(',', '_').Replace(' ', '_').Replace(':', '_');
            
            var c = _applicationContext.Categories.FirstOrDefault(x => x.NameEng == nameEng);
            if (c is null)
            {
                isInDb = false;
                
                c = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    NameEng = nameEng,
                    Image = null,
                    Children = new ()
                };
            }

            if (i != relevantBreadcrumNodes.Count - 1)
            {
                var childName = relevantBreadcrumNodes[i + 1].InnerText.Trim().ToRussianLatin()
                    .Replace(',', '_').Replace(' ', '_').Replace(':', '_');
                
                var child = _applicationContext
                    .Categories
                    .FirstOrDefault(x => x.NameEng == childName);
                
                c.Children.Add(child);
            }
            
            categories.Add(c);
            
            if(!isInDb)
                _applicationContext.Categories.Add(c);
            _applicationContext.SaveChanges();
        }

        return categories.First();
    }

    private List<Characteristic> AddCharacteristics(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var characteristicsNode =
            document.DocumentNode.SelectNodes("//ul[contains(concat(' ', @class, ' '), 'characters')]//li");

        var characteristics = new List<Characteristic>();

        if (characteristicsNode is null)
            return characteristics;
        
        foreach (var characteristic in characteristicsNode)
        {
            var characteriscicsDocument = new HtmlDocument();
            characteriscicsDocument.LoadHtml(characteristic.InnerHtml);

            var spans = characteriscicsDocument.DocumentNode.SelectNodes("//span");
            var name = spans[0].InnerHtml;
            var value = spans[1].InnerHtml;

            var nameEng = name.ToRussianLatin().Replace(',', '_').Replace(' ', '_').Replace(':', '_');
            var valueEng = value.ToRussianLatin().Replace(',', '_').Replace(' ', '_').Replace(':', '_');

            var c = _applicationContext.Characteristics.FirstOrDefault(x =>
                x.NameEng == nameEng && x.ValueEng == valueEng);

            if (c is null)
            {
                c = new Characteristic
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Value = value,
                    NameEng = nameEng,
                    ValueEng = valueEng,
                    Products = new List<Product>()
                };

                _applicationContext.Characteristics.Add(c);
                _applicationContext.SaveChanges();

                Console.WriteLine(c.NameEng + " " + c.ValueEng);
            }
            
            characteristics.Add(c);
        }

        return characteristics;
    }
}