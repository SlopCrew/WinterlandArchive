import {readFileSync, writeFileSync} from 'fs';
const [, , manifest] = process.argv;
let json = JSON.parse(readFileSync(manifest, 'utf8'));
json = {
    ...json,
    dependencies: {
        "com.unity.ide.visualstudio": "2.0.22",
        "com.unity.ide.vscode": "1.2.5",
        "com.unity.timeline": "1.6.5",
        "com.unity.ugui": "1.0.0",
        ...json.dependencies
    }
};
writeFileSync(manifest, JSON.stringify(json, null, 2));