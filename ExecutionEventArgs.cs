/**
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
	public sealed class ExecutionEventArgs : EventArgs
	{
		private readonly string _info;

		public ExecutionEventArgs(string info)
		{
			_info = info;
		}

		public string Info
		{
			get { return (_info); }
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void ExecutionEventHandler(object sender, ExecutionEventArgs e);
}
