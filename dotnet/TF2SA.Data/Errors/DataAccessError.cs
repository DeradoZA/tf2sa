using TF2SA.Common.Errors;

namespace TF2SA.Data.Errors;

public class DataAccessError : Error
{
	public DataAccessError(string message)
	{
		Message = message;
	}
}
