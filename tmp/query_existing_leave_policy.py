import os
import psycopg


def connection_args(value):
    parts = {}
    for item in value.split(";"):
        if "=" in item:
            key, setting = item.split("=", 1)
            parts[key.strip().lower()] = setting.strip()
    return {
        "host": parts.get("host") or parts.get("server"),
        "port": parts.get("port", "5432"),
        "dbname": parts.get("database"),
        "user": parts.get("username") or parts.get("user id"),
        "password": parts.get("password"),
        "sslmode": (parts.get("ssl mode") or "prefer").lower(),
    }


with psycopg.connect(**connection_args(os.environ["AXION_RENDER_CS"])) as connection:
    with connection.cursor() as cursor:
        cursor.execute(
            '''
            SELECT p."Id", p."PolicyCode", p."PolicyName",
                   v."Id", v."VersionNumber", v."PolicyStatusId"
            FROM axionpro."Policy" p
            JOIN axionpro."PolicyVersion" v ON v."PolicyId" = p."Id"
            WHERE p."TenantId" = 7
              AND (p."PolicyCode" ILIKE %s OR p."PolicyName" ILIKE %s)
            ORDER BY p."Id" DESC
            ''',
            ("%LEAVE_2026_2027%", "%Leave Policy 2026-2027%"),
        )
        print(cursor.fetchall())
