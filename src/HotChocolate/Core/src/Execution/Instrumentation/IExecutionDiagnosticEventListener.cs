using HotChocolate.Resolvers;

namespace HotChocolate.Execution.Instrumentation
{
    /// <summary>
    /// Register an implementation of this interface in the DI container to
    /// listen to diagnostic events. Multiple implementations can be registered
    /// and they will all be called in the registration order.
    /// </summary>
    /// <seealso cref="ExecutionDiagnosticEventListener"/>
    public interface IExecutionDiagnosticEventListener : IExecutionDiagnosticEvents
    {
        /// <summary>
        /// Return true to tell the event dispatcher that the listener has
        /// implemented
        /// <see cref="IExecutionDiagnosticEvents.ResolveFieldValue(IMiddlewareContext)"/> or
        /// <see cref="IExecutionDiagnosticEvents.RunTask(IExecutionTask)"/>.
        /// </summary>
        /// <remarks>
        /// Returning false allow the event dispatcher to avoid
        /// making unnecessary calls for these frequent events. It is not a
        /// guarantee that the methods will not be called.
        /// </remarks>
        bool EnableResolveFieldValue { get; }
    }
}
