using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        int Create(T entity);

        T? Read(int id);

        //List<T> ReadAll();

        T? Read(Func<T, bool> filter); // stage 2

        void Update(T entity);

        void Delete(int id);

        IEnumerable<T?> ReadAll(Func<T, bool>? filter = null); // stage 2

    }
}
