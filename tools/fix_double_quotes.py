#!/usr/bin/env python3
from pathlib import Path
import re
p=Path('axionpro.application')/'DTOS'/'AxionproPostgresScript.txt'
text=p.read_text(encoding='utf-8')
# replace double double-quotes like ""Name"" to "Name"
text=re.sub(r'""([A-Za-z0-9_]+)""', r'"\1"', text)
# also fix occurrences of ""Name with trailing quotes issues
text=re.sub(r'""([A-Za-z0-9_]+)"', r'"\1"', text)
# write back
p.write_text(text,encoding='utf-8')
print('Fixed double double-quotes in',p)