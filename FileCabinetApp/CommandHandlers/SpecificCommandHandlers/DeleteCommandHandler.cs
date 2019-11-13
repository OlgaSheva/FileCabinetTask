using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Class AppCommandRequest Instance.
        /// </returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "delete")
            {
                this.Delete(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Delete(string parameters)
        {
            var words = parameters.Split(
                new char[] { ' ', ',', '.', ':', ';', '(', ')', '\'', '!', '?', '\t' }).Where(s => s.Length > 0).ToList();
            string key = null;
            string value = null;
            if (words[0].Equals("where", StringComparison.InvariantCultureIgnoreCase))
            {
                key = words[1];
                value = words[2];
            }

            try
            {
                switch (key.ToLower(CultureInfo.DefaultThreadCurrentCulture))
                    {
                    case "id":
                        DeleteById(value);
                        break;
                    case "firstname":
                        break;
                    case "lastname":
                        break;
                    case "dateofbirth":
                        break;
                    default:
                        throw new ArgumentException(nameof(key), $"Search by key '{key}' does not supported.");
                }
            }
            catch (ArgumentException)
            {
                throw;
            }

            void DeleteById(string parameter)
            {
                if (!int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) || id == 0)
                {
                    throw new ArgumentException($"Record with id = '{parameter}' does not exists.");
                }

                if (this.Service.IsThereARecordWithThisId(id, out long position))
                {
                    this.Service.Remove(id, position);
                    Console.WriteLine($"Record #{id} is removed.");
                }
                else
                {
                    Console.WriteLine($"Record #{id} didn't found.");
                }
            }
        }
    }
}
