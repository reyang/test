#!/usr/bin/env python3

import glob
import sys

retval = 0

for filename in glob.glob('**/*.md', recursive=True):
    with open(filename, 'rb') as file:
        content = file.read()
        if all(b < 128 for b in content):
            print('{} [PASS]'.format(filename))
        else:
            retval += 1
            print('{} [FAIL] non-ASCII character found'.format(filename), file=sys.stderr)
            lineno = 0
            for line in content.splitlines():
                if any(b > 127 for b in line):
                    print('  Ln:{} {}'.format(lineno, line), file=sys.stderr)
                lineno += 1

sys.exit(retval)
