using System.Numerics;
using System.Text;
using ConsoleTables;

# region Класс для управления списком задач (to-do list), включая методы добавления, удаления, и получения всех задач.
/// <summary>
/// 3.	Создай класс для управления списком задач (to-do list), включая методы добавления, удаления, и получения всех задач.
/// </summary>
public class Task
{
	public int Id { get; set; } = Random.Shared.Next(100); // Я понимаю что это фигня, но просто для вида.
																	// обычно ID генерируется БД, либо используем Guid
	public string Title { get; set; }
	public string Description { get; set; }
	public Status Status { get; set; }
	public DateTime Created { get; set; } = DateTime.Now;
	public DateTime? Completed { get; set; }
}

public enum Status
{
	created,
	paused,
	inProgress,
	completed,
	failed
}

public class TaskService
{
	// наш виртуальный конектор к бд. Будем считать что мы уже где-то загрузили все данные в List<Task>
	private readonly List<Task> _tasks;

	public TaskService(List<Task> tasks)
	{
		_tasks = tasks;
	}
	
	public Task AddTask(string title, string description, Status status)
	{
		// не проверяем существование такой же задачи, потому что никто не запрещает создавать дубли
		var newTask = new Task() { Title = title, Description = description, Status = status };
		_tasks.Add(newTask);
		return _tasks.LastOrDefault();
	}

	public bool DeleteTask(Task? task)
	{
		if (task != null)
		{
			if (_tasks.Contains(task))
			{
				_tasks.Remove(task);
				return true;
			}
		}
		return false;
	}

	public List<Task> GetAll()
	{
		if (_tasks.Count != 0)
		{
			return _tasks!;
		}
		return new List<Task>();
	}

	public Task EditTask(int id, string title, string description, Status status)
	{
		var currentTask = _tasks.FirstOrDefault(t => t.Id == id);
		if (currentTask != null)
		{
			currentTask.Title = title;
			currentTask.Description = description;
			currentTask.Status = status;
			if (status == Status.completed || status == Status.failed)
			{
				currentTask.Completed = DateTime.Now;
			}
			return currentTask;
		}
		else
		{
			return null;
		}
	}
}
# endregion

# region Простой кэш-класс, который хранит результаты вычислений для уже обработанных значений.

/// <summary>
/// 8.	Реализуй простой кэш-класс, который хранит результаты вычислений для уже обработанных значений.
/// К сожалению сам не справился, хоть и двигался в верном направлении
/// </summary>
public class SimpleCache<TInput, TResult> where TInput : notnull
{
	private class CacheItem
	{
		public TResult Value { get; set; }
		public DateTime ExpirationTime { get; set; }
	}

	private readonly Dictionary<TInput, CacheItem> _cache = new();

	public TResult GetOrAdd(TInput input, Func<TInput, TResult> valueFactory, TimeSpan ttl)
	{
		if (_cache.TryGetValue(input, out var item))
		{
			if (DateTime.Now <= item.ExpirationTime)
			{
				// Кеш ещё живой — возвращаем закэшированный результат
				return item.Value;
			}
		}

		// Кеш отсутствует или протух
		var result = valueFactory(input);

		_cache[input] = new CacheItem
		{
			Value = result,
			ExpirationTime = DateTime.Now.Add(ttl)
		};

		return result;
	}

	public void Clear() => _cache.Clear();

	public void Remove(TInput input) => _cache.Remove(input);
}
# endregion

