using Dapper;
using MyApp.Util;
using MyApp.Common;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL
{
    public static class QueryGenerator
    {

        // Create a SQL INSERT Query like:
        // INSERT into Product(Id, Name) Values (@Id, @Name)
        // It uses reflection to read the properties from the model
        public static string GenerateInsertQuery(Type InputModel, Dictionary<string, string>? FieldCustomSQL =null, string? TableName = null,int? AppUserId = null)
        {
            TableName ??= GetTableNameFromModelName(InputModel);
            var insertQuery = new StringBuilder($"INSERT INTO [{TableName}] ");

            insertQuery.Append("(");
            var propertyList = ObjectMapper.GetListOfProperties(InputModel);

            // Fieldnames
            propertyList.ForEach(prop =>
            {
                if (!(prop.Contains(TableName + "Id") || prop == "Id")) // Ignore Id, as it is AutoNumber
                {
                    insertQuery.Append($"[{prop}],");
                }
            });
            if (FieldCustomSQL != null)
            {
                foreach (var item in FieldCustomSQL)
                {
                    insertQuery.Append($"[{item.Key}],");
                }
            }
            insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");

            // Dapper ParameterList
            propertyList.ForEach(prop =>
            {
                if (!(prop.Contains(TableName + "Id") || prop == "Id"))
                {
                    if (new List<string> { "CreatedBy", "UpdatedBy" }.ConvertAll(l => l.ToLower()).Contains(prop.ToString().ToLower()))
                    {
                        insertQuery.Append($"'{AppUserId}',"); // Utc date function of SQL
                    }
                    else if ((new List<string> { "CreateDateTimeUtc", "UpdateDateTimeUtc" }.ConvertAll(l => l.ToLower())).Contains(prop.ToString().ToLower()))
                    {
                        insertQuery.Append($"GetUtcDate(),"); // Utc date function of SQL
                    }
                    else
                    {
                        insertQuery.Append($"@{prop},");
                    }

                }
            });
            if (FieldCustomSQL != null)
            {
                foreach (var item in FieldCustomSQL)
                {
                    insertQuery.Append($"{item.Value},");
                }
            }

            insertQuery.Remove(insertQuery.Length - 1, 1).Append(")");
            insertQuery.Append(" ;SELECT SCOPE_IDENTITY()");
            return insertQuery.ToString();
        }

        // This works like GenerateInsertQuery but for multiple inserts, when inumerable is passed
        public static string GenerateInsertQueryMultiple<T>(IEnumerable<T> entity, string TableName)
        {
            if (TableName == null)
            {
                TableName = GetTableNameFromModelName(typeof(T));
            }
            var insertQuery = new StringBuilder($"INSERT INTO [{TableName}] ");

            insertQuery.Append("(");
            var propertyList = ObjectMapper.GetListOfProperties(typeof(T));

            propertyList.ForEach(prop =>
            {
                if (!prop.Contains(TableName + "Id"))
                    insertQuery.Append($"[{prop}],");
            });

            insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");

            foreach (var item in entity.ToList())
            {
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    if (!prop.Name.Contains(TableName + "Id"))
                    {
                        if (prop.GetValue(item, null) != null && prop.GetValue(item, null)!.ToString()!.Trim() != "")
                        {
                            if (prop.Name.ToLower().Contains("dateutc"))
                            {
                                insertQuery.Append($"getutcdate(),");
                            }
                            else
                            {
                                insertQuery.Append($"'{prop.GetValue(item, null)}',");
                            }
                        }
                        else
                        {
                            insertQuery.Append($"null,");
                        }
                    }
                }
                insertQuery.Remove(insertQuery.Length - 1, 1).Append("),(");
            }

            insertQuery.Remove(insertQuery.Length - 2, 2);
            return insertQuery.ToString();
        }

        // Similar to the concept of GenerateInsertQuery, it creates UPDATE query
        public static string GenerateUpdateQuery(Type InputModel, string? TableName = null, int? AppUserId = null)
        {
            if (TableName == null)
            {
                TableName = GetTableNameFromModelName(InputModel);
            }
            var updateQuery = new StringBuilder($"UPDATE [{TableName}] SET ");

            var propertyList = ObjectMapper.GetListOfProperties(InputModel);

            propertyList.ForEach(property =>
            {
                if (!(new List<string> { "CreatedBy", "CreateDateTimeUtc", TableName + "Id" ,"Id"}.Contains(property)))
                {
                    if (new List<string> { "UpdatedBy" }.ConvertAll(l => l.ToLower()).Contains(property.ToString().ToLower()))
                    {
                        updateQuery.Append($"UpdatedBy='{AppUserId}',"); // Utc date function of SQL
                    }
                    else if ((new List<string> { "UpdateDateTimeUtc" }.ConvertAll(l => l.ToLower())).Contains(property.ToString().ToLower()))
                    {
                        updateQuery.Append($"UpdateDateTimeUtc=GetUtcDate(),"); // Utc date function of SQL
                    }
                    else
                    {
                        updateQuery.Append($"{property}=@{property},");
                    }
                }

            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            //var idValue = propertyList.Where(m => m.Equals(TableName + "Id")).First();
            if (InputModel.GetProperty($"{TableName}Id")!=null)
            {
                updateQuery.Append($" WHERE {TableName}Id =@{TableName}Id");
            }
            else
            {
                updateQuery.Append($" WHERE Id = @Id");
            }
            
            updateQuery.Append(" ;SELECT @@RowCount");

            return updateQuery.ToString();
        }


        // Generates a DELETE query
        public static string GenerateDeleteQuery(int id, Type TModel)
        {
            var TableName = GetTableNameFromModelName(TModel);
            var updateQuery = new StringBuilder($"DELETE [{TableName}]  ");

            if (TModel.GetProperty($"{TableName}Id") != null)
            {
                updateQuery.Append($"WHERE {TableName}Id =" + id);
            }
            else
            {
                updateQuery.Append($"WHERE Id =" + id);
            }
            //updateQuery.Append($"WHERE {TableName}Id =" + id);
            updateQuery.Append(" ;SELECT @@RowCount");

            return updateQuery.ToString();
        }


        public static string GetTableNameFromModelName(Type InputModel)
        {
            string modelName = InputModel.Name;

            if (modelName.EndsWith("Model", StringComparison.InvariantCultureIgnoreCase))
            {
                return modelName.Substring(0, modelName.Length - 5);
            }
            return modelName;
        }

        public static string AppendUserIdWhereClause(JWTAuth account)
        {
            int? userId = account.Id; // TODO
            if (account.RoleName == Enums.UserRoles.SuperAdmin || account.RoleName == Enums.UserRoles.Admin)
            {
                userId = null;
            }
            string whereClause = userId != null ? $" AND A.Id = {userId}" : "";
            return whereClause;
        }

       
    }
}
