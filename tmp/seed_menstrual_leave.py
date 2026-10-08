import os, re
from pathlib import Path
import psycopg
root=Path(r'C:\AxionProCodeBase\QuecksilberTechnologies')
sql=(root/'database-scripts'/'AddPolicyLeaveTypeTargeting.sql').read_text(encoding='utf-8')

def connstr(data):
    pairs={}
    for part in data.split(';'):
        if '=' in part:
            k,v=part.split('=',1); pairs[k.strip().lower()]=v.strip()
    ssl=(pairs.get('ssl mode') or pairs.get('sslmode') or 'prefer').lower()
    if ssl == 'require': ssl='require'
    return dict(host=pairs.get('host') or pairs.get('server'), port=pairs.get('port','5432'), dbname=pairs.get('database'), user=pairs.get('username') or pairs.get('user id'), password=pairs.get('password'), sslmode=ssl)
for label,cs in [('Local',os.environ['AXION_LOCAL_CS']),('Render',os.environ['AXION_RENDER_CS'])]:
    with psycopg.connect(**connstr(cs)) as conn:
        with conn.cursor() as cur:
            cur.execute(sql)
            cur.execute('''SELECT COUNT(*) FROM axionpro."LeaveType" lt JOIN axionpro."Tenant" t ON t."Id"=lt."TenantId" WHERE t."CompanyName"=%s AND lower(lt."LeaveName")=lower(%s) AND lt."IsSoftDeleted" IS NOT TRUE''',('TechNova Solutions Pvt. Ltd.','Menstrual Leave'))
            print(f'{label}: menstrual_leave_rows={cur.fetchone()[0]}')

