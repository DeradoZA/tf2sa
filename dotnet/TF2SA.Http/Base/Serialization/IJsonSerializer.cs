using Monad;
using TF2SA.Http.Base.Errors;

namespace TF2SA.Http.Base.Serialization;

public interface IJsonSerializer
{
	public EitherStrict<SerializationError, TDeserialized> Deserialize<TDeserialized>(string json);
	public EitherStrict<SerializationError, string> Serialize<TSerialized>(TSerialized obj);
}
