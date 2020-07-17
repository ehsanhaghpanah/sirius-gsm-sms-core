/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;
using System.Threading;
using sirius.GSM.IO;
using CS = sirius.GSM.IO.CS;

namespace sirius.GSM
{
	public partial class Modem : IDisposable
	{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private readonly Adaptor _adaptor;
		private readonly MessageReaderQ _ReaderQ;
		private readonly MessageWriterQ _WriterQ;
		private bool _Running;
		private bool _Stopped;
		private readonly Thread _ReaderT;
		private readonly Thread _WriterT;
		private bool _IsReady;
		private string _SMSC;

		private string _Name = string.Empty;
		private string _Description = string.Empty;
		private string _Model = string.Empty;
		private string _PortName = string.Empty;

		public Modem()
		{ 
		}

		public Modem(string portName, int baudRate)
		{
			_adaptor = new Adaptor(portName, baudRate);
			_ReaderT = new Thread(MessageReader);
			_WriterT = new Thread(MessageWriter);
			_ReaderQ = new MessageReaderQ();
			_WriterQ = new MessageWriterQ();
		}

		#region Delegates

		public event EventHandler Attached;
		public event EventHandler Detached;
		public event EventHandler ReaderStarted;
		public event EventHandler ReaderStopped;
		public event EventHandler WriterStarted;
		public event EventHandler WriterStopped;
		public event IncomingItemReadEventHandler ItemRead;
		public event OutgoingItemSentEventHandler ItemSent;
		public event ExecutionEventHandler Executing;

		protected virtual void OnAttached(EventArgs e)
		{
			if (Attached != null)
				Attached(this, e);
		}

		protected virtual void OnDetached(EventArgs e)
		{
			if (Detached != null)
				Detached(this, e);
		}

		protected virtual void OnReaderStarted(EventArgs e)
		{
			if (ReaderStarted != null)
				ReaderStarted(this, e);
		}

		protected virtual void OnReaderStopped(EventArgs e)
		{
			if (ReaderStopped != null)
				ReaderStopped(this, e);
		}

		protected virtual void OnWriterStarted(EventArgs e)
		{
			if (WriterStarted != null)
				WriterStarted(this, e);
		}

		protected virtual void OnWriterStopped(EventArgs e)
		{
			if (WriterStopped != null)
				WriterStopped(this, e);
		}

		protected virtual void OnItemRead(ItemReadEventArgs e)
		{
			if (ItemRead != null)
				ItemRead(this, e);
		}

		protected virtual void OnItemSent(ItemSentEventArgs e)
		{
			if (ItemSent != null)
				ItemSent(this, e);
		}

		protected virtual void OnExecute(ExecutionEventArgs e)
		{
			if (Executing != null)
				Executing(this, e);
		}

		#endregion

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

		#region Propertys

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get { return (_Name); }
			internal set { _Name = value; }
		}

		/// <summary>
		/// Description.
		/// </summary>
		public string Description
		{
			get { return (_Description); }
			internal set { _Description = value; }
		}

		/// <summary>
		/// Model.
		/// </summary>
		public string Model
		{
			get { return (_Model); }
			internal set { _Model = value; }
		}

		/// <summary>
		/// Port Name.
		/// </summary>
		public string PortName
		{
			get { return (_PortName); }
			set { _PortName = value; }
		}

		/// <summary>
		/// Baud Rate.
		/// </summary>
		public int BaudRate { get; set; }

		/// <summary>
		/// Maximum Baud Rate To Serial Port.
		/// </summary>
		public int MaxBaudRateToSerialPort { get; set; }

		/// <summary>
		/// Terminator.
		/// </summary>
		public char[] Terminator { get; set; }

		/// <summary>
		/// Reader Queue.
		/// </summary>
		public MessageReaderQ ReaderQ
		{
			get { return (_ReaderQ); }
		}

