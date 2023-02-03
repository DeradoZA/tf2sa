using TF2SA.Common.Errors;

namespace TF2SA.Http.Base.Errors;

public class SteamError : Error
{
	public SteamError(string message)
	{
		Message = message;
	}
}
