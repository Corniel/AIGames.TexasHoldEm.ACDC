#if DEBUG

using System;
using System.Runtime.Serialization;

namespace AIGames.TexasHoldEm.ACDC.Game
{
	/// <summary>The state of the game is invalid.</summary>
	/// <remarks>
	/// This exception should only be thrown in debug mode.
	/// </remarks>
	[Serializable]
	public class InvalidStateException : Exception
	{
		/// <summary>Initializes a new instance of the exception.</summary>
		public InvalidStateException() : base() { }
		/// <summary>Initializes a new instance of the exception.</summary>
		public InvalidStateException(string message) : base(message){ }
		/// <summary>Initializes a new instance of the exception.</summary>
		public InvalidStateException(string message, Exception innerException) : base(message, innerException) { }
		
		/// <summary>Initializes a new instance of the exception.</summary>
		protected InvalidStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
#endif
