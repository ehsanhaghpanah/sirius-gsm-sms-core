/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System.Collections.Generic;

namespace sirius.GSM
{
	public sealed class MessageWriterQ
	{
		private readonly Queue<Message> _WriterQ;

		internal MessageWriterQ()
		{
			_WriterQ = new Queue<Message>();
		}

		public void Enqueue(Message message)
		{
			_WriterQ.Enqueue(message);
		}

		internal Message Peek()
		{
			return (_WriterQ.Peek());
		}

		internal Message Dequeue()
		{
			return (_WriterQ.Dequeue());
		}

		internal int Count
		{
			get { return (_WriterQ.Count); }
		}
	}
}
