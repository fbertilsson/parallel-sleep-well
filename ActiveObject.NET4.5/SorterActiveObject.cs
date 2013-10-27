using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActiveObject.NET45
{
    public class SorterActiveObject
    {
        public Task<IEnumerable<string>> SortAsync(IEnumerable<string> strings)
        {
            var future = new Task<IEnumerable<string>>(
                () => Sort(strings));
            future.Start();
            return future;
        }

        public IEnumerable<string> Sort(IEnumerable<string> strings)
        {
            return strings.Select(s => s).OrderBy(s => s);
        }




        public Task<IEnumerable<string>> SortAsync2(IEnumerable<string> strings)
        {
            var stringsCopy = strings.ToList(); // We need private data
            var task = new Task<IEnumerable<string>>(() => Sort(stringsCopy));
            task.Start();
            return task;
        }

    }
}
