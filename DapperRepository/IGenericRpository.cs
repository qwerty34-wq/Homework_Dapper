using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Add(TEntity item);

        TEntity FindById(object id);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        void Remove(int id);

        void Update(TEntity item);
    }
}
