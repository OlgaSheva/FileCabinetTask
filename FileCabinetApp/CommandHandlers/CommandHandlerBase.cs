using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ICommandHandler" />
    public class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// The next handler.
        /// </returns>
        public ICommandHandler SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
            return handler;
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Class AppCommandRequest Instance.
        /// </returns>
        public virtual AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (this.nextHandler != null)
            {
                return this.nextHandler.Handle(request);
            }
            else
            {
                return null;
            }
        }
    }
}