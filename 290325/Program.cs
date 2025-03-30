using System.Net;
using System.Numerics;
using ConsoleTables;

public class Person
{
	public string Name { get; set; }
	public int Age { get; set; }

	public Person(string name, int age)
	{
		Name = name;
		Age = age;
	}
}

public class Matrix<T> where T : INumber<T>
{
	private readonly T[,] _data;

	public int Rows => _data.GetLength(0);
	public int Columns => _data.GetLength(1);

	public Matrix(T[,] data)
	{
		_data = data;
	}

	public Matrix<T> Add(Matrix<T> other)
	{
		if (Rows != other.Rows || Columns != other.Columns)
			throw new ArgumentException("Матрицы должны быть одного размера");

		var result = new T[Rows, Columns];

		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
				result[i, j] = _data[i, j] + other._data[i, j];
		}

		return new Matrix<T>(result);
	}

	public override string ToString()
	{
		var sb = new System.Text.StringBuilder();

		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
				sb.Append($"{_data[i, j],5}");
			sb.AppendLine();
		}

		return sb.ToString();
	}
}

public struct Range
{
	public double Min { get; set; }
	public double Max { get; set; }

	public Range(double min, double max)
	{
		if (min > max)
		{
			Max = min;
			Min = max;
		}
		else
		{
			Min = min;
			Max = max;
		}
	}

	public Range Intersect(Range range)
	{
		if (range.Min > Max || range.Max < Min)
		{
			return new Range(0, 0);
		}
		var result = new Range();
		result.Min = Math.Max(range.Min, Min);
		result.Max = Math.Min(range.Max, Max);
		return result;
	}