public class Program
{
	public static void Main(string[] args)
	{
		#region Массив + 10
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		Console.WriteLine(" Добавить к значениям исходного массива чисел 10");
		var table = new ConsoleTable("Исходный массив", "Результат");
		table.Options.EnableCount = false;
		table.AddRow("5, 3, 9", string.Join("; ", NumUpper(new int[] { 5, 3, 9 })));
		table.AddRow("-5.6, 3.885, 9.53", string.Join("; ", NumUpper(new double[] { -5.6, 3.885, 9.53 })));
		Console.WriteLine(table);
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		#endregion
		
		# region Палиндром
		Console.WriteLine("Проверить явялется ли строка палиндромом");
		table = new ConsoleTable("Оригинальная строка", "Результат");
		table.Options.EnableCount = false;
		table.AddRow("НАВЛОБ,БОЛВАН", IsPalindrome("НАВЛОБ,БОЛВАН"));
		table.AddRow("HelloWorld!", IsPalindrome("HelloWorld!"));
		Console.WriteLine(table);
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		# endregion
		
		# region TASKS
		Console.WriteLine("Класс для управления ToDo листом");
		table = new ConsoleTable("ID", "Title", "Description", "Status", "Date Completed");
		table.Options.EnableCount = false;
		TaskService taskService = new TaskService(new List<Task>()
		{
			new() {Id = 1, Title = "Create Task", Description = "Create a new task", Status = Status.created},
			new() {Id = 2, Title = "Edit Task", Description = "Edit a task", Status = Status.created}
		});
		
		var task = taskService.AddTask("Delete Task", "delete a  task", Status.created);
		taskService.AddTask("Get Task", "Get all tasks", Status.created);
		
		
		Console.WriteLine("Список ToDo после добавления значений");
		foreach (var t in taskService.GetAll())
		{
			table.AddRow(t.Id, t.Title, t.Description, t.Status, t.Completed.ToString());
		}
		Console.WriteLine(table);

		table = new ConsoleTable("ID", "Title", "Description", "Status", "Date Completed");
		table.Options.EnableCount = false;
		taskService.EditTask(task.Id, task.Title, task.Description, Status.completed);
		Console.WriteLine("Список ToDo после изменения задачи");

		foreach (var t in taskService.GetAll())
		{
			table.AddRow(t.Id, t.Title, t.Description, t.Status, t.Completed.ToString());
			
		}
		Console.WriteLine(table);
		
		table = new ConsoleTable("ID", "Title", "Description", "Status", "Date Completed");
		table.Options.EnableCount = false;
		var resDelete = taskService.DeleteTask(task);
		Console.WriteLine("Список ToDo после удаления задачи");
		foreach (var t in taskService.GetAll())
		{
			table.AddRow(t.Id, t.Title, t.Description, t.Status, t.Completed.ToString());
			
		}
		Console.WriteLine(table);
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		# endregion
		
		#region Заменить все гласные
		Console.WriteLine("Заменить все гласные буквы символом *");
		table = new ConsoleTable("Исходная строка", "Результат");
		table.Options.EnableCount = false;
		table.AddRow("Привет world", RemoveVowels("Привет world"));
		table.AddRow("ДОБРО ПОЖАЛОВАТЬ на Луну", RemoveVowels("ДОБРО ПОЖАЛОВАТЬ на Луну"));
		Console.WriteLine(table);
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		#endregion
		
		#region Количество дней между датами
		Console.WriteLine("Вычислить количество дней между двумя датами");
		Console.WriteLine(DaysBetween(DateTime.Now, DateTime.Parse("2025-04-14")));
		Console.WriteLine();
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#endregion
		
		#region Преобразовать двумерный массив в список строк
		Console.WriteLine("Преобразовать двумерный массив в список строк");
		Console.WriteLine(string.Join("\n", ConvertMatrixToStringList(new int[,]{{1, 2, 3, 4}, {2, 3, 44, 5}, {5, 6, 2, 4}})));
		Console.WriteLine();
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));

		#endregion
		
		#region КЭШ
		Console.WriteLine("Реализовать упрощенный вариант кэша");
		var cache = new SimpleCache<int, string>();

		string SlowCalculation(int number)
		{
			Console.WriteLine($"[!] Обработка {number}...");
			Thread.Sleep(1000); // эмуляция долгой работы
			return $"Результат: {number * 2}";
		}

		var result1 = cache.GetOrAdd(5, SlowCalculation, TimeSpan.FromSeconds(5)); // обработка
		var result2 = cache.GetOrAdd(5, SlowCalculation, TimeSpan.FromSeconds(5)); // из кеша

		Console.WriteLine(result1); // Результат: 10
		Console.WriteLine(result2); // Результат: 10 (быстро)
		Console.WriteLine();
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		#endregion
		
		#region Индекс самого большого числа
		Console.WriteLine("Получить индекс самого большого числа в массиве");
		table = new ConsoleTable("Исходный массив", "Результат");
		table.Options.EnableCount = false;
		table.AddRow("1.0, 14.5, 2.55, 9.94", IndexMaxValue(new double[] { 1.0, 14.5, 2.55, 9.94 }));
		Console.WriteLine(table);
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		#endregion
		
		#region Перевернуть строку и вывести символы
		Console.WriteLine("Перевернуть строку и вывести список символов");
		StringReverse("Hello World!");
		Console.WriteLine();
		Console.WriteLine(String.Concat(Enumerable.Repeat("=", 70)));
		#endregion
	}
	
	# region Метод, который принимает массив чисел и возвращает новый массив, где каждое число увеличено на 10.
	/// <summary>
	/// 	1.	Реализуй метод, который принимает массив чисел и возвращает новый массив, где каждое число увеличено на 10.
	/// </summary>
	private static T[] NumUpper<T>(T[] items) where T : INumber<T>
	{
		var result = new T[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			result[i] = items[i] + T.CreateChecked(10); 
		}
		return result;
	}
	# endregion
	
	# region Функция, которая проверяет, является ли строка палиндромом (учитывая регистр и пробелы).

	/// <summary>
	/// 2.	Напиши функцию, которая проверяет, является ли строка палиндромом (учитывая регистр и пробелы).
	/// </summary>
	private static bool IsPalindrome(string str)
	{
		var reversed = str.ToCharArray().Reverse();
		var origin = str.ToCharArray();
		return (reversed.SequenceEqual(origin));
	}
	# endregion
	
	# region Метод, который принимает список строк и возвращает их в отсортированном порядке.

	/// <summary>
	/// 4.	Напиши метод, который принимает список строк и возвращает их в отсортированном порядке.
	/// </summary>
	private List<string> SortedList(List<string> list)
	{
		return list.OrderBy(x => x).ToList();
	}
	# endregion
	
	# region Метод, который принимает строку и возвращает новую строку, где все гласные заменены на символ *.

	/// <summary>
	/// 5.	Реализуй метод, который принимает строку и возвращает новую строку, где все гласные заменены на символ *.
	/// </summary>
	private static string RemoveVowels(string str)
	{
		// HashSet быстрее и более подходит, так как в нем хранятся уникальные значения
		var vowelsList = new HashSet<char>{'a', 'e', 'i', 'o', 'u', 'а', 'е', 'ё', 'и', 'о','у','э','ю','я','ы'};
		var originChars = str.ToLower().ToCharArray();
		for (var i = 0; i < originChars.Length; i++)
		{
			if (vowelsList.Contains(originChars[i]))
			{
				originChars[i] = '*';
			}
		}
		return new string(originChars);
	}
	# endregion
	
	# region Функция, которая принимает две даты и возвращает количество дней между ними.

	/// <summary>
	/// 6.	Создай функцию, которая принимает две даты и возвращает количество дней между ними.
	/// </summary>
	private static int DaysBetween(DateTime start, DateTime end)
	{
		return (end.Date - start.Date).Days;
	}
	# endregion
	
	# region Метод, который преобразует двумерный массив чисел в список строк, где каждая строка — это числа из строки массива, разделённые запятыми.

	/// <summary>
	/// 7.	Напиши метод, который преобразует двумерный массив чисел в список строк, где каждая строка — это числа из строки массива, разделённые запятыми.
	/// </summary>
	private static List<string> ConvertMatrixToStringList(int[,] array)
	{
		var result = new List<string>();
		var tempSB = new StringBuilder();
		int rows = array.GetUpperBound(0) + 1;
		int columns = array.Length / rows; 
		for(var i = 0; i < rows; i++)
		{
			for (var j = 0; j < columns; j++)
			{
				tempSB.Append(array[i, j]);
				tempSB.Append(", ");
			}
			result.Add(tempSB.ToString());
			tempSB.Clear();
		}
		return result;
	}
	# endregion
	
	# region Метод, который принимает массив чисел и возвращает индекс первого элемента, который больше всех остальных.

	/// <summary> 
	/// 9.	Напиши метод, который принимает массив чисел и возвращает индекс первого элемента, который больше всех остальных.
	/// </summary>
	private static int IndexMaxValue<T>(T[] numbers) where T : INumber<T>
	{
		if (numbers.Length == 0)
			return -1;
		return Array.IndexOf(numbers, numbers.Max());
	}
	# endregion
	
	# region Метод, которая принимает на вход строку и выводит на экран её символы в обратном порядке.

	/// <summary>
	/// 10.	Создай метод, которая принимает на вход строку и выводит на экран её символы в обратном порядке.
	/// </summary>
	private static void StringReverse(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentNullException(nameof(text));
		}
		var resCharsFirst = text.ToCharArray().Reverse();
		Console.Write("С использованием Reverse: ");
		Console.WriteLine(string.Join(" ", resCharsFirst));
		
		//Если нельзя использовать Reverse
		Console.Write("Без использования Reverse: ");
		var chars = text.ToCharArray();
		var resultChar = new char[chars.Length];
		for (var i = chars.Count() - 1; i >= 0; i--)
		{
			resultChar[chars.Length - i - 1] = chars[i];
		}
		Console.WriteLine(string.Join(" ", resultChar));
	}
	# endregion
}