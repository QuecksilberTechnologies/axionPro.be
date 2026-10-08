import os, psycopg

def cs(s):
 p={}
 for x in s.split(';'):
  if '=' in x:
   k,v=x.split('=',1);p[k.strip().lower()]=v.strip()
 return dict(host=p.get('host') or p.get('server'),port=p.get('port','5432'),dbname=p.get('database'),user=p.get('username') or p.get('user id'),password=p.get('password'),sslmode=(p.get('ssl mode') or 'prefer').lower())
q='''SELECT lc."LoginId", e."Id", e."FirstName", e."LastName", r."RoleName", r."RoleType" FROM axionpro."LoginCredential" lc JOIN axionpro."Employee" e ON e."Id"=lc."EmployeeId" LEFT JOIN axionpro."UserRole" ur ON ur."EmployeeId"=e."Id" AND ur."IsPrimaryRole"=true LEFT JOIN axionpro."Role" r ON r."Id"=ur."RoleId" WHERE lc."TenantId"=7 AND lc."IsActive"=true ORDER BY e."Id"'''
for label,s in [('Local',os.environ['AXION_LOCAL_CS']),('Render',os.environ['AXION_RENDER_CS'])]:
 with psycopg.connect(**cs(s)) as c:
  with c.cursor() as x:
   x.execute(q); print(label, x.fetchall())
