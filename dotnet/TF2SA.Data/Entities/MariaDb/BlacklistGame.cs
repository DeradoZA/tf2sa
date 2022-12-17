namespace TF2SA.Data.Entities.MariaDb;

public partial class BlacklistGame
{
	public uint GameId { get; set; }
	public string? Reason { get; set; }
}
