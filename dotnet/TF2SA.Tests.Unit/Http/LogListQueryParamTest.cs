using TF2SA.Common.Models.LogsTF.LogListModel;
using Xunit;

namespace TF2SA.Tests.Unit.Http;

public class LogListQueryParamTest
{
	[Fact]
	public void GivenNullFilter_ReturnsEmpty()
	{
		LogListQueryParams filter = new();
		Assert.Empty(LogListQueryParams.GetQueryString(filter));
	}

	[Fact]
	public void TestCorrectQueryString()
	{
		LogListQueryParams filter =
			new()
			{
				Title = "Bruh",
				Map = "cp_bruh",
				Uploader = 123U,
				Players = new ulong[] { 123U, 321U },
				Limit = 10,
				Offset = 1000
			};

		Assert.Equal(
			"title=Bruh&uploader=123&limit=10&offset=1000",
			LogListQueryParams.GetQueryString(filter)
		);
	}
}
