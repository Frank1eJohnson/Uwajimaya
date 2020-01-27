# Package and Upload


## Uwajimaya

### Expected directory structure

```sh

```


### Scripts

```sh

```


## Uwajimaya: Prologue

### Expected directory structure
```sh
<UwajimayaGitRepo> - The path to the Uwajimaya git repository

# Windows64
<UwajimayaGitRepo>/Packaged/UwajimayaPrologue_Windows64_<VERSION>_<GIT_COMMIT>/

# Linux
<UwajimayaGitRepo>/Packaged/UwajimayaPrologue_Steam_Linux_<VERSION>_<GIT_COMMIT>/
```


### Scripts

```sh
# Package ALL (Windows + Linux)
./Tools/Uwajimaya/Demo_ItchGameJolt_PackageAll.py --unattended

# [OPTIONAL] Package Windows
# ./Tools/Uwajimaya/Demo_ItchGameJolt_PackageWindows.py
# [OPTIONAL] Package Linux
# ./Tools/Uwajimaya/Demo_ItchGameJolt_PackageLinux.py

# Upload to Itch
./Tools/Uwajimaya/Demo_Itch_Upload.py

# Sentry crash reporter
./Tools/Uwajimaya/Demo_Sentry_UploadDif.py --itch-gamejolt
```

NOTE: to run all the commands all in one just run
```sh
./Tools/Uwajimaya/Demo_ItchGameJolt_All_Run.sh
```
