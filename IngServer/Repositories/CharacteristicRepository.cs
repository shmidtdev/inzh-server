using IngServer.DataBase;
using IngServer.DataBase.Models;

namespace IngServer.Repositories;

public class CharacteristicRepository(ApplicationContext applicationContext)
{
    public List<Characteristic> GetCharacteristics(Product product)
    {
        return applicationContext.Characteristics.Where(x => x.Products.Contains(product)).ToList();
    }
    
    public List<Characteristic> GetCharacteristics(Func<Characteristic, bool> predicate)
    {
        return applicationContext.Characteristics.Where(predicate).ToList();
    }
}