namespace StompyBlondie
{
	// I found this on a stackoverflow post
	// frankly i think it's a pretty hilarious
	// solution to this problem
	public class DeepClone
	{
	    public static T Clone<T>(T source)
	    {
		    var serialized = GardenPlanet.JsonHandler.Serialize(source);
		    return GardenPlanet.JsonHandler.Deserialize<T>(serialized);
	    }
	}
}