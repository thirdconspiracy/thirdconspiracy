using System;

namespace thirdconspiracy.Utilities.CodeScope
{
    public interface ICodeScope : IDisposable
    {
        Guid ID { get; }

        Guid ChainID { get; }

        int ManagedThreadID { get; }

        string Name { get; }

        string Domain { get; }

        bool HasException { get; }

        ICodeScope Root { get; }

        ICodeScope NextNode { get; }

        ICodeScope PreviousNode { get; }

        object SyncRoot { get; }

        ICodeScopeExtension GetExtension<T>() where T : ICodeScopeExtension;
    }
}
