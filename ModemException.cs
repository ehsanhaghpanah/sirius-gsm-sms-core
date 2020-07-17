/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;

namespace sirius.GSM
{
	internal sealed class ModemException : Exception
	{
		public ModemException()
		{
		}

		public ModemException(string message)
			: base(message)
		{
		}

		public ModemException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
