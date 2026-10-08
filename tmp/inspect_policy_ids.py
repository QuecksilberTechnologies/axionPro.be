import os, psycopg

def cs(s):
 p={}
 for x in s.split(';'):
  if '=' in x:
   k,v=x.split('=',1);p[k.strip().lower()]=v.strip()
 return dict(host=p.get('host') or p.get('server'),port=p.get('port','5432'),dbname=p.get('database'),user=p.get('username') or p.get('user id'),password=p.get('password'),sslmode=(p.get('ssl mode') or 'prefer').lower())
with psycopg.connect(**cs(os.environ['AXION_RENDER_CS'])) as c:
 with c.cursor() as x:
  for label,q in [
   ('modules','SELECT "Id","ModuleCode" FROM axionpro."Module" WHERE "ModuleCode" IN (\'TENANT_POLICY_DEFINITIONS\',\'TENANT_POLICY_TYPES\')'),
   ('ops','SELECT "Id","OperationName","OperationType" FROM axionpro."Operation" ORDER BY "Id"'),
   ('ptype','SELECT pt."Id",pt."PolicyTypeCode",pc."CategoryCode" FROM axionpro."PolicyType" pt JOIN axionpro."PolicyCategory" pc ON pc."Id"=pt."PolicyCategoryId" WHERE pt."TenantId"=7 AND pt."PolicyTypeCode"=\'ANNUAL_LEAVE\''),
   ('rtypes','SELECT "Id","RuleTypeCode" FROM axionpro."PolicyRuleType" WHERE "RuleTypeCode" IN (\'ENTITLEMENT\',\'ACCRUAL\',\'CARRY_FORWARD\')'),
   ('leaves','SELECT "Id","LeaveName" FROM axionpro."LeaveType" WHERE "TenantId"=7 AND "IsActive"=true AND "IsSoftDeleted" IS NOT TRUE ORDER BY "Id"'),
   ('gender','SELECT "Id","GenderName" FROM axionpro."Gender" ORDER BY "Id"'),
   ('doctype','SELECT "Id","DocumentTypeCode" FROM axionpro."PolicyDocumentType" WHERE "DocumentTypeCode"=\'POLICY_DOCUMENT\'')]:
    x.execute(q); print(label,x.fetchall())
