using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using ZipkinNet;
using ZipkinNet.Serialization.Json;

namespace BasicSample
{
	public class Program
	{
		public static void Main()
		{
			const string zipkinHost = "http://localhost:9411";
			const string zipkinSpanPostPath = "/api/v1/spans";
			var httpClient = new HttpClient { BaseAddress = new Uri(zipkinHost) };

			var span = Span.CreateRootSpan("Sample3")
				.WithTimestamp(DateTimeOffset.Now)
				.WithDuration(TimeSpan.FromMinutes(3))
				.AddAnnotation(Annotation.CreateServerReceive(DateTimeOffset.Now).WithEndpoint(Endpoint.CreateWithoutPort(IPAddress.Parse("127.0.0.1"), "Sample3")));
			var span2 = Span.CreateChildSpan("Sample4", span)
				.WithTimestamp(DateTimeOffset.Now)
				.AddAnnotation(
					Annotation.CreateServerSend(DateTimeOffset.Now.AddMinutes(1))
						.WithEndpoint(Endpoint.CreateWithoutPort(IPAddress.Parse("127.0.0.2"), "Sample4")));
			var span3 = Span.CreateChildSpan("Sample5", span2)
				.WithTimestamp(DateTimeOffset.Now)
				.AddAnnotation(
					Annotation.CreateServerSend(DateTimeOffset.Now.AddMinutes(2))
						.WithEndpoint(Endpoint.CreateWithoutPort(IPAddress.Parse("127.0.0.2"), "Sample5")));
			var span4 = Span.CreateChildSpan("Sample6", span2)
				.WithTimestamp(DateTimeOffset.Now)
				.AddAnnotation(
					Annotation.CreateServerSend(DateTimeOffset.Now.AddMinutes(2))
						.WithEndpoint(Endpoint.CreateWithoutPort(IPAddress.Parse("127.0.0.2"), "Sample6")));
			var spans = new[] { span, span2, span3, span4 };

			var jsonSpan = JsonZipkinSerializer.Serialize(spans.Select(x => new JsonSpan(x)));
			var httpContent = new StringContent(jsonSpan, Encoding.UTF8, "application/json");
			var result = httpClient.PostAsync(zipkinSpanPostPath, httpContent).Result;
			var response = result.Content.ReadAsStringAsync().Result;
			Console.WriteLine(response);
		}
	}
}
