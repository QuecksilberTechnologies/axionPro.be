from pathlib import Path
p=Path('tmp/inspect_policy_ids.py')
s=p.read_text()
s=s.replace('SELECT "Id","OperationCode" FROM axionpro."Operation" WHERE "OperationCode" IN (\'ADD\',\'UPLOAD\',\'VIEW\') ORDER BY "Id"','SELECT "Id","OperationName","OperationType" FROM axionpro."Operation" ORDER BY "Id"')
p.write_text(s)
