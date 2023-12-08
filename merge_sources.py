
from os import O_APPEND
from typing import OrderedDict


files = ['TriggerTree.cs', 'TriggerTree.Designer.cs']
files.extend(['FormEditTrigger.cs', 'FormEditTrigger.Designer.cs'])
files.extend(['FormEditSound.cs', 'FormEditSound.Designer.cs'])
files.extend(['FormEditTimer.cs', 'FormEditTimer.Designer.cs'])
files.extend(['FormHistogram.cs', 'FormHistogram.Designer.cs'])
files.extend(['TextBoxX.cs', 'TextBoxX.designer.cs'])
files.extend(['SimpleMessageBox.cs', 'SimpleMessageBox.designer.cs'])
files.extend(['XmlCopyForm.cs', 'XmlCopyForm.Designer.cs'])
files.extend(['HeaderListView.cs', 'HeaderListView.Designer.cs'])
files.extend(['FormResultsTabs.cs', 'FormResultsTabs.Designer.cs'])
files.extend(['ListViewDateComparer.cs'])
files.extend(['Macros.cs'])
files.extend(['Config.cs'])
files.extend(['MyToolStrip.cs'])

merged = []
usings = []
result = []
first_file = True
first_using = False
first_using_index = 0

for fn in files:
  print ('processing ' + fn)
  found_namespace = False
  found_ns_bracket = False
  first_line = True
  f = open(fn, 'r', encoding="utf8")
  if not first_file:
    merged.append('\t#region ' + fn + '\n')
  linenum = 0
  for ln in f:
    linenum += 1
    if first_line:
      first_line = False
      # first line might contain the byte order mark
      # just remove it
      #if ln.startswith('\xef'):
      #  ln = ln[3:]
      if ln.startswith('\ufeff'):
        ln = ln[1:]
    if ln.startswith('using') and ';' in ln:
      usings.append(ln)
      if not first_using:
        first_using = True
        first_using_index = linenum-1
    elif ln.startswith('namespace'):
      if first_file:
        merged.append(ln)
      else:
        found_namespace = True
        found_ns_bracket = False
    elif found_namespace and not found_ns_bracket:
      if '{' in ln:
        found_ns_bracket = True
        if first_file:
          merged.append(ln)
    else:
      merged.append(ln)

  f.close()
  # remove the last } that closes the namespace
  bracket = False
  while (not bracket) and (len(merged) > 1):
    x = merged.pop()
    if '}' in x:
      bracket = True
  if not first_file:
    merged.append('\t#endregion ' + fn + '\n')
  first_file = False

# add the namespace close
merged.append('}\n')

# merge the usings, remove duplicates
nodups = list(OrderedDict.fromkeys(usings))

print (str(len(nodups)), "usings")
print (str(len(merged)), "code lines")

# construct the final file
if first_using_index > 0:
  # if there is 'stuff' before the initial using, do that first
  result = merged[0:first_using_index]
  result.extend(nodups)
  result.extend(merged[first_using_index:])
else:
  # first line of the first file was a using
  result.extend(nodups)
  result.extend(merged)
print (str(len(result)), 'file lines')

file_out = open("single_source\\TriggerTree.cs","w", encoding="utf8")
file_out.writelines(result)
file_out.close()


