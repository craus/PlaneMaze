using System;
public class RejectableException : Exception
{
	public RejectableException(string message)
		: base(message) {

	}
}
