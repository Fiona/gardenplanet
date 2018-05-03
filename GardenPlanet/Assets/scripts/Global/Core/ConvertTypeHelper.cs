using System;

namespace StompyBlondie
{
	public static class ConvertTypeHelper
	{
		public static T ConvertType<T,U>(U value) where U : IConvertible
		{
			return (T)Convert.ChangeType(value, typeof(T));
		}
	}
}

