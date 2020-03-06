using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyIntegrations.Models
{
	public class Layout
	{
		[JsonProperty(Required = Required.Always)]
		public string Domain { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Number { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Version { get; set; }

		[JsonProperty(Required = Required.Always)]
		public List<Field> Fields { get; set; }
	}
}
