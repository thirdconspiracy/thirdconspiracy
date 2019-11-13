namespace thirdconspiracy.Utilities.CodeScope
{
    public interface ICodeScopeExtensionManager
    {
        ICodeScope Scope { get; }

        void Attach<T>(T value) where T : ICodeScopeExtension;

        T Detach<T>() where T : ICodeScopeExtension;

        void AttachRoot<T>(T value) where T : ICodeScopeExtension;

        T DetachRoot<T>() where T : ICodeScopeExtension;
    }
}
