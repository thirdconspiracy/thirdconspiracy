using System;
using System.Collections.Generic;
using thirdconspiracy.Logger;
using thirdconspiracy.Logger.Null;

namespace thirdconspiracy.Utilities.CodeScope
{
    class CodeScopeManager
    {
        private const string CODE_SCOPE_KEY = "CodeScope";

        private static object _lockObject = new object();
        private static volatile CodeScopeManager _insance;

        private DateTime _nextCheckPoint;
        private List<ICodeScopeListener> _listeners;
        private Dictionary<Guid, WeakReference<CodeScope>> _scopes;

        private static readonly TimeSpan CHECKPOINT_INTERVAL = TimeSpan.FromMinutes(5.0d);

        private CodeScopeManager()
        {
            _listeners = new List<ICodeScopeListener>();
            _scopes = new Dictionary<Guid, WeakReference<CodeScope>>();
            _nextCheckPoint = DateTime.UtcNow.Add(CHECKPOINT_INTERVAL);
        }

        #region Public
        public static void Subscribe(ICodeScopeListener listener)
        {
            Instance.AddListener(listener);
        }

        public static void UnSubscribe(ICodeScopeListener listener)
        {
            Instance.RemoveListener(listener);
        }

        public static ICodeScope GetCurrentScope()
        {
            lock (_lockObject)
            {
                ICodeScope root = Instance.GetCurrentRoot();
                if (root == null)
                {
                    return null;
                }

                var scope = root;
                while (scope.NextNode != null)
                {
                    scope = scope.NextNode;
                }
                return scope;
            }
        }

        public static ICodeScope GetCurrentRootScope()
        {
            return Instance.GetCurrentRoot();
        }

        #endregion

        #region Scope Lifecycle
        internal static void BeginScope(CodeScope codeScope)
        {
            Instance.OnScopeStarting(codeScope);
        }

        internal static void EndScope(CodeScope codeScope)
        {
            Instance.OnScopeEnding(codeScope);
        }
        #endregion

        private void OnScopeStarting(CodeScope codeScope)
        {
            lock (_lockObject)
            {
                //see if we already have a chain started
                var root = GetCurrentRoot();
                if (root != null)
                {
                    codeScope.SetRoot(root.ChainID, root);
                    Add(root, codeScope);
                }
                else
                {
                    //start a new root scope chain
                    var chainID = Guid.NewGuid();
                    codeScope.SetRoot(chainID, codeScope);
                    codeScope.NextNode = null;
                    codeScope.PreviousNode = null;

                    CallContext.SetData(CODE_SCOPE_KEY, codeScope.ChainID);
                    AddRoot(codeScope);
                }
            }

            Notify(codeScope, true);
            OnOperationFinished();
        }

        private void OnScopeEnding(CodeScope codeScope)
        {
            lock (_lockObject)
            {
                var root = GetRoot(codeScope.ChainID);
                if (root != null)
                {
                    var tail = Remove(root);
                    if (root == tail)
                    {
                        RemoveRoot(root);
                        CallContext.FreeNamedDataSlot(CODE_SCOPE_KEY);
                    }
                }
            }

            Notify(codeScope, false);
            OnOperationFinished();
        }

        private void AddListener(ICodeScopeListener listener)
        {
            lock (_lockObject)
            {
                _listeners.Add(listener);
            }
        }

        private void RemoveListener(ICodeScopeListener listener)
        {
            lock (_lockObject)
            {
                _listeners.Remove(listener);
            }
        }

        private CodeScope GetCurrentRoot()
        {
            var key = CallContext.GetData(CODE_SCOPE_KEY);
            if (key is Guid id)
            {
                return GetRoot(id);
            }

            //we are not in an active tracking scope on the calling threads execution context
            return null;
        }

        private CodeScope GetRoot(Guid id)
        {
            lock (_lockObject)
            {
                if (_scopes.TryGetValue(id, out var scopeRef))
                {
                    if (scopeRef.TryGetTarget(out var scope))
                    {
                        return scope;
                    }
                }
            }

            //no scope found for ID
            return null;
        }

        private void OnOperationFinished()
        {
            CheckPoint();
        }

        private void AddRoot(CodeScope root)
        {
            lock (_lockObject)
            {
                _scopes.Add(root.ChainID, new WeakReference<CodeScope>(root));
            }
        }

        private void RemoveRoot(CodeScope root)
        {
            lock (_lockObject)
            {
                _scopes.Remove(root.ChainID);
            }
        }

        private void Add(CodeScope head, CodeScope node)
        {
            if (head == null)
            {
                throw new ArgumentNullException(nameof(head));
            }

            var current = head;
            while (current.NextNode != null)
            {
                current = (CodeScope)current.NextNode;
            }

            current.NextNode = node;
            node.PreviousNode = current;
        }

        private CodeScope Remove(CodeScope head)
        {
            CodeScope last = null;

            while (head != null)
            {
                if (head.NextNode == null)
                {
                    last = head;
                }
                head = (CodeScope)head.NextNode;
            }

            if (last?.PreviousNode != null)
            {
                var previous = (CodeScope)last.PreviousNode;
                previous.NextNode = null;
            }

            return last;
        }

        private void Notify(CodeScope scope, bool starting)
        {
            var extensionManager = new CodeScopeExtensionManager(scope);

            lock (_lockObject)
            {

                try
                {
                    foreach (var listener in _listeners)
                    {
                        try
                        {
                            if (starting)
                            {
                                listener.Started(extensionManager);
                            }
                            else
                            {
                                listener.Ended(extensionManager);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(LogLevel.Error, "Failed to process code scope listener", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, "General failure processing code scope listeners", ex);
                }
            }
        }

        private void CheckPoint()
        {
            var now = DateTime.UtcNow;
            if (now <= _nextCheckPoint)
            {
                return;
            }

            lock (_lockObject)
            {
                if (now > _nextCheckPoint)
                {
                    try
                    {
                        Collect();
                    }
                    finally
                    {
                        _nextCheckPoint = DateTime.UtcNow.Add(CHECKPOINT_INTERVAL);
                    }
                }
            }
        }

        private void Collect()
        {
            lock (_lockObject)
            {
                try
                {
                    var finalizationList = new List<Guid>();

                    //mark any old scopes that have been GCd
                    foreach (var kvp in _scopes)
                    {
                        if (!kvp.Value.TryGetTarget(out _))
                        {
                            finalizationList.Add(kvp.Key);
                        }
                    }

                    //process the finalization list
                    foreach (var key in finalizationList)
                    {
                        _scopes.Remove(key);
                    }
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, "DiagnosticManager failed to GC collected scopes", ex);
                }
            }
        }

        private void Log(LogLevel level, string message, Exception ex)
        {
            //NOTE: the logger is not thread safe, so we create it each time
            ILogger logger = new NullLogger();//TODO: Create logger eventually
            logger.Log(level, message, ex);
        }

        private static CodeScopeManager Instance
        {
            get
            {
                if (_insance == null)
                {
                    lock (_lockObject)
                    {
                        if (_insance == null)
                        {
                            _insance = new CodeScopeManager();
                        }
                    }
                }
                return _insance;
            }
        }
    }
}
