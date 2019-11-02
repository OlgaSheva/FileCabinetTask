using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected static IFileCabinetService service;

        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService;
        }
    }
}
