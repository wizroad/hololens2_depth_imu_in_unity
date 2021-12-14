# hololens2_depth_imu_in_unity

get depth and imu sensor stream from hololens2 at C# script in Unity project
--
In your unity project..
1. Copy the Plugin and Scripts folder into your Assets folder. (you may need to edit dll import setting)
2. you can use ResearchModeSensor class in Scripts/TestController.cs in order to start and stop sensor steram. (see the sample unity project)
3. Bulid the unity project and open .sln on VisualStudio.
4. Check capability setting in package.menifest (with the one in this repository)
5. Deploy the UWP project to your hololens2 (ARM64, device)

---
dll import setting
* ARM64 - WSAPLayer, UWP, ARM64, il 2 Cpp
* x64 - Editor, x86_64, Any OS
