using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TableLog.Business
{
    public class TableManager : ITableManager
    {
        public List<string> ListTables(string connectionString, string tableName)
        {
            List<string> tables = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string cmdText = @"
                        select
	                        name
                        from sys.tables
                        where type = 'u'
                        and name like '%'+@name+'%'
                        order by name";

                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@name", tableName);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tables.Add(reader[0].ToString());
                    }

                    reader.Close();
                    conn.Close();

                    cmd.Dispose();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("connectionString", connectionString);
                ex.Data.Add("tableName", tableName);
                throw;
            }

            return tables;
        }

        public Models.Table ReadTableSchema(string connectionString, string tableName)
        {
            Models.Table table = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string cmdText = @"
                        select distinct
	                        table_name = C.TABLE_NAME,
	                        column_name = C.COLUMN_NAME,
	                        data_type = C.DATA_TYPE,
	                        max_length = C.CHARACTER_MAXIMUM_LENGTH,
	                        is_primary = case
		                        when KU.CONSTRAINT_NAME is not null then 1
		                        else 0
		                        end,
	                        is_required = case C.IS_NULLABLE
		                        when 'YES' then 0
		                        else 1
		                        end,
	                        collation_name = C.COLLATION_NAME
                        from INFORMATION_SCHEMA.COLUMNS as C
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS as TC
	                        on C.TABLE_NAME = TC.TABLE_NAME
	                        and C.TABLE_SCHEMA = TC.TABLE_SCHEMA
	                        and C.TABLE_CATALOG = TC.TABLE_CATALOG
				            and TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
                        left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE as KU
	                        on TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
	                        and C.COLUMN_NAME = KU.COLUMN_NAME
	                        and C.TABLE_NAME = KU.TABLE_NAME
	                        and C.TABLE_SCHEMA = KU.TABLE_SCHEMA
	                        and C.TABLE_CATALOG = KU.TABLE_CATALOG
                        where C.TABLE_NAME = @name
                        order by 
	                        table_name, 
	                        column_name";

                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@name", tableName);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    table = CreateTableModel(tableName, reader);

                    reader.Close();
                    conn.Close();

                    cmd.Dispose();
                    cmd = null;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("connectionString", connectionString);
                ex.Data.Add("tableName", tableName);
                throw;
            }

            return table;
        }

        private Models.Table CreateTableModel(string tableName, SqlDataReader reader)
        {
            Models.Table table = new Models.Table() { Name = tableName };

            while (reader.Read())
            {
                table.Columns.Add(new Models.Column()
                {
                    CollationName = checkStringNull(reader["collation_name"]),
                    DataType = reader["data_type"].ToString(),
                    IsPrimary = reader["is_primary"].ToString() == "1",
                    IsRequired = reader["is_required"].ToString() == "1",
                    MaxLength = checkIntegerNull(reader["max_length"]),
                    Name = reader["column_name"].ToString()
                });
            }

            return table;
        }

        private string checkStringNull(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }
            else
            {
                return v.ToString();
            }
        }

        private int? checkIntegerNull(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }
            else
            {
                return (int)v;
            }
        }
    }
}
