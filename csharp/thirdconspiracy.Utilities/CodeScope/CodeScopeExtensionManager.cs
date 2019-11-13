using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.CodeScope
{
    internal sealed class CodeScopeExtensionManager : ICodeScopeExtensionManager
    {
        private CodeScope _codeScope;

        internal CodeScopeExtensionManager(CodeScope codeScope)
        {
            _codeScope = codeScope;
        }

        ICodeScope ICodeScopeExtensionManager.Scope => _codeScope;

        void ICodeScopeExtensionManager.Attach<T>(T value)
        {
            _codeScope.Attach(value);
        }

        T ICodeScopeExtensionManager.Detach<T>()
        {
            return _codeScope.Detatch<T>();
        }

        void ICodeScopeExtensionManager.AttachRoot<T>(T value)
        {
            var root = (CodeScope)_codeScope.Root;
            root.Attach(value);
        }

        T ICodeScopeExtensionManager.DetachRoot<T>()
        {
            var root = (CodeScope)_codeScope.Root;
            return root.Detatch<T>();
        }
    }
}
