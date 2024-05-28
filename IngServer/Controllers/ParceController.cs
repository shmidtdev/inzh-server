using System.Net.Mime;
using Cyrillic.Convert;
using HtmlAgilityPack;
using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace IngServer.Controllers;

[Route("api/parce/[action]")]
public class ParceController
{
    private static readonly string _url = "https://kinlong.ru";
    
    private readonly ApplicationContext _applicationContext;
    private readonly CategoryRepository _categoryRepository;
    private readonly ProductRepository _productRepository;
    private readonly CharacteristicRepository _characteristicRepository;
    
    public ParceController(ApplicationContext applicationContext, 
        CategoryRepository categoryRepository, 
        ProductRepository productRepository, 
        CharacteristicRepository characteristicRepository)
    {
        _applicationContext = applicationContext;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _characteristicRepository = characteristicRepository;
    }

    [HttpGet]
    public async Task Characteristics()
    {
        var smt =  _applicationContext.CategoryInfos.Include(x => x.Characteristics)
            .Where(x => x.Characteristics.Count > 0).ToList();
        
        var categoryInfos = _applicationContext.CategoryInfos.Include(c => c.Category).ToList();

        var counter = 0;
        
        foreach (var info in categoryInfos)
        {
            
            var products = _applicationContext.Products.Where(x => x.Category == info.Category)
                .Include(product => product.Characteristics).ToList();

            info.Characteristics = products.SelectMany(x => x.Characteristics).ToList();
            Console.WriteLine($"{info.Category.Name} | characteristics-{info.Characteristics.Count} | {counter++}");
            
            _applicationContext.CategoryInfos.Update(info);
        }
        
        await _applicationContext.SaveChangesAsync();
        Console.WriteLine("end");
    }

    [HttpGet]
    public async Task CategoryInfos()
    {
        var smt = _applicationContext.CategoryInfos
            .Include(x => x.Characteristics)
            .FirstOrDefault(x => x.Category.NameEng == "Catalog");
        
        var categories = _applicationContext.Categories.Include(x => x.Children).ToList();
    
        var counter = 0;
    
        var ci = _applicationContext.CategoryInfos;
        _applicationContext.CategoryInfos.RemoveRange(ci);
        _applicationContext.SaveChanges();
        
        foreach (var category in categories)
        {
            counter++;
            Console.WriteLine($"{category.Name} | {counter}");
            
            var bottomChildren = await _categoryRepository.GetBottomChildrenAsync(category);
            var bc = bottomChildren.Select(x => new CategoryInfo
            {
                Id = Guid.NewGuid(),
                Category = x
            });

            var amount = 0;

            //var characteristics = _applicationContext.Characteristics.Where(x => x.Products.Any(y => y.Category.NameEng == category.NameEng)).ToList();
            
            foreach (var item in bottomChildren)
            {
                var products = _applicationContext.Products.Include(c => c.Characteristics)
                    .Where(x => x.Category.Id == item.Id).ToList();

                //var tmp = _applicationContext.Characteristics.Include(x => x.Products).Where(x => x.Products.Any(y => products.Contains(y))).ToList();
                //var tmp = products.SelectMany(_characteristicRepository.GetCharacteristics).ToList();
                
                // _applicationContext.CategoryInfos.Add(new CategoryInfo
                // {
                //     Id = Guid.NewGuid(),
                //     Category = item,
                //     BottomChildren = null,
                //     Characteristics = tmp,
                //     AmountOfProducts = products.Count
                // });

                amount += products.Count;
            }
            
            _applicationContext.CategoryInfos.Add(new CategoryInfo
            {
                Id = Guid.NewGuid(),
                Category = category,
                BottomChildren = bc.ToList(),
                AmountOfProducts = amount
            });
        }
    
        Console.WriteLine("end");
        _applicationContext.SaveChanges();
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

    [HttpGet]
    public async Task GetImages()
    {
        var counter = 1124;
        using StreamReader stream = new StreamReader("C:\\Users\\Kirill\\source\\repos\\Inzh-Server\\Parcer\\Links.txt");
        var links = stream.ReadToEnd().Split("\n").Skip(counter);

        using IWebDriver driver = new FirefoxDriver();
            foreach (var link in links.AsParallel())
            {
                counter++;
                Console.WriteLine(counter + link);
                await LoadImage(link, driver);
            }
    }

    private async Task LoadImage(string link, IWebDriver driver)
    {
        var mainFolderPath = "C:\\Users\\Kirill\\Desktop\\images";

        // var links = link.Split('/');
        // var lastLink = links[links.Length - 2];

        driver.Navigate().GoToUrl(_url + link);
        var title = driver.FindElement(By.CssSelector("h1")).Text.Replace('>', '_').Replace('<', '_').Replace('*', '_');
        
        var teams = driver.FindElements(By.CssSelector("li.slick-slide.slick-active > a > img"));

        if (teams is null)
            return;

        var counter = 0;
        
        foreach (var item in teams)
        {
            try
            {
                var text = item.GetAttribute("src");

                var client = new HttpClient();
                var bytes = await client.GetByteArrayAsync(new Uri(text));

                Directory.CreateDirectory($"{mainFolderPath}/{title}");

                await using var memoryStream = new MemoryStream(bytes);
                await using var fileStream =
                    new FileStream($"C:\\Users\\Kirill\\Desktop\\images\\{title}\\{++counter}.jpg", FileMode.Create);
                memoryStream.WriteTo(fileStream);
            }
            catch
            {
                Console.WriteLine(title + "error");
            }
        }
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