using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Serialization;
using Xunit;

namespace TF2SA.Tests.Unit.Http;

public class SerializationTests
{
	[Fact]
	public void TestRootGameLog()
	{
		EitherStrict<SerializationError, GameLog> gameLog =
			LogsTFSerializer<GameLog>
			.Deserialize(SerializationStubs.NormalGameLogJsonResponse);
		Assert.True(gameLog.IsRight);

		Assert.Equal(1787U, gameLog.Right.Length);
		Assert.Equal("Blue", gameLog.Right.Players?["[U:1:146905179]"].Team);
		Assert.True(gameLog.Right.Success);
	}

}