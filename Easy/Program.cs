using System.Text;

namespace ConsoleApp2;

#region DataProcessor
public class DataProcessor<T>
{
	private List<T> _items;

	public DataProcessor(IEnumerable<T> items)
	{
		_items = items.ToList();
	}

	public List<TResult> Process<TResult>(Func<T, TResult> handler)
	{
		return _items.Select(handler).ToList();
	}

	public TResult Aggregate<TResult>(Func<IEnumerable<T>, TResult> aggregator)
	{
		return aggregator(_items);
	}
}
#endregion

#region LoggedList
/// <summary>
/// Написать класс которые будет вести себя как List, но будет логгировать добавление элементов
/// </summary>
/// <typeparam name="T"></typeparam>
public class LoggerList<T> : List<T>
{
	public void Add(T item)
	{
		Console.WriteLine($"Add new object {item}");
		base.Add(item);
	}
}
#endregion

#region FilteredList
/// <summary>
/// Реализуй метод, который принимает список объектов и функцию-фильтр, и возвращает только те, что прошли проверку.
/// </summary>
public class FilteredList
{
	public static List<T> Filter<T>(List<T> source, Func<T, bool> predicate)
	{
		return source.Where(predicate).ToList();
	}
}
#endregion

#region Calculator
/// <summary>
/// Реализуй класс Calculator, который поддерживает операции сложения, вычитания, умножения и деления.
/// Операции должны задаваться извне через делегаты.
/// </summary>
public class Calculator
{
	public TResult Execute<TResult>(double a, double b, Func<double, double, TResult> handler)
	{
		return handler(a, b);
	}
}
#endregion

#region SafeDictionary
/// <summary>
/// Создай класс SafeDictionary<TKey, TValue>, который не выбрасывает исключение при попытке доступа к отсутствующему ключу,
/// а возвращает значение по умолчанию.
/// </summary>
public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
	public TValue this[TKey key]
	{
		get
		{
			if(TryGetValue(key, out TValue value))
				return value;
			return default;
		}
		set
		{
			base[key] = value;
		}
	}
}
#endregion

#region FormattedString
/// <summary>
/// Напиши расширение ToFormattedString<T>, которое преобразует список объектов в строку,
/// используя указанный формат (например, "Item: {0}").
/// </summary>
public static class Formatted
{
	public static string ToFormattedString<T>(this IEnumerable<T> source, string prefix = "item: ", string separator = "; ")
	{
		var sb = new StringBuilder();
		foreach (var element in source)
		{
			sb.Append(prefix + element + separator);
		}
		return sb.ToString();
	}
}
#endregion

#region SummAll
/// <summary>
/// Реализуй метод SumAll, который принимает переменное количество чисел и возвращает их сумму.
/// </summary>
public static class Summ
{
	public static double SumAll(params double[] values)
	{
		var result = 0.0;
		foreach (var value in values)
		{
			result += value;
		}
		return result;
	}
}
#endregion

#region Shape
/// <summary>
/// Реализуй интерфейс IShape с методом GetArea(). Сделай классы Circle, Square, Rectangle, реализующие этот интерфейс.
/// </summary>
public interface IShape
{
	double GetArea();
}

public class Rectangle : IShape
{
	private readonly double _a;
	private readonly double _b;

	public Rectangle(double a, double b)
	{
		_a = a;
		_b = b;
	}
	
	public double GetArea()
	{
		return _a * _b;
	}
}

public class Circle : IShape
{
	private readonly double _radius;

	public Circle(double radius)
	{
		_radius = radius;
	}

	public double GetArea()
	{
		return Math.PI * _radius * _radius;
	}
}
#endregion

#region GroupByFirstLetter

/// <summary>
/// Сделай метод GroupByFirstLetter, который группирует строки из списка по первой букве (без учёта регистра)
/// и возвращает Dictionary<char, List<string>>.
/// </summary>
public static class FirstLetter
{
	public static Dictionary<char, List<string>> GroupByFirstLetter(List<string> letters)
	{
		Dictionary<char, List<string>> result = new();
		foreach (var letter in letters)
		{
			var firstChar = letter.ToLower()[0];
			if (result.ContainsKey(firstChar))
			{
				result[firstChar].Add(letter);
			}
			else
			{
				result.Add(firstChar, new List<string> { letter });
			}
		}
		
		return result;
	}
}
#endregion


public class Program
{
	public static void Main(string[] args)
	{
		var processor = new DataProcessor<int>(new List<int> { 1, 2, 3, 4, 5 });

		var multy = processor.Process(x => x * 2);
		foreach (var res in multy)
			Console.WriteLine(res);

		var min = processor.Aggregate(item => item.Min());
		Console.WriteLine(min);

		var testList = new LoggerList<string>();
		testList.Add("first");
		testList.Add("second");
		testList.Add("third");

		foreach (var list in testList)
		{
			Console.WriteLine(list);
		}

		var filtered = FilteredList.Filter(new List<string> { "Sanya", "Dasha", "Vanya", "Vanya" },
			name => name == "Vanya");
		foreach (var item in filtered)
		{
			Console.WriteLine(item);
		}

		var calc = new Calculator();
		var resAdd = calc.Execute(1, 10, (a, b) => a + b);
		var resultMultiply = calc.Execute(1, 10, (a, b) => a * b);
		var resultDivide = calc.Execute(1, 10, (a, b) => a / b);
		Console.WriteLine(resAdd);
		Console.WriteLine(resultMultiply);
		Console.WriteLine(resultDivide);

		var sd = new SafeDictionary<int, string>();
		sd.Add(1, "one");
		sd.Add(2, "two");
		Console.WriteLine(sd[3]);

		Console.WriteLine(new List<string> { "one", "two", "three" }.ToFormattedString());
		Console.WriteLine(Summ.SumAll(2, 1, 4, 55.9));

		Console.WriteLine(new Rectangle(5, 4.5).GetArea());
		Console.WriteLine(new Circle(5).GetArea());
		
		var letters = new List<string> {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};
		var grouppedLetters = FirstLetter.GroupByFirstLetter(letters);
		foreach (var group in grouppedLetters)
		{
			Console.Write(group.Key + " ");
			Console.WriteLine(group.Value.ToFormattedString());
		}
	}
}