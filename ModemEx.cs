/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;
using System.Collections.Generic;
using sirius.GSM.IO;
using CS = sirius.GSM.IO.CS;

namespace sirius.GSM
{
	internal sealed class ModemEx : IDisposable
	{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private readonly Adaptor _adaptor;
		private bool _IsReady;
		private string _SMSC;

		public ModemEx(string portName, int baudRate)
		{
			_adaptor = new Adaptor(portName, baudRate);
		}

		~ModemEx()
		{
			logger.Debug(_SMSC);
		}

		#region Interface

		private bool _disposed;

		/// <summary>
		///
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///
		/// </summary>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
				if (disposing)
				{
					try
					{
						_adaptor.Detach();
						((IDisposable)_adaptor).Dispose();
					}
					catch (Exception p)
					{
						logger.Error(p);
					}
				}
			_disposed = true;
		}

		#endregion

		public Message New(string address, byte[] content)
		{
			return (new Message(_SMSC, address, content));
		}

		public Message New(string address, string content)
		{
			return (new Message(_SMSC, address, content));
		}

		public Message New(string address, string content, int gateway)
		{
			return (new Message(_SMSC, address, content, gateway));
		}

		public Message New(string address, byte[] content, int gateway)
		{
			return (new Message(_SMSC, address, content, gateway));
		}

		public bool Attach()
		{
			try
			{
				_adaptor.Attach();
				_IsReady = Check();
				if (!_IsReady)
					throw new ModemException("adaptor is not ready.");

				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		public bool Detach()
		{
			try
			{
				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		/// <summary>
		///
		/// </summary>
		private bool Check()
		{
			try
			{

				using (Command command = new CS.CodeSuppressionCommand(_adaptor))
				{
					((CS.CodeSuppressionCommand)command).Type = CS.CodeSuppressionType.TransmitCodes;
					if (!command.Execute())
						throw new ModemException();
				}

				using (Command command = new CS.CommandEchoCommand(_adaptor))
				{
					((CS.CommandEchoCommand)command).Type = CS.CommandEchoType.NoEcho;
					if (!command.Execute())
						throw new ModemException();
				}

				using (Command command = new CS.ResponseFormatCommand(_adaptor))
				{
					((CS.ResponseFormatCommand)command).Type = CS.ResponseFormatType.Verbose;
					if (!command.Execute())
						throw new ModemException();
				}

				using (Command command = new CS.SMS.MessageFormatCommand(_adaptor))
				{
					((CS.SMS.MessageFormatCommand)command).FormatType = CS.SMS.MessageFormatType.PDU;
					if (((CS.SMS.MessageFormatCommand)command).FormatType != CS.SMS.MessageFormatType.PDU)
						throw new ModemException();
				}

				using (Command command = new CS.ReportMEErrorCommand(_adaptor))
				{
					((CS.ReportMEErrorCommand)command).ReportType = CS.ReportMEErrorType.ReportOK;
					if (((CS.ReportMEErrorCommand)command).ReportType != CS.ReportMEErrorType.ReportOK)
						throw new ModemException();
				}

				using (Command command = new CS.SMS.PreferredMessageStorage(_adaptor))
				{
					if (!command.Execute())
						throw new ModemException();
				}

				using (Command command = new CS.SMS.NewMessageIndications(_adaptor))
				{
					if (!command.Execute())
						throw new ModemException();
				}

				using (Command command = new CS.SMS.ServiceCentreAddress(_adaptor))
				{
					if (!command.Execute())
						throw new Exception();
					_SMSC = ((CS.SMS.ServiceCentreAddress)command).Address;
				}

				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		/// <summary>
		/// If modem passes the check process successfully then it returns true and modem is ready to execute commands.
		/// </summary>
		public bool IsReady
		{
			get { return (_IsReady); }
		}

		/// <summary>
		///
		/// </summary>
		public string SMSC
		{
			get { return (_SMSC); }
		}

		#region —— Message Processing ——

		public bool Send(Message message)
		{
			try
			{
				CS.SMS.SendMessageCommand command = new CS.SMS.SendMessageCommand(_adaptor, message.Corn);
				command.Execute();
				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		public List<Message> Read()
		{
			try
			{
				List<Message> list = new List<Message>();
				CS.SMS.ListMessageCommand lmc = new CS.SMS.ListMessageCommand(_adaptor, CS.SMS.MessageStateType.All, CS.SMS.MessageFormatType.PDU);
				if (lmc.Execute())
				{
					foreach (CS.SMS.ShortMessage item in lmc.MessageCollection)
					{
						CS.SMS.DeleteMessageCommand dmc = new CS.SMS.DeleteMessageCommand(_adaptor, item.Index);
						if (dmc.Execute())
							list.Add(new Message(item));
					}
					return (list);
				}
				
				throw new ModemException("ListMessageCommand.Execute() failed.");
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (null);
			}
		}

		#endregion
	}
}
