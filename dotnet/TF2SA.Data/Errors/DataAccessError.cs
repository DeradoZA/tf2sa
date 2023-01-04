using TF2SA.Common.Errors;

namespace TF2SA.Data.Errors;

public class DatabaseError : Error
{
	public DatabaseError(string message)
	{
		Message = message;
	}
}
