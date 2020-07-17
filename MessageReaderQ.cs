/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System.Collections.Generic;

namespace sirius.GSM
{
	public sealed class MessageReaderQ
	{
		private readonly Queue<Message> _ReaderQ;

		internal MessageReaderQ()
		{
			_ReaderQ = new Queue<Message>();
		}

		internal void Enqueue(Message message)
		{
			_ReaderQ.Enqueue(message);
		}

		public Message Dequeue()
		{
			return (_ReaderQ.Dequeue());
		}

		public int Count
		{
			get { return (_ReaderQ.Count); }
		}
	}
}