	public override string ToString()
	{
		return $"({Min} - {Max})";
	}
}

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		
		#region список имён людей старше 18 лет, отсортированный по возрасту
		Console.WriteLine("Persons старше 18 лет с сортировкой по возрасту");
		var table = new ConsoleTable("Исходный массив", "Результат");
		table.Options.EnableCount = false;
		var persons = new List<Person>()
		{
			new("First", 10),
			new("Second", 31),
			new("Third", 30),
			new("Fourth", 11),
		};
		table.AddRow(string.Join("; ", persons.Select(p => $"{p.Name} {p.Age}")),
			string.Join("; ", GetPersonOver18(persons)));
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		
		#region Словарь с количеством вхождений каждого символа
		Console.WriteLine("Количество символов");
		table = new ConsoleTable("Исходная строка", "Результат");
		table.Options.EnableCount = false;
		table.AddRow("aabbccc", string.Join("; ", CountChars("aabbccc").Select(c => $"{c}")));
		table.AddRow("ASsaSins", string.Join("; ", CountChars("ASsaSins").Select(c => $"{c}")));
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#region Класс Matrix с операцией сложения двух матриц и выводом результата.
		Console.WriteLine("Результат сложения 2 матриц");
		var firstMatrix = new Matrix<double>(new double[,]
		{
			{ 1.3, 2.5, 3.5 }, 
			{ 4.2, 5.3, 6.6 }, 
			{ 7, 8, 9 }
		});
		var secondMatrix = new Matrix<double>(new double[,]
		{
			{ 1, 2, 3 }, 
			{ 4, 5, 6 }, 
			{ 7, 8, 9 }
		});
		var result = firstMatrix.Add(secondMatrix);
		Console.WriteLine(result);
		
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#region Словарь с количеством вхождений каждого слов
		Console.WriteLine("Количество слов одинаковой длины");
		table = new ConsoleTable("Исходный список", "Результат");
		table.Options.EnableCount = false;
		var listStrings = new List<string>(){"Word", "World", "Hello", "Goodbye", "Goodbye", "Wild", "Hi"};
		table.AddRow(string.Join("; ", listStrings
			.Select(s => $"{s}")), 
			string.Join("; ", GroupStringLength(listStrings)
				.Select(s => $"[{s.Key}, {string.Join(", ", s.Value)}]")));
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#region Проверка является ли строка ip адрес
		Console.WriteLine("Является ли строка корректным IP адресом");
		table = new ConsoleTable("Исходная строка", "Результат");
		table.Options.EnableCount = false;
		var correctIp = "254.1.0.12";
		var incorrectIp = "256.004.2566.9903";
		table.AddRow(correctIp, isCorrectIP(correctIp));
		table.AddRow(incorrectIp, isCorrectIP(incorrectIp));
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#region Консольная утилита, которая читает строки в из файла и заменяет все цифры на *
		while (true)
		{
			Console.WriteLine();
			Console.WriteLine("Введи полный путь до файла");
			Console.WriteLine("Если хочешь выйти - введи --exit");
			var filePath = Console.ReadLine();
			if (filePath == "--exit")
			{
				break;
			}
			try
			{
				if (File.Exists(filePath))
				{
					byte[] content = File.ReadAllBytes(filePath);
					for (int i = 1; i < 512 && i < content.Length; i++) {
						if (content[i] == 0x00 && content[i-1] == 0x00)
						{
							throw new Exception("Binary file");
						}
					}
					var fileContent = File.ReadAllText(filePath);
					Console.WriteLine(RemoveNumbers(fileContent));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.Contains("Binary") ? "Файл бинарный, чтение невозможно" : "Ошибка при чтении файла.");
			}
		}
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		
		#region Структура Range с методом Intersect возвращающим пересечение двух диапазонов
		Console.WriteLine("Результат");
		table = new ConsoleTable("Исходные данные", "Результат");
		table.Options.EnableCount = false;
		
		var firstRange = new Range(1.4, 12);
		var secondRange = new Range(20, 2);
		var resultRange = firstRange.Intersect(secondRange);
		
		table.AddRow($"1. {firstRange.ToString()}; 2. {secondRange.ToString()}", 
			$"{resultRange.ToString()}");
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		
		#region Метод удаляющий дубликаты из списка чисел, но сохраняющий порядок
		Console.WriteLine("Удалить дубли в списке чисел");
		table = new ConsoleTable("Исходные данные", "Результат");
		table.Options.EnableCount = false;
		
		var firstNumbers = new List<double>() { 1, 2.3, 55, 55, 1, 35, 5.4 };
		var secondNumbers = new List<int>() { 2, 4, 66, 1, 4, 55, 66 };
		var firstResult = DeleteDoubles(firstNumbers);
		var secondResult = DeleteDoubles(secondNumbers);
		
		table.AddRow(string.Join("; ", firstNumbers), string.Join("; ", firstResult));
		table.AddRow(string.Join("; ", secondNumbers), string.Join("; ", secondResult));
		Console.WriteLine(table);
		#endregion
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

	}

	private static List<string> GetPersonOver18(List<Person> peoples)
	{
		var result = new List<string>();
		result.AddRange(peoples
			.Where(age => age.Age >= 18)
			.OrderBy(age => age.Age)
			.Select(p => p.Name)
			.ToList());
		return result;
	}

	private static Dictionary<char, int> CountChars(string data)
	{
		var result = new Dictionary<char, int>();
		var chars = data.ToLower().ToCharArray();
		for (int i = 0; i < chars.Length; i++)
		{
			if (result.ContainsKey(chars[i]))
			{
				result[chars[i]]++;
			}
			else
			{
				result[chars[i]] = 1;
			}
		}
		return result;
	}
	
	private static Dictionary<int, List<string>> GroupStringLength(List<string> data)
	{
		var result = new Dictionary<int, List<string>>();
		foreach (var str in data)
		{
			if (result.ContainsKey(str.Length))
			{
				result[str.Length].Add(str);
			}
			else
			{
				result[str.Length] = new List<string> { str };
			}
		}
		return result;
	}

	private static bool isCorrectIP(string ip)
	{
		return IPAddress.TryParse(ip, out _);
	}
	
	private static string RemoveNumbers(string str)
	{
		var numbersList = new HashSet<char>{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
		var originChars = str.ToCharArray();
		for (var i = 0; i < originChars.Length; i++)
		{
			if (numbersList.Contains(originChars[i]))
			{
				originChars[i] = '*';
			}
		}
		return new string(originChars);
	}

	private static List<T> DeleteDoubles<T>(List<T> numbers) where T : INumber<T>
	{
		var resultList = new List<T>();
		foreach (var number in numbers)
		{
			if (!resultList.Contains(number))
				resultList.Add(number);
			else
				continue;
		}
		return resultList;
	}
}