using System;
using System.Collections.Generic;

namespace TF2SA.Data.Entities
{
    public partial class BlacklistGame
    {
        public uint GameId { get; set; }
        public string? Reason { get; set; }
    }
}
