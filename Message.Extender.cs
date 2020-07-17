/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;
using System.Text;
using SMSP = sirius.GSM.Protocols.SMS;

namespace sirius.GSM
{
	/// <summary>
	/// 
	/// </summary>
	public sealed partial class Message
	{
		#region Functions

		public static Message NewEx(string smsc, string address, byte[] content, int port) 
		{
			Message m = new Message();
			m._corn = new SMSP.SubmitMessage(Protocols.GsmDataCoding.GsmDefault, port);
			m._corn.Data = content;
			if (m._corn.Data.Length > 154)
				throw new NotSupportedException("Long Message Not Supported.");

			m._corn.CenterAddress = new SMSP.Address(
				SMSP.DataRepresentation.Octet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				smsc);
			m._corn.DestinationAddress = new SMSP.Address(
				SMSP.DataRepresentation.SemiOctet,
				SMSP.NumberType.InternationalNumber,
				SMSP.NumberingPlan.IsdnOrTelephone,
				address);

			m._Address = Encoding.ASCII.GetBytes(address);
			m._Content = content;
			m._Port = port;
			m._Coding = Protocols.GsmDataCoding.GsmDefault;

			return (m);
		}

		#endregion
	}
}
