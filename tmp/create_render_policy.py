import os, json, urllib.request, urllib.error, uuid, mimetypes, sys
base='https://axionpro-api.onrender.com/api'
def call(path,method='GET',obj=None,headers=None,body=None,ctype='application/json'):
 h=dict(headers or {})
 if obj is not None:
  body=json.dumps(obj).encode(); h['Content-Type']=ctype
 req=urllib.request.Request(base+path,data=body,headers=h,method=method)
 try:
  with urllib.request.urlopen(req,timeout=120) as r: return r.status,r.read().decode()
 except urllib.error.HTTPError as e: return e.code,e.read().decode()
status,text=call('/NewLogin/login','POST',{'loginId':'axionpro.net@gmail.com','password':os.environ['AXION_ADMIN_PASSWORD'],'macAddress':'','ipAddressPublic':'','ipAddressLocal':'127.0.0.1','latitude':0,'longitude':0,'loginDevice':1})
print('login_status',status)
if status>=400: print(text[:1000]);sys.exit(1)
body=json.loads(text); data=body.get('data') or body.get('Data') or {}; token=data.get('accessToken') or data.get('AccessToken') or body.get('accessToken') or body.get('AccessToken')
if not token: raise RuntimeError('Access token missing')
h={'Authorization':'Bearer '+token}
payload={
 'moduleId':110,
 'operationId':1,
 'policyTypeId':1,
 'policyCode':'EMPLOYEE_LEAVE_2026_2027',
 'policyName':'Employee Leave Policy 2026-2027',
 'summary':'Employee leave framework for Casual, Earned, Sick, Maternity, Paternity, Menstrual, Bereavement and Birthday Leave with component-specific entitlement, accrual, carry-forward and eligibility rules.',
 'ownerDepartmentId':None,
 'defaultCurrencyCode':'INR',
 'effectiveFrom':'2026-04-01',
 'effectiveTo':'2027-03-31',
 'changeSummary':'Initial 2026-2027 draft with leave-type-specific rules and eligibility.',
 'leaveTypeIds':[1,2,3,4,5,6,7,8],
 'rules':[
  {'policyRuleTypeId':2,'ruleName':'Casual Leave Annual Entitlement','ruleOrder':10,'ruleConfiguration':json.dumps({'quantity':6,'unit':'DAY'}),'leaveTypeIds':[1]},
  {'policyRuleTypeId':3,'ruleName':'Earned Leave Monthly Accrual','ruleOrder':20,'ruleConfiguration':json.dumps({'frequency':'MONTHLY','amountPerCycle':1.5,'prorateNewJoiner':True}),'leaveTypeIds':[2]},
  {'policyRuleTypeId':4,'ruleName':'Earned Leave Carry Forward','ruleOrder':30,'ruleConfiguration':json.dumps({'enabled':True,'maximumQuantity':30,'expiryMonths':12}),'leaveTypeIds':[2]},
  {'policyRuleTypeId':2,'ruleName':'Sick Leave Annual Entitlement','ruleOrder':40,'ruleConfiguration':json.dumps({'quantity':6,'unit':'DAY'}),'leaveTypeIds':[5]},
  {'policyRuleTypeId':2,'ruleName':'Maternity Leave Statutory Entitlement','ruleOrder':50,'ruleConfiguration':json.dumps({'quantity':182,'unit':'DAY'}),'leaveTypeIds':[3]},
  {'policyRuleTypeId':2,'ruleName':'Paternity Leave Event Entitlement','ruleOrder':60,'ruleConfiguration':json.dumps({'quantity':15,'unit':'DAY'}),'leaveTypeIds':[4]},
  {'policyRuleTypeId':2,'ruleName':'Menstrual Leave Annual Entitlement','ruleOrder':70,'ruleConfiguration':json.dumps({'quantity':12,'unit':'DAY'}),'leaveTypeIds':[6]},
  {'policyRuleTypeId':2,'ruleName':'Bereavement Leave Event Entitlement','ruleOrder':80,'ruleConfiguration':json.dumps({'quantity':5,'unit':'DAY'}),'leaveTypeIds':[7]},
  {'policyRuleTypeId':2,'ruleName':'Birthday Leave Annual Entitlement','ruleOrder':90,'ruleConfiguration':json.dumps({'quantity':1,'unit':'DAY'}),'leaveTypeIds':[8]}
 ],
 'applicability':[
  {'applicabilityMode':1,'genderId':2,'priority':500,'effectiveFrom':'2026-04-01','effectiveTo':'2027-03-31','leaveTypeIds':[3,6]},
  {'applicabilityMode':1,'genderId':1,'priority':500,'effectiveFrom':'2026-04-01','effectiveTo':'2027-03-31','leaveTypeIds':[4]}
 ]
}
status,text=call('/TenantPolicy','POST',payload,h); print('create_status',status); print('create_body',text[:1500])
if status>=400:sys.exit(2)
rd=json.loads(text).get('data') or json.loads(text).get('Data') or {}; policy_id=rd.get('policyId') or rd.get('PolicyId') or rd.get('id') or rd.get('Id'); version_id=rd.get('policyVersionId') or rd.get('PolicyVersionId') or rd.get('versionId') or rd.get('VersionId')
if not policy_id or not version_id: raise RuntimeError('Created IDs missing')
boundary='----Codex'+uuid.uuid4().hex; chunks=[]
def field(n,v): chunks.extend([f'--{boundary}\r\nContent-Disposition: form-data; name="{n}"\r\n\r\n{v}\r\n'.encode()])
for k,v in {'moduleId':110,'operationId':15,'policyVersionId':version_id,'policyDocumentTypeId':1,'documentTitle':'Employee Leave Policy 2026-2027','languageCode':'en','isEmployeeVisible':'true'}.items():field(k,v)
fdata=open(r'output/pdf/AnnualLeave2026-2027.pdf','rb').read(); chunks.extend([f'--{boundary}\r\nContent-Disposition: form-data; name="file"; filename="AnnualLeave2026-2027.pdf"\r\nContent-Type: application/pdf\r\n\r\n'.encode(),fdata,b'\r\n',f'--{boundary}--\r\n'.encode()])
status,utext=call('/TenantPolicy/documents','POST',headers={**h,'Content-Type':'multipart/form-data; boundary='+boundary},body=b''.join(chunks)); print('upload_status',status); print('upload_body',utext[:1500])
if status>=400:sys.exit(3)
print('created_policy_id',policy_id); print('created_version_id',version_id)
