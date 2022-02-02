namespace EKOLidl.Services.Data
{
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public interface ISignalService
    {
        public ICollection<Signal> GetAll();

        public Signal AddAsync(Signal signalToAdd);

        public Signal UpdateAsync(Signal signalToAdd);

        public bool RemoveAsync(Signal signalToRemove);
    }
}
