using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bench
{
	public class Fields
	{
		public decimal Field1 { get; set; }
		public decimal Field2 { get; set; }
	}

	public class FieldsArray
	{
		public decimal[] Array1 { get; set; }
		public decimal[] Array2 { get; set; }
	}

	public class ResourceReader
	{
		public ResourceReader()
		{
		}

		public FieldsArray ReadResource(string resourceName)
		{
			var list = new List<Fields>();

			var assembly = Assembly.GetExecutingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (TextReader reader = new StreamReader(stream))
			{
				string line = reader.ReadLine();
				while (line != null)
				{
					var split = line.Split(';');

					if (split.Length == 2)
					{
						list.Add(new Fields
						{
							Field1 = Convert.ToDecimal(split[0]),
							Field2 = Convert.ToDecimal(split[1])
						});
					}
					else if (split.Length == 1)
					{
						list.Add(new Fields
						{
							Field1 = Convert.ToDecimal(split[0]),
							Field2 = Convert.ToDecimal(1M)
						});
					}

					line = reader.ReadLine();
				}

				var array = new FieldsArray
				{
					Array1 = list.Select(y => y.Field1).ToArray(),
					Array2 = list.Select(y => y.Field2).ToArray()
				};

				return array;
			}
		}
	}
}
