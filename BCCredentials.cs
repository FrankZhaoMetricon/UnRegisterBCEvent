using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnRegisterBCEvent
{
	public class BCCredentials
	{
		private string sADUserName;
		public string ADUserName
		{
			get { return sADUserName; }
			set { sADUserName = value; }
		}
		private string sBCCode;
		public string BCCode
		{
			get { return sBCCode; }
			set { sBCCode = value; }
		}
		private string sDisplayFirstName;
		private string sIP;
		public string IP
		{
			get { return sIP; }
			set { sIP = value; }
		}
		public string DisplayFirstName
		{
			get { return sDisplayFirstName; }
			set { sDisplayFirstName = value; }
		}
		private string sDisplaySurname;
		private string sCompany;
		public string Company
		{
			get { return sCompany; }
			set { sCompany = value; }
		}
		public string DisplaySurname
		{
			get { return sDisplaySurname; }
			set { sDisplaySurname = value; }
		}
		public BCCredentials()
		{
			//
			// TODO: Add constructor logic here
			//
			this.sADUserName = "Unknown";
			this.sBCCode = "Unknown";
			this.sCompany = "Unknown";
			this.sDisplayFirstName = "";
			this.sDisplaySurname = "";
			this.sIP = "";

		}

	}
}
