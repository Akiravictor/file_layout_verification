using Newtonsoft.Json;
using System.Collections.Generic;

namespace VerifyIntegrations.Models
{
	public class Field
	{
		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string DataType { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int isRequired { get; set; }

		[JsonProperty(Required = Required.Default)]
		public int isPrimaryKey { get; set; }

		[JsonProperty(Required = Required.Default)]
		public string Conditional { get; set; }

		[JsonProperty(Required = Required.Default)]
		public string RegexValidation { get; set; }

		[JsonProperty(Required = Required.Default)]
		public List<string> ValidValues { get; set; }

		[JsonProperty(Required = Required.Default)]
		public List<int> LengthValidation { get; set; }

		[JsonProperty(Required = Required.Default)]
		public List<string> MechanicRequired { get; set; }
	}
}
