using TF2SA.Common.Errors;

namespace TF2SA.Http.Base.Errors;

public class SerializationError : Error
{
	public SerializationError(string message)
	{
		Message = message;
	}
}
