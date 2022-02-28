using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcMan.Runners
{
	public class RunDotNet : IRun
	{
		private readonly string _name;
		private readonly string _path;

		private bool _running = false;

		public RunDotNet(string name, string path)
		{
			_name = name;
			_path = path;
		}

		//TODO: log errors

		public void Run()
		{
			try
			{
				throw new NotImplementedException();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_running = false;
			}
		}

		public string GetStatus()
		{
			if (_running)
				return $"{_name} is running";

			return $"{_name} is stopped";
		}
	}
}
