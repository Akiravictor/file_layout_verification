using Newtonsoft.Json;

namespace VerifyIntegrations.Models
{
	public class Root
	{
		[JsonProperty(Required = Required.Always)]
		public Layout Layout { get; set; }
	}
}