		/// <summary>
		/// Writer Queue.
		/// </summary>
		public MessageWriterQ WriterQ
		{
			get { return (_WriterQ); }
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

		#endregion

		#region Functions

		public Message New(string address, byte[] content)
		{
			try
			{
				Message m = new Message(_SMSC, address, content);
				return (m);
			}
			catch (Exception p)
			{
				logger.Error("Exception; {0} and Args; {1}, {2}", p, address, content);
				return (null);
			}
		}

		public Message New(string address, string content)
		{
			try
			{
				Message m = new Message(_SMSC, address, content);
				return (m);
			}
			catch (Exception p)
			{
				logger.Error("Exception; {0} and Args; {1}, {2}", p, address, content);
				return (null);
			}
		}

		public Message New(string address, string content, int port)
		{
			try
			{
				Message m = new Message(_SMSC, address, content, port);
				return (m);
			}
			catch (Exception p)
			{
				logger.Error("Exception; {0} and Args; {1}, {2}, {3}", p, address, content, port);
				return (null);
			}
		}

		public Message New(string address, byte[] content, int port)
		{
			try
			{
				Message m = new Message(_SMSC, address, content, port);
				return (m);
			}
			catch (Exception p)
			{
				logger.Error("Exception; {0} and Args; {1}, {2}, {3}", p, address, content, port);
				return (null);
			}
		}

		public Message NewEx(string address, byte[] content, int port) 
		{
			try
			{
				Message m = Message.NewEx(_SMSC, address, content, port);
				return (m);
			}
			catch (Exception p)
			{
				logger.Error("Exception; {0} and Args; {1}, {2}, {3}", p, address, content, port);
				return (null);
			}
		}

		public bool Attach()
		{
			try
			{
				_adaptor.Attach();
				_IsReady = Check();
				if (!_IsReady)
					throw new ModemException("adaptor is not ready.");

				_Stopped = false;
				_ReaderT.Start();
				_WriterT.Start();

				OnAttached(EventArgs.Empty);
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
				_Stopped = true;
				_ReaderT.Join();
				_WriterT.Join();

				OnDetached(EventArgs.Empty);
				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		#endregion

		/// <summary>
		///
		/// </summary>
		private bool Check()
		{
			try
			{
				OnExecute(new ExecutionEventArgs("Setting Code Suppression..."));
				using (Command command = new CS.CodeSuppressionCommand(_adaptor))
				{
					((CS.CodeSuppressionCommand)command).Type = CS.CodeSuppressionType.TransmitCodes;
					if (!command.Execute())
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting Command Echo..."));
				using (Command command = new CS.CommandEchoCommand(_adaptor))
				{
					((CS.CommandEchoCommand)command).Type = CS.CommandEchoType.NoEcho;
					if (!command.Execute())
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting Response Format..."));
				using (Command command = new CS.ResponseFormatCommand(_adaptor))
				{
					((CS.ResponseFormatCommand)command).Type = CS.ResponseFormatType.Verbose;
					if (!command.Execute())
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting Message Format..."));
				using (Command command = new CS.SMS.MessageFormatCommand(_adaptor))
				{
					((CS.SMS.MessageFormatCommand)command).FormatType = CS.SMS.MessageFormatType.PDU;
					if (((CS.SMS.MessageFormatCommand)command).FormatType != CS.SMS.MessageFormatType.PDU)
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting Report Error..."));
				using (Command command = new CS.ReportMEErrorCommand(_adaptor))
				{
					((CS.ReportMEErrorCommand)command).ReportType = CS.ReportMEErrorType.ReportOK;
					if (((CS.ReportMEErrorCommand)command).ReportType != CS.ReportMEErrorType.ReportOK)
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting Preferred Message Storage..."));
				using (Command command = new CS.SMS.PreferredMessageStorage(_adaptor))
				{
					if (!command.Execute())
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Setting New Message Indications..."));
				using (Command command = new CS.SMS.NewMessageIndications(_adaptor))
				{
					if (!command.Execute())
						throw new ModemException();
				}

				OnExecute(new ExecutionEventArgs("Fetching Service Centre Address..."));
				using (Command command = new CS.SMS.ServiceCentreAddress(_adaptor))
				{
					if (!command.Execute())
						throw new ModemException();
					_SMSC = ((CS.SMS.ServiceCentreAddress)command).Address;
				}

				OnExecute(new ExecutionEventArgs("Clearing Incoming Message Queue..."));
				using (Command command = new CS.SMS.ListMessageCommand(_adaptor, CS.SMS.MessageStateType.All, CS.SMS.MessageFormatType.PDU))
				{
					if (!command.Execute())
						throw new ModemException();

					foreach (CS.SMS.ShortMessage item in ((CS.SMS.ListMessageCommand)command).MessageCollection)
					{
						CS.SMS.DeleteMessageCommand dmc = new CS.SMS.DeleteMessageCommand(_adaptor, item.Index);
						if (!dmc.Execute())
							throw new ModemException("DeleteMessageCommand Execute Failed.");
						if (_Stopped)
							break;
					}
				}

				OnExecute(new ExecutionEventArgs("Initializing Completed."));

				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		#region —— Message Processing ——

		private bool ExecuteWriter(Message message)
		{
			try
			{
				OnWriterStarted(EventArgs.Empty);

				Thread.BeginCriticalRegion();
				CS.SMS.SendMessageCommand command = new CS.SMS.SendMessageCommand(_adaptor, message.Corn);
				bool sent = command.Execute();
				Thread.EndCriticalRegion();

				if (sent)
					OnItemSent(new ItemSentEventArgs(message));

				OnWriterStopped(EventArgs.Empty);
				return (true);
			}
			catch (Exception p)
			{
				logger.Error(p);
				return (false);
			}
		}

		private void MessageWriter()
		{
			while (!_Stopped)
			{
				try
				{
					if (!_Running)
					{
						_Running = true;
						{
							int count = 1;
							while (count < 3)
							{
								if (_WriterQ.Count > 0)
								{
									if (ExecuteWriter(_WriterQ.Peek()))
										_WriterQ.Dequeue();
									else
									{
										Message message = _WriterQ.Dequeue();
										_WriterQ.Enqueue(message);
									}
								}
								count++;
							}
						}
						_Running = false;
					}
					if (!_Stopped)
						Thread.Sleep(2500);
				}
				catch (Exception p)
				{
					logger.Error(p);
				}
			}
		}

		private void ExecuteReader()
		{
			try
			{
				OnReaderStarted(EventArgs.Empty);

				CS.SMS.ListMessageCommand lmc = new CS.SMS.ListMessageCommand(_adaptor, CS.SMS.MessageStateType.All, CS.SMS.MessageFormatType.PDU);

				if (lmc.Execute())
				{
					foreach (CS.SMS.ShortMessage item in lmc.MessageCollection)
					{
						Thread.BeginCriticalRegion();
						CS.SMS.DeleteMessageCommand dmc = new CS.SMS.DeleteMessageCommand(_adaptor, item.Index);
						if (dmc.Execute())
							_ReaderQ.Enqueue(new Message(item));
						Thread.EndCriticalRegion();

						OnItemRead(new ItemReadEventArgs(new Message(item)));

						if (_Stopped)
							break;
					}
				}
				else
					throw new ModemException("ListMessageCommand.Execute() Failed.");

				OnReaderStopped(EventArgs.Empty);
			}
			catch (Exception p)
			{
				logger.Error(p);
			}
		}

		private void MessageReader()
		{
			while (!_Stopped)
			{
				try
				{
					if (!_Running)
					{
						_Running = true;
						{
							ExecuteReader();
						}
						_Running = false;
					}
					if (!_Stopped)
						Thread.Sleep(2500);
				}
				catch (Exception p)
				{
					logger.Error(p);
				}
			}
		}

		#endregion

		//public static IList<Modem> Detect()
		//{
		//     var searcher = new ManagementObjectSearcher(new SelectQuery("CIM_PotsModem"));

		//     foreach (var item in searcher.Get())
		//          foreach (var prop in item.Properties)
		//          { 
		//          }

		//     return (null);
		//}
	}
}
