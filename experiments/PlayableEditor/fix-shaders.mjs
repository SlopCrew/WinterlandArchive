// Workaround https://github.com/AssetRipper/AssetRipper/issues/695
// Only used with AssetRipper Shader->yaml export option
// I'm not using this script anymore, because yaml exported shaders were broken in Unity Editor, even with this fix.

import {readFile, readdir, writeFile} from 'fs/promises';

// TODO use glob to recursively find shader assets
// .asset extension, line 4 is `Shader:`

const arrayRegex = /- (- \d+(\n +- \d+)*)/g;

for(const file of await readdir('.')) {
    if(!file.endsWith('.asset')) continue;
    console.dir(file);

    let content = await readFile(file, 'utf8');
    content = content.replace(arrayRegex, (_0, _1) => {
        console.dir(_1);
        return `- [` + _1.split('\n').map(s =>
            s.trim().replace('- ', '')
        ).join(',') + `]`;
    });
    await writeFile(file, content);
}
