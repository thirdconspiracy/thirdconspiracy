using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.CodeScope
{
    public class CodeScope : ICodeScope
    {
        #region Member Variables

        private readonly Guid _id;
        private readonly string _domain;
        private readonly string _name;

        private Guid _chainID;
        private ICodeScope _root;
        private int _managedThreadID;
        private bool _hasException;
        private ConcurrentDictionary<Type, ICodeScopeExtension> _extensions;
        private object _lockObject = new object();

        #endregion Member Variables

        #region CTOR

        public CodeScope(string domain, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentException("Domain is required", nameof(domain));
            }

            _id = Guid.NewGuid();
            _domain = domain;
            _name = name;

            _hasException = false;
            _extensions = new ConcurrentDictionary<Type, ICodeScopeExtension>();
            _managedThreadID = Thread.CurrentThread.ManagedThreadId;

            CodeScopeManager.BeginScope(this);
        }
        #endregion CTOR

        public Guid ID => _id;

        public Guid ChainID => _chainID;

        public int ManagedThreadID => _managedThreadID;

        public string Name => _name;

        public string Domain => _domain;

        public string Path
        {
            get
            {
                lock (_lockObject)
                {
                    if (_root != null)
                    {
                        var builder = new StringBuilder();
                        builder.Append(_root.Name);

                        var current = _root;
                        while (current.NextNode != null)
                        {
                            builder.AppendFormat("->{0}", current.NextNode.Name);
                            current = current.NextNode;
                        }

                        return builder.ToString();
                    }

                    return string.Empty;
                }
            }
        }

        public bool HasException => _hasException;

        public ICodeScope Root => _root;

        public ICodeScope NextNode { get; internal set; }

        public ICodeScope PreviousNode { get; internal set; }

        public ICodeScopeExtension GetExtension<T>() where T : ICodeScopeExtension
        {
            ICodeScopeExtension extension = default(T);
            _extensions.TryGetValue(typeof(T), out extension);
            return (T)extension;
        }

        public object SyncRoot => _lockObject;

        internal void SetRoot(Guid chainID, ICodeScope root)
        {
            _root = root;
            _chainID = chainID;
        }

        internal void Attach<T>(T extension) where T : ICodeScopeExtension
        {
            if (extension == null)
            {
                return;
            }

            if (!_extensions.TryAdd(typeof(T), extension))
            {
                throw new ArgumentException($"Extension of type '{typeof(T)}' already exists", nameof(extension));
            }
        }

        internal T Detatch<T>() where T : ICodeScopeExtension
        {
            ICodeScopeExtension extension = default(T);
            _extensions.TryRemove(typeof(T), out extension);
            return (T)extension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var codeScope = obj as CodeScope;
            if ((Object)codeScope == null)
            {
                return false;
            }

            return codeScope.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return _name ?? _id.ToString("D");
        }

        protected virtual void OnDispose()
        {
        }

        public void Dispose()
        {
            _hasException = Marshal.GetExceptionCode() != 0;
            CodeScopeManager.EndScope(this);
            OnDispose();
        }
    }
}
