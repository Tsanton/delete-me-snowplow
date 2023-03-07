using Snowplow.Models.Enums;

namespace Snowplow.Models.Describables;


public class Grant : ISnowflakeDescribable
{
    public ISnowflakeGrantTarget Target { get; init; }
    
    public string GetDescribeStatement()
    {
        string principalType;
        string principalIdentifier;
        switch (Target)
        {
            case Role principal:
                principalType = SnowflakePrincipal.Role.GetSnowflakeType();
                principalIdentifier = principal.Name;
                break;
            case DatabaseRole principal:
                principalType = SnowflakePrincipal.DatabaseRole.GetSnowflakeType();
                principalIdentifier = $"{principal.DatabaseName}.{principal.Name}";
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
            var query = $@"
with show_grants_to_role as procedure(principal_type varchar, principal_identifier varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_grants_to_role_py'
as $$
def show_grants_to_role_py(snowpark_session, principal_type_py:str, principal_identifier_py:str):
    res = []
    for row in snowpark_session.sql(f'SHOW GRANTS TO {{principal_type_py}} {{principal_identifier_py}}').to_local_iterator():
        res.append(row.as_dict())
    return res
$$
call show_grants_to_role('{principalType}', '{principalIdentifier}');";
                return query;
    }
}