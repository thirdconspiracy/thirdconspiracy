using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.CodeScope
{
    public interface ICodeScopeListener
    {
        void Started(ICodeScopeExtensionManager extensionManager);

        void Ended(ICodeScopeExtensionManager extensionManager);
    }
}
