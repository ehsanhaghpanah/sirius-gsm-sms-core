/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;
using System.Text;
using SMSP = sirius.GSM.Protocols.SMS;
using GSMS = sirius.GSM.IO.CS.SMS;

namespace sirius.GSM
{
	/// <summary>
	/// 
	/// </summary>
	public sealed partial class Message
	{
		private SMSP.SubmitMessage _corn;
		private byte[] _Address;
		private byte[] _Content;
		private int _Port;
		private Protocols.GsmDataCoding _Coding;

		internal Message() 
		{
		}

		public Message(string smsc, string address, byte[] content)
		{
			_corn = new SMSP.SubmitMessage(Protocols.GsmDataCoding.GsmDefault);

			_corn.Data = content;

			if (_corn.Data.Length > 160)
				throw new NotSupportedException("Long Message Not Supported.");

			_corn.CenterAddress = new SMSP.Address(
				SMSP.DataRepresentation.Octet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				smsc);
			_corn.DestinationAddress = new SMSP.Address(
				SMSP.DataRepresentation.SemiOctet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				address);

			_Address = Encoding.ASCII.GetBytes(address);
			_Content = content;
			_Port = 0;
			_Coding = Protocols.GsmDataCoding.GsmDefault;
		}

		public Message(string smsc, string address, string content)
		{
			_corn = new SMSP.SubmitMessage(Protocols.GsmDataCoding.GsmDefault);

			_corn.Data = Encoding.ASCII.GetBytes(content);
			if (_corn.Data.Length > 160)
				throw new NotSupportedException("Long Message Not Supported.");

			_corn.CenterAddress = new SMSP.Address(
				SMSP.DataRepresentation.Octet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				smsc);
			_corn.DestinationAddress = new SMSP.Address(
				SMSP.DataRepresentation.SemiOctet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				address);

			_Address = Encoding.ASCII.GetBytes(address);
			_Content = Encoding.ASCII.GetBytes(content);
			_Port = 0;
			_Coding = Protocols.GsmDataCoding.GsmDefault;
		}

		public Message(string smsc, string address, string content, int port)
		{
			_corn = new SMSP.SubmitMessage(Protocols.GsmDataCoding.Ascii);

			_corn.Data = Encoding.ASCII.GetBytes(content);
			if (_corn.Data.Length > 134)
				throw new NotSupportedException("Long Message Not Supported.");

			_corn.CenterAddress = new SMSP.Address(
				SMSP.DataRepresentation.Octet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				smsc);
			_corn.DestinationAddress = new SMSP.Address(
				SMSP.DataRepresentation.SemiOctet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				address);

			_Address = Encoding.ASCII.GetBytes(address);
			_Content = Encoding.ASCII.GetBytes(content);
			_Port = port;
			_Coding = Protocols.GsmDataCoding.Ascii;
		}

		public Message(string smsc, string address, byte[] content, int port)
		{
			_corn = new SMSP.SubmitMessage(Protocols.GsmDataCoding.Ascii);

			_corn.Data = content;
			if (_corn.Data.Length > 134)
				throw new NotSupportedException("Long Message Not Supported.");

			_corn.CenterAddress = new SMSP.Address(
				SMSP.DataRepresentation.Octet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				smsc);
			_corn.DestinationAddress = new SMSP.Address(
				SMSP.DataRepresentation.SemiOctet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				address);

			_Address = Encoding.ASCII.GetBytes(address);
			_Content = content;
			_Port = port;
			_Coding = Protocols.GsmDataCoding.Ascii;
		}

		public Message(GSMS.ShortMessage message)
		{
			SMSP.DeliverMessage ms = new SMSP.DeliverMessage(message.Content);
			_Address = Encoding.ASCII.GetBytes(ms.OriginatingAddress.Number);
			_Content = ms.Data;
			_Coding = ms.DataCodingScheme.MessageCoding;

			_Port = ms.DataHeaderIndication == Protocols.SMS.DataHeaderIndication.Yes ? 16001 : 0;
		}

		/// <summary>
		/// This is correct just for single-message messages
		/// </summary>
		public GSMS.ShortMessage Corn
		{
			get 
			{ 
				return (new GSMS.ShortMessage(_corn.Length[0], _corn.ToPDU()[0])); 
			}
		}

		/// <summary>
		///
		/// </summary>
		public string Address
		{
			get 
			{ 
				return (Encoding.ASCII.GetString(_Address)); 
			}
			internal set 
			{ 
				_Address = Encoding.ASCII.GetBytes(value); 
			}
		}

		/// <summary>
		/// this encoding is not generally correct for all cases. This may cause exception
		/// </summary>
		public byte[] Content
		{
			get { return (_Content); }
			internal set { _Content = value; }
		}

		/// <summary>
		/// Port
		/// </summary>
		public int Port
		{
			get { return (_Port); }
			internal set { _Port = value; }
		}

		/// <summary>
		/// Data Coding
		/// </summary>
		public Protocols.GsmDataCoding Coding
		{
			get { return (_Coding); }
			internal set { _Coding = value; }
		}
	}
}
