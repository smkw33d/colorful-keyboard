# colorful-keyboard

一个用于七彩虹笔记本键盘灯的 Windows 小工具，支持 RGB 循环和自定义颜色。

本项目基于原项目修改：

- 原作者：moshuiD
- 原项目：<https://github.com/moshuiD/Colorful-Keyborad-Led-Color-Setting>
- 许可证：见仓库内 `LICENSE`

## 功能

- RGB 循环灯效
- 自定义键盘灯颜色
- 速度滑条，范围 `1-15`，默认 `7`
- 三分区键盘灯同步设置

## 下载

- 最新 Release：<https://github.com/smkw33d/Colorful-Keyborad-Led-Color-Setting/releases/latest/download/colorful-keyboard.zip>
- GitHub Actions 构建记录：<https://github.com/smkw33d/Colorful-Keyborad-Led-Color-Setting/actions/workflows/build.yml>

Release 压缩包包含 `colorful-keyboard.exe`、配置文件、README 和 LICENSE，不包含 `InsydeDCHU.dll`。

## 使用

1. 编译或下载程序。
2. 将 `InsydeDCHU.dll` 放到程序同一目录。
3. 运行 `colorful-keyboard.exe`。
4. 点击 `RGB循环` 开始循环，点击 `停止循环` 停止。
5. 点击 `自定义RGB` 选择固定颜色。

如果提示 `InsydeDCHU.dll` 缺失，请确认 DLL 和 exe 在同一个目录。

如果提示 DLL 位数不匹配，请使用 64 位版本的 `InsydeDCHU.dll`，并使用当前项目配置重新编译。

## 本版本更改

- 程序名称改为 `colorful-keyboard`。
- 移除了启动时的免责声明弹窗。
- 移除了程序界面中的作者信息、关于按钮和源码链接。
- RGB 循环从无延迟线程循环改为可取消异步循环，降低 CPU 占用。
- RGB 循环刷新限制为约 25 FPS。
- 修复速度条调整后 RGB 循环可能停在某个颜色的问题。
- 速度条范围改为 `1-15`，默认值改为 `7`。
- 关闭窗口、停止循环、选择自定义颜色时会正确取消后台循环。
- 关闭 `Prefer 32-bit`，避免与 64 位 `InsydeDCHU.dll` 位数不匹配。
- 增加 `.gitignore`，忽略 Visual Studio 构建输出。

## 编译

Release：

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' ColorfulLedKeyboardSet\ColorfulLedKeyboardSet.sln /t:Build /p:Configuration=Release /p:Platform="Any CPU"
```

Debug：

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' ColorfulLedKeyboardSet\ColorfulLedKeyboardSet.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"
```

编译产物位于：

- `ColorfulLedKeyboardSet\bin\Release\colorful-keyboard.exe`
- `ColorfulLedKeyboardSet\bin\Debug\colorful-keyboard.exe`

## 发布

推送 `v*` tag 会自动构建 Release 并上传 `colorful-keyboard.zip`：

```powershell
git tag v1.0.0
git push origin v1.0.0
```

## 注意

该程序通过 `InsydeDCHU.dll` 调用设备接口修改键盘灯。不同机型和 DLL 版本可能不兼容，使用前请自行确认风险。
