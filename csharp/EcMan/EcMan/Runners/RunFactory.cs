using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcMan.Runners
{
	public class RunFactory
	{
		public enum RunType
		{
			DotNet,
			IISExpress,
			Exe
		}

		public IRun Instance(RunType runType, string name, string location)
		{
			switch (runType)
			{
				case RunType.DotNet:
					return new RunDotNet(name, location);
				case RunType.IISExpress:
					return new RunIISExpress(name, location);
				case RunType.Exe:
				default:
					throw new ArgumentOutOfRangeException(nameof(runType), runType, "Concrete definition not defined");
			}

		}
	}
}
