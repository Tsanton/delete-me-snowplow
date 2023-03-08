namespace Snowplow.Models.Describables;

public class Table: ISnowflakeDescribable
{
    public string? DatabaseName { get; init; }
    public string? SchemaName { get; init; }
    public string? TableName { get; init; }
    public string GetDescribeStatement()
    {
        // return $"SHOW TABLES LIKE '{TableName}' IN SCHEMA {DatabaseName}.{SchemaName}".ToUpper();
        var query = $@"
with show_table_description as procedure(db_name varchar, schema_name varchar, table_name varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_table_description_py'
as $$
import json
def show_table_description_py(snowpark_session, db_name_py:str, schema_name_py:str, table_name_py:str):
    table = snowpark_session.sql(f""SHOW TABLES like '{{table_name_py}}' IN SCHEMA {{db_name_py}}.{{schema_name_py}}"").collect()[0].as_dict()
    res = []
    for row in snowpark_session.sql(f'DESCRIBE TABLE {{db_name_py}}.{{schema_name_py}}.{{table_name_py}}').to_local_iterator():
        col = snowpark_session.sql(f""SHOW COLUMNS LIKE '{{row['name']}}' IN TABLE {{db_name_py}}.{{schema_name_py}}.{{table_name_py}}"").collect()[0].as_dict()
        res.append({{**row.as_dict(), **{{'auto_increment': col['autoincrement'] if col['autoincrement'] != '' else None, 'data_type': json.loads(col['data_type'])}} }})
    table['columns'] = res
    return table
$$
call show_table_description('TEST_SNOWPLOW_DB', 'TEST_SNOWPLOW_SCHEMA', 'TEST_TABLE')";
        return query;
    }
}
