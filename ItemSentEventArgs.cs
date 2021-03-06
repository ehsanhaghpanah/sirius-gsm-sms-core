﻿/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;

namespace sirius.GSM
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ItemSentEventArgs : EventArgs
	{
		private readonly Message _message;

		public ItemSentEventArgs(Message message)
		{
			_message = message;
		}

		public Message Message
		{
			get { return (_message); }
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void OutgoingItemSentEventHandler(object sender, ItemSentEventArgs e);
}
