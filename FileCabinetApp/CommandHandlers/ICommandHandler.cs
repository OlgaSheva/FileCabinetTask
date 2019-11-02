namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The Handler interface declares a method for constructing a chain of handlers.
    /// It also declares a method to execute the request.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>The next handler.</returns>
        ICommandHandler SetNext(ICommandHandler handler);

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Class AppCommandRequest Instance.</returns>
        AppCommandRequest Handle(AppCommandRequest request);
    }
}
