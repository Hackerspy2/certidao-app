#region

using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using iDevCL;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Repository;

public class GenericDataRepository<T> : IGenericDataRepository<T>, IDisposable where T : class
{
    private Contexto _context;

    public GenericDataRepository(Contexto context)
    {
        _context = context;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (_context == null) return;
        _context.Dispose();
        _context = null;
    }

    public virtual IList<T> GetAll(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).ToList();
        return list;
    }

    private static IQueryable<T> DbQueryInclude(Expression<Func<T, object>>[] includes, IQueryable<T> dbQuery)
    {
        if (!Equals(includes, null) && includes.Any())
            dbQuery = includes.Aggregate(dbQuery, (current, s) => current.Include(s));
        return dbQuery.AsNoTracking();
    }

    public virtual IList<T> GetAllItens(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).ToList();
        return list;
    }

    public virtual IList<object> GetAllObject(Func<T, bool> includeProperties, Func<T, object> seletor,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).Select(seletor).ToList();
        return list;
    }

    public virtual IList<T> GetAll(Func<T, bool> includeProperties, int skip, int take,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).Skip((skip - 1) * take).Take(take)
            .ToList();
        return list;
    }

    public virtual IList<T> GetAll(Func<T, bool> includeProperties, Func<T, bool> order,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderBy(order).ToList();
        return list;
    }

    public virtual IList<T> GetAll(Func<T, bool> includeProperties, Func<T, bool> order, int skip, int take,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderBy(order)
            .Skip((skip - 1) * take).Take(take)
            .ToList();
        return list;
    }

    public virtual IList<object> GetAllOrder(Func<T, bool> includeProperties, Func<T, object> seletor,
        Func<T, object?> order, string tipo, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = tipo == "asc"
            ? dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderBy(order).Select(seletor).ToList()
            : dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderByDescending(order)
                .Select(seletor).ToList();
        return list;
    }

    public virtual IList<object> GetAllOrder(Func<T, bool> includeProperties, Func<T, object> seletor,
        Func<T, object> order, string tipo, int skip, int take, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = tipo == "acs"
            ? dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderBy(order).Skip((skip - 1) * take)
                .Take(take)
                .Select(seletor).ToList()
            : dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).OrderByDescending(order)
                .Skip((skip - 1) * take)
                .Take(take).Select(seletor).ToList();
        return list;
    }

    public virtual IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).Select(selector).ToList();
        return list;
    }

    public virtual IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector, int skip,
        int take, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var list = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).Skip((skip - 1) * take).Take(take)
            .Select(selector).ToList();
        return list;
    }

    public virtual T Find(Func<T, bool> includeProperties, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        return dbQuery.AsNoTracking().FirstOrDefault(includeProperties)!;
    }

    public virtual object FindObject(Func<T, bool> includeProperties, Func<T, object> seletor,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _context.Set<T>();
        dbQuery = DbQueryInclude(includes, dbQuery);
        var item = dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).Select(seletor).FirstOrDefault();
        return item!;
    }

    public virtual T Find(Func<T, bool> includeProperties)
    {
        var item = _context.Set<T>().AsNoTracking().FirstOrDefault(includeProperties);
        if (item != null) _context.Entry(item).State = EntityState.Detached;
        return item!;
    }

    public virtual bool Add(T item)
    {
        using var tran = _context.Database.BeginTransaction();
        try
        {
            _context.Entry(item).State = EntityState.Added;
            _context.SaveChanges();
            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw;
        }
    }

    public virtual bool Update(T item)
    {
        using var tran = _context.Database.BeginTransaction();
        try
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw;
        }
    }

    public virtual bool Remove(T item)
    {
        using var tran = _context.Database.BeginTransaction();
        try
        {
            _context.Entry(item).State = EntityState.Deleted;
            _context.SaveChanges();
            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw;
        }
    }

    public virtual IList<object> GetAll(Func<T, bool> includeProperties, Func<T, object> selector,
        DataTablesParam dtParams, out int totalRecords, out int totalRecordsDisplay, string[] columnNames,
        DataType[] types, string[] includes = null!)
    {
        List<object> query;

        IQueryable<T> dbQuery = _context.Set<T>();

        if (!Equals(includes, null) && includes.Any())
            dbQuery = includes.Aggregate(dbQuery, (current, s) => current.Include(s));

        totalRecords = dbQuery.AsNoTracking().Count(includeProperties);

        var sortString = "";
        sortString = SortString(dtParams, columnNames, sortString);

        if (!string.IsNullOrEmpty(dtParams.sSearch))
        {
            var searchString = "";
            const bool first = true;
            searchString = SearchString(dtParams, columnNames, types, first, searchString);

            query = DynamicQueryableExtensions
                .OrderBy(
                    DynamicQueryableExtensions.Where(
                        dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).AsQueryable(),
                        searchString), sortString).Select(selector).ToList();
        }
        else
        {
            query = DynamicQueryableExtensions
                .OrderBy(dbQuery.AsNoTracking().AsEnumerable().Where(includeProperties).AsQueryable(), sortString)
                .Select(selector)
                .ToList();
            totalRecordsDisplay = query.Count;
        }

        totalRecordsDisplay = query.Count;
        return query.Skip(dtParams.iDisplayStart).Take(dtParams.iDisplayLength).ToList();
    }

    public virtual bool Update(T item, object ob1)
    {
        using var tran = _context.Database.BeginTransaction();
        try
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.Entry(ob1).State = EntityState.Modified;
            _context.SaveChanges();
            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw;
        }
    }

    private string SortString(DataTablesParam dtParams, IReadOnlyList<string> columnNames, string sortString)
    {
        for (var i = 0; i < dtParams.iSortingCols; i++)
        {
            var columnNumber = dtParams.iSortCol[i];
            var columnName = columnNames[columnNumber];
            var sortDir = dtParams.sSortDir[i];
            if (i != 0) sortString += ", ";
            if (columnName.Split('|').Length > 1) sortString += columnName.Split('|')[0] + " " + sortDir;
            else sortString += columnName + " " + sortDir;
        }

        return sortString;
    }

    private string SearchString(DataTablesParam dtParams, IReadOnlyList<string> columnNames,
        IReadOnlyList<DataType> types, bool first, string searchString)
    {
        for (var i = 0; i < dtParams.iColumns; i++)
            if (dtParams.bSearchable[i])
            {
                var columnName = columnNames[i];

                if (!first) searchString += " or ";
                else first = false;
                var termos = dtParams.sSearch.Split(' ');

                foreach (var t in termos)
                {
                    switch (types[i])
                    {
                        case DataType.tBool:
                            searchString += columnName + ".ToString().StartsWith(\"" + t + "\")";
                            break;
                        case DataType.tNullBool:
                            var condBool = columnName.Split('*');
                            searchString += columnName.Contains("*")
                                ? condBool[1] + " " + condBool[0] + ".ToString().StartsWith(\"" + t + "\")"
                                : columnName + ".ToString().StartsWith(\"" + t + "\")";
                            break;
                        case DataType.tInt:
                            searchString += columnName + ".ToString().StartsWith(\"" + t + "\")";
                            break;
                        case DataType.tNullInt:
                            var condInt = columnName.Split('*');
                            searchString += columnName.Contains("*")
                                ? condInt[1] + " " + condInt[0] + ".ToString().StartsWith(\"" + t + "\")"
                                : columnName + " != null && " + columnName + ".ToString().StartsWith(\"" + t + "\")";
                            break;
                        case DataType.tDate:
                            searchString += columnName + ".ToString().Contains(\"" + t + "\")";
                            break;
                        case DataType.tNullDate:
                            var condDate = columnName.Split('*');
                            searchString += columnName.Contains("*")
                                ? condDate[1] + " " + condDate[0] + ".Value.ToString().Contains(\"" + t + "\")"
                                : columnName + " != null && " + columnName + ".Value.ToString().Contains(\"" + t + "\")";
                            break;
                        case DataType.tDecimal:
                            searchString += columnName + ".ToString().Contains(\"" + t.ToLower() + "\")";
                            break;
                        case DataType.tNullDecimal:
                            var condDecimal = columnName.Split('*');
                            searchString += columnName.Contains("*")
                                ? condDecimal[1] + " " + condDecimal[0] + ".Value.ToString().Contains(\"" + t.ToLower() + "\")"
                                : columnName + " != null && " + columnName + ".Value.ToString().Contains(\"" + t + "\")";
                            break;
                        case DataType.tStringCondicao:
                            var condString = columnName.Split('*');
                            foreach (var c in condString)
                            {
                                searchString += c + ".ToLower().Contains(\"" + t.ToLower() + "\")";
                                searchString += " or ";
                            }

                            //searchString += condString[1] + " " + condString[0] + ".ToLower().Contains(\"" + t.ToLower() + "\")";
                            break;
                        default:
                            if (columnName.Contains("."))
                            {
                                searchString += columnName.Split('.')[0] + " != null && ";
                            }
                            searchString += columnName + " != null && " + columnName + ".ToLower().Contains(\"" +
                                            t.ToLower() + "\")";
                            //searchString += columnName + ".Contains(\"" + t.ToLower() + "\")";
                            break;
                    }

                    searchString += " or ";
                    first = true;
                    Trace.WriteLine(searchString);
                }
            }

        searchString += "|";
        searchString = searchString.Replace("or |", "");
        return searchString;
    }
}