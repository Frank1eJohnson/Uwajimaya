# Steam Package & Upload

**Guides:**
1. https://partner.steamgames.com/doc/sdk/installscripts
2. https://partner.steamgames.com/doc/sdk/uploading
3. https://developer.valvesoftware.com/wiki/SteamCMD

> **Note:**
To actually upload change `preview = 0`.
That if  `preview = 1` which means that it will only test that everything is ok.

Main app build file: [`Steam/game/app_build_Uwajimaya_790360.vdf`](/Steam/game/app_build_Uwajimaya_790360.vdf)

Demo app build file: [`Steam/demo/app_build_Uwajimaya_demo_1193760.vdf`](/Steam/demo/app_build_Uwajimaya_demo_1193760.vdf)

## Uwajimaya

### Expected directory structure

```sh
<UwajimayaGitRepo> - The path to the Uwajimaya git repository

# Windows64
<UwajimayaGitRepo>/Packaged/Uwajimaya_Steam_Windows64/

# Linux
<UwajimayaGitRepo>/Packaged/Uwajimaya_Steam_Linux/
```


### Scripts

```sh
# Package ALL (Windows + Linux)
./Tools/Uwajimaya/Steam_PackageAll.py

# [OPTIONAL] Package Windows
./Tools/Uwajimaya/Steam_Package_Windows.py
# [OPTIONAL] Package Linux
./Tools/Uwajimaya/Steam_Package_Linux.py

# Upload
./Tools/Uwajimaya/Steam_Upload.py

# Sentry crash reporter
./Tools/Uwajimaya/Sentry_UploadDif.py --steam
```

**NOTE: to run all the commands all in one just run**
```sh
./Tools/Uwajimaya/Steam_All_Run.sh
```


## Uwajimaya: Prologue

### Expected directory structure
```sh
<UwajimayaGitRepo> - The path to the Uwajimaya git repository

# Windows64
<UwajimayaGitRepo>/Packaged/UwajimayaPrologue_Steam_Windows64/

# Linux
<UwajimayaGitRepo>/Packaged/UwajimayaPrologue_Steam_Linux/
```


### Scripts

```sh
# Package ALL (Windows + Linux)
./Tools/Uwajimaya/Demo_Steam_PackageAll.py

# [OPTIONAL] Package Windows
./Tools/Uwajimaya/Demo_Steam_PackageWindows.py
# [OPTIONAL] Package Linux
./Tools/Uwajimaya/Demo_Steam_PackageLinux.py

# Upload
./Tools/Uwajimaya/Demo_Steam_Upload.py

# Sentry crash reporter
./Tools/Uwajimaya/Demo_Sentry_UploadDif.py --steam
```

**NOTE: to run all the commands all in one just run**
```sh
./Tools/Uwajimaya/Demo_Steam_All_Run.sh
```
