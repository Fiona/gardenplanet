using System;
using UnityEngine.Events;

namespace StrawberryNova
{
	public class IntEvent: UnityEvent<int>{ } 

	public class Events
	{
		public IntEvent NewHourEvent;

		public Events()
		{
			NewHourEvent = new IntEvent();
		}
	}
}

