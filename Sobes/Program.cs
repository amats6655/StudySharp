/// <summary>
/// csv | ID | UserName | Comment | Date |
/// Вывести в консоль в удобном виде для каждого Username список задач в которых оставлен коммент в разрезе дат
/// </summary>

public class Issue
{
	public int Id { get; set; }
	public string UserName { get; set; }
	public string Comment { get; set; }
	public DateOnly Date { get; set; }
}

public static class Program
{
	public static void Main(string[] args)
	{
		var rawData = File.ReadAllLines("./rawData.csv");
		rawData = rawData.Skip(1).ToArray();
		var issues = new List<Issue>();
		foreach (var line in rawData)
		{
			var values = line.Split(';');
			var tempIssue = new Issue()
			{
				Id = int.Parse(values[0]),
				UserName = values[1],
				Comment = values[2],
				Date = DateOnly.Parse(values[3])
			};
			issues.Add(tempIssue);
		}

		var resDictionary = new Dictionary<string, Dictionary<DateOnly, List<Issue>>>();
		foreach (var issue in issues)
		{
			if (resDictionary.ContainsKey(issue.UserName))
			{
				if (resDictionary[issue.UserName].ContainsKey(issue.Date))
				{
					resDictionary[issue.UserName][issue.Date].Add(issue);
				}
				else
				{
					resDictionary[issue.UserName].Add(issue.Date, new List<Issue> { issue });
				}
			}
			else
			{
				resDictionary.Add(issue.UserName, new Dictionary<DateOnly, List<Issue>>());
				resDictionary[issue.UserName].Add(issue.Date, new List<Issue> { issue });
			}
			
		}
		var resDictionaryLinq = issues
			.GroupBy(u => u.UserName)
			.ToDictionary(
				g => g.Key, 
				g => g
					.GroupBy(u => u.Date)
					.ToDictionary(
						g => g.Key, 
						g => g.ToList()
						)
				);
		
		Print(resDictionary);
		Print(resDictionaryLinq);
	}

	private static void Print(Dictionary<string, Dictionary<DateOnly, List<Issue>>> resDictionary)
	{
		foreach (var kvp in resDictionary)
		{
			Console.WriteLine($"{kvp.Key}");
			foreach (var val in kvp.Value)
			{
				Console.WriteLine($"\t{val.Key}");
				foreach (var item in val.Value)
				{
					Console.WriteLine($"\t\t{item.Id}, {item.Comment}");
				}
			}
		}
	}
}


