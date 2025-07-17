using System.Linq.Expressions;
using iDevCL;

namespace Repository
{
    public interface IGenericDataRepository<T> where T : class
    {
        void Dispose();
        IList<object> GetAllObject(Func<T, bool> includeProperties, Func<T, object> seletor, params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetAll(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetAllItens(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetAll(Func<T, bool> includeProperties, int skip, int take,
            params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetAll(Func<T, bool> includeProperties, Func<T, bool> order,
            params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetAll(Func<T, bool> includeProperties, Func<T, bool> order, int skip, int take,
            params Expression<Func<T, object>>[] navigationProperties);

        IList<object> GetAllOrder(Func<T, bool> includeProperties, Func<T, object> seletor, Func<T, object?> order,
            string tipo, params Expression<Func<T, object>>[] navigationProperties);

        IList<object> GetAllOrder(Func<T, bool> includeProperties, Func<T, object> seletor, Func<T, object> order,
            string tipo, int skip, int take, params Expression<Func<T, object>>[] navigationProperties);

        IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector,
            params Expression<Func<T, object>>[] navigationProperties);

        IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector, int skip, int take,
            params Expression<Func<T, object>>[] navigationProperties);

        T Find(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] navigationProperties);

        object FindObject(Func<T, bool> includeProperties, Func<T, object> seletor,
            params Expression<Func<T, object>>[] navigationProperties);

        IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector, DataTablesParam dtParams,
            out int totalRecords, out int totalRecordsDisplay, string[] columnNames, DataType[] types,
            string[] includes = null);

        T Find(Func<T, bool> includeProperties);
        bool Add(T item);
        bool Update(T item);
        bool Remove(T item);
    }
}