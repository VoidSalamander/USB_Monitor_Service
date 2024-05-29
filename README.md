
```
USB_Monitor_Service
| .vs
| Properties
| bin <-- 只留這個
| obj
.gitattributes
...
WindowsService_tutorial.sln
```

```
bin
| Debug
| Release
	WindowsService_tutorial.InstallLog
	WindowsService_tutorial.InstallState
	WindowsService_tutorial.exe <- 執行檔
	WindowsService_tutorial.exe.config
	WindowsService_tutorial.pdb
InstallUtil.InstallLog
install.bat <- 安裝：以管理員執行
uninstall.bat <- 解除安裝
```

開啟 windows服務：
win + R -> services.msc
找到 USB_Service.Demo
啟動並將啟動類型設成自動

log檔位置
win + R -> eventvwr
-> 應用程式及服務紀錄檔
-> MyApplicationLog
