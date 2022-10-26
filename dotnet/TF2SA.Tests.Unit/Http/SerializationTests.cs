using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Serialization;
using Xunit;

namespace TF2SA.Tests.Unit.Http;

public class SerializationTests
{
    [Fact]
    public void TestRootGameLog()
    {
        GameLog gameLog = LogsTFSerializer.DeserializeGameLog(SerializationStubs.NormalGameLogJsonResponse);
        uint expected = 1787;
        Assert.Equal(gameLog?.Length, expected);
        Assert.Equal(gameLog?.Players?["[U:1:146905179]"].Team, "Red");
    }
}