#region Limited List (SUCCESS)

/// <summary>
/// Сделай класс LimitedList<T> с максимальной ёмкостью.
/// Если в него добавляют элемент сверх лимита — удаляется самый старый.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LimList<T> : List<T>
{
	private readonly int _maxSize;
	public LimList(int maxSize)
	{
		_maxSize = maxSize;
	}
	public void Add(T item)
	{
		if (Count >= _maxSize)
		{
			RemoveAt(0);
		}
		base.Add(item);
	}
}
#endregion

#region Retry Method (SUCCESS)

/// <summary>
/// Напиши метод Retry, который выполняет переданную функцию и, если та кидает исключение, пробует ещё N раз.
/// Retry(SomeMethod(), 3);
/// </summary>
/// <param name="args"></param>
public static class RetryTask
{
	public static void Retry(Action action, int retryCount)
	{
		for (int i = 0; i < retryCount; i++)
		{
			try
			{
				action();
				break;
			}
			catch
			{
				Thread.Sleep(300);
				Console.WriteLine("Что-то не вышло");
				continue;
			}
		}
	}
}
#endregion

#region Anagrams (SUCCESS)

/// <summary>
/// Напиши метод AreAnagrams(string a, string b), который определяет, являются ли строки анаграммами.
/// Пример: "listen" и "silent" — анаграммы.
/// </summary>
/// <param name="args"></param>
public static class Anagrams
{
	public static bool AreAnagrams(string first, string second)
	{
		if (first.Length != second.Length)
			return false;

		var a = first.ToLower().ToCharArray();
		var b = second.ToLower().ToCharArray();

		Array.Sort(a);
		Array.Sort(b);

		return a.SequenceEqual(b);
	}
}

#endregion

#region Max Positive numbers (SUCCESS)

/// <summary>
/// Напиши метод, который принимает массив чисел и возвращает максимальную сумму непрерывной последовательности положительных чисел.
/// </summary>
/// <param name="args"></param>
public static class MaxPositiveNumbers
{
	public static int GetPositiveSequenceSum(int[] numbers)
	{
		int currentSum = 0;
		int maxSum = 0;

		foreach (var number in numbers)
		{
			if (number >= 0)
			{
				currentSum += number;
				if (currentSum > maxSum)
					maxSum = currentSum;
			}
			else
			{
				currentSum = 0;
			}
		}

		return maxSum;
	}
}
#endregion



public class Program(string[] args)
{
	public static void Main(string[] args)
	{
		Console.WriteLine("LIMITED LIST");
		var limitedList = new LimList<string>(9){"1", "2", "3", "4", "5", "6", "7", "8", "9"};
		Console.WriteLine(string.Join(", ", limitedList));
		limitedList.Add("10");
		Console.WriteLine(string.Join(", ", limitedList));
		limitedList.Add("11");
		Console.WriteLine(string.Join(", ", limitedList));
		limitedList.Add("12");
		Console.WriteLine(string.Join(", ", limitedList));
		Console.WriteLine("====================");
		
		Console.WriteLine("ANAGRAMS");
		Console.WriteLine(Anagrams.AreAnagrams("левочек", "человек"));
		Console.WriteLine("====================");

		Console.WriteLine("RETRY METHOD");
		RetryTask.Retry(RandomMethod, 7);
		Console.WriteLine("====================");

		Console.WriteLine("SUMM MAX POSITIVE NUMBERS");
		Console.WriteLine(MaxPositiveNumbers.GetPositiveSequenceSum(new []{5, 2, 0, 4, 2, -5, -3, 5, 0, 2}));
		Console.WriteLine("====================");

	}


	private static void RandomMethod()
	{
		var random = new Random();
		var isSuccess = random.Next(5) == 0;
		if (isSuccess)
			Console.WriteLine("Удачненько");
		else
			throw new Exception();
	}
	
}

