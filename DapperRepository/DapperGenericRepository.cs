using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DapperRepository
{
    public class DapperGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private IEnumerable<PropertyInfo> GetProperties => typeof(TEntity).GetProperties();

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);

            // properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            for (int i = 1; i < properties.Count; i++)
            {
                insertQuery.Append($"[{properties[i]}],");
            }

            insertQuery

             .Remove(insertQuery.Length - 1, 1)

             .Append(") VALUES (");

            // properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            for (int i = 1; i < properties.Count; i++)
            {
                insertQuery.Append($"@{properties[i]},");
            }

            insertQuery

             .Remove(insertQuery.Length - 1, 1)

             .Append(")");

            return insertQuery.ToString();

        }

        private string GenerateUpdateQuery()
        {

            var updateQuery = new StringBuilder($"UPDATE {tableName} SET ");

            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma

            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();

        }

        private string tableName;

        private IDbConnection dbConnection;

        public DapperGenericRepository(string tableName, IDbConnection dbConnection)
        {

            this.tableName = tableName;

            this.dbConnection = dbConnection;

        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().AsQueryable().Where(predicate).ToList();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbConnection.Query<TEntity>($"SELECT * FROM {tableName}");
        }

        public TEntity FindById(object id)
        {
            return dbConnection.Query<TEntity>($"SELECT * FROM {tableName} WHERE Id=@Id", new { Id = id }).FirstOrDefault();
        }

        public void Add(TEntity item)
        {
            var insertQuery = GenerateInsertQuery();
            dbConnection.Query(insertQuery, item);
        }

        public void Update(TEntity item)
        {
            var updateQuery = GenerateUpdateQuery();
            dbConnection.Query(updateQuery, item);
        }

        public void Remove(int id)
        {
            dbConnection.Query($"DELETE FROM {tableName} WHERE Id=@Id", new { Id = id });
        }

        public TEntity FindByName(object name)
        {
            return dbConnection.Query<TEntity>($"SELECT * FROM {tableName} WHERE Name=@Name", new { Name = name }).FirstOrDefault();

        }
    }
}
