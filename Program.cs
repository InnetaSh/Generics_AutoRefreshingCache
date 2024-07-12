

//Создайте универсальный класс AutoRefreshingCache<TKey, TValue>, который будет представлять собой кэш с автоматическим обновлением значений. Реализуйте следующие функции:

//Добавление элемента: Метод Add(TKey key, TValue value, TimeSpan expiration, Func<TValue> refreshFunc) для добавления элемента в кэш. 
//    Здесь expiration указывает на время, через которое значение должно быть обновлено, а refreshFunc — функция для обновления значения.
//Получение элемента: Метод Get(TKey key) для получения элемента из кэша.
//Удаление элемента: Метод Remove(TKey key) для удаления элемента из кэша.
//Автоматическое обновление: Реализуйте механизм, который автоматически обновляет значение в кэше по истечении указанного времени.
//Событие обновления: Событие OnItemRefreshed, которое будет вызываться каждый раз, когда элемент обновляется.
//Требования:

//Класс AutoRefreshingCache<TKey, TValue> должен использовать дженерики для обеспечения универсальности.
//Реализуйте перечисленные методы, чтобы они корректно работали с любым типом данных для ключей и значений.
//Напишите несколько тестов, демонстрирующих использование вашего класса с различными типами данных (например, int, string, пользовательские классы).


List<Product> products = new List<Product>
        {
            new Product (1, "компьютер", 57000){ DeltaPrice = 100},
            new Product (2,  "клавиатура",  1500){ DeltaPrice = 200},
            new Product (3,  "динамики",  2400){ DeltaPrice = 300},
            new Product (4,  "веб камера",  200){ DeltaPrice = 400},
            new Product (5,  "ноутбук",  62000){ DeltaPrice = 500},
        };

AutoRefreshingCache<string, int> stringCache = new AutoRefreshingCache<string, int>();
int num = 100000;
foreach (var p in products)
{ 
    stringCache.Add(p.Name, p.Price, TimeSpan.FromSeconds(5), () => p.RefreshPrice()) ;
}
Console.ReadKey();




public class AutoRefreshingCache<TKey, TValue>
{
    Dictionary<TKey, TValue> dict;

    public AutoRefreshingCache()
    {
        dict = new Dictionary<TKey, TValue>();
        ItemRefreshed += Print;
    }

    public void Add(TKey key, TValue value, TimeSpan expiration, Func<TValue> refreshFunc)
    {
        dict[key] = value;
        var timer = new Timer((e) =>
        {
            dict[key] = refreshFunc();
            OnItemRefreshed(key, value);
        }, null, TimeSpan.Zero, expiration);
    }


    public TValue Get(TKey key)
    {
        if (!dict.Keys.Contains(key))
            throw new Exception("key is not found");
        return dict[key];
    }

    public void Remove(TKey key)
    {
        if (!dict.Keys.Contains(key))
            throw new Exception("key is not found");
        dict.Remove(key);
    }

    public delegate void UpdateHandler(TKey key, TValue value);
    public event UpdateHandler ItemRefreshed;

    public void OnItemRefreshed(TKey key, TValue value)
    {
        ItemRefreshed?.Invoke(key, value);
    }

    public void Print(TKey key, TValue value)
        {
        Console.WriteLine($"обновлен объект {key} со значением {dict[key]}");
        Console.WriteLine("-------------------------------------");
    }
}


class Product
{
    public int Id;
    public string Name;
    public int Price;
    public int DeltaPrice;
    public Product(int productId, string name, int price)
    {
        Id = productId;
        Name = name;
        Price = price;
    }

    public int RefreshPrice()
    {
        Price += DeltaPrice;
        return Price;
    }

}