import os
from pathlib import Path
import psycopg


def connection_args(value):
    parts = {}
    for item in value.split(";"):
        if "=" in item:
            key, raw = item.split("=", 1)
            parts[key.strip().lower()] = raw.strip()
    return {
        "host": parts.get("host") or parts.get("server"),
        "port": parts.get("port", "5432"),
        "dbname": parts.get("database"),
        "user": parts.get("username") or parts.get("user id"),
        "password": parts.get("password"),
        "sslmode": (parts.get("ssl mode") or "prefer").lower(),
    }


names = ("Menstrual Leave", "Bereavement Leave", "Birthday Leave")
for label, environment_name in (("Local", "AXION_LOCAL_CS"), ("Render", "AXION_RENDER_CS")):
    with psycopg.connect(**connection_args(os.environ[environment_name])) as connection:
        with connection.cursor() as cursor:
            cursor.execute(
                '''
                SELECT lt."LeaveName", COUNT(*)
                FROM axionpro."LeaveType" lt
                JOIN axionpro."Tenant" t ON t."Id" = lt."TenantId"
                WHERE t."CompanyName" = %s
                  AND lt."LeaveName" = ANY(%s)
                  AND lt."IsSoftDeleted" IS NOT TRUE
                GROUP BY lt."LeaveName"
                ORDER BY lt."LeaveName"
                ''',
                ("TechNova Solutions Pvt. Ltd.", list(names)),
            )
            rows = cursor.fetchall()
            print(f"{label}: " + ", ".join(f"{name}={count}" for name, count in rows))
