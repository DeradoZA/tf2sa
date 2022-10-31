using TF2SA.Http.LogsTF.Models;
using Xunit;

namespace TF2SA.Tests.Unit.Http;

public class LogListQueryParamTest
{
	[Fact]
	public void TestCorrectQueryString()
	{
		LogListQueryParams filter = new()
		{
			Title = "Bruh",
			Uploader = 123U
		};

		Assert.Equal("title=Bruh&uploader=123", LogListQueryParams.GetQueryString(filter));
	}
}
