using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcMan.Runners
{
	public class RunIISExpress : IRun
	{
		private readonly string _name;
		private readonly string _siteName;

		public RunIISExpress(string name, string location)
		{
			_name = name;
			_siteName = location;
		}
		public void Run()
		{
			throw new NotImplementedException();
		}

		public string GetStatus()
		{
			throw new NotImplementedException();
		}
	}
}
