using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcMan.VersionControl
{
	public interface IVersionControl
	{
		void Update(bool overwriteChanges);
		string Status();
	}
}
