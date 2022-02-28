using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcMan.Runners
{
	public interface IRun
	{
		void Run();
		string GetStatus();
	}
}
