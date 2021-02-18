#if UNITY_IOS && USE_APPLE_WATCH
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace VirtualPhenix.AppleWatch
{
    public class VP_SetupWatchExtension
    {
        [PostProcessBuild]
        private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
        {
            PBXProject proj = new PBXProject();
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            proj.ReadFromFile(projPath);
            string targetGuid = proj.GetUnityFrameworkTargetGuid();
            string watchExtensionTargetGuid = PBXProjectExtensions.AddWatchExtension(proj, targetGuid, "watchtest Extension",
                "com.unity3d.watchtest.watchkitapp.watchkitextension",
                "watchtest Extension/Info.plist");
            string watchAppTargetGuid = PBXProjectExtensions.AddWatchApp(proj, targetGuid, watchExtensionTargetGuid,
                "watchtest", "com.unity3d.watchtest.watchkitapp", "watchtest/Info.plist");

            FileUtil.CopyFileOrDirectory("Assets/Plugins/iOSWatchAppFiles/watchtest", Path.Combine(buildPath, "watchtest"));
            FileUtil.CopyFileOrDirectory("Assets/Plugins/iOSWatchAppFiles/watchtest Extension", Path.Combine(buildPath, "watchtest Extension"));

            var filesToBuild = new List<string>
        {
            "watchtest/Interface.storyboard",
            "watchtest/Assets.xcassets",
        };

            foreach (var path in filesToBuild)
            {
                var fileGuid = proj.AddFile(path, path);
                proj.AddFileToBuild(watchAppTargetGuid, fileGuid);
            }

            filesToBuild = new List<string>
        {
            "watchtest Extension/Assets.xcassets",

            "watchtest Extension/ExtensionDelegate.h",
            "watchtest Extension/ExtensionDelegate.m",
            "watchtest Extension/InterfaceController.h",
            "watchtest Extension/InterfaceController.m",
            "watchtest Extension/NotificationController.h",
            "watchtest Extension/NotificationController.m",
        };

            foreach (var path in filesToBuild)
            {
                var fileGuid = proj.AddFile(path, path);
                proj.AddFileToBuild(watchExtensionTargetGuid, fileGuid);
            }

            var filesToAdd = new List<string>
        {
            "watchtest/Info.plist",
            "watchtest Extension/PushNotificationPayload.apns",
            "watchtest Extension/Info.plist",
        };

            foreach (var path in filesToAdd)
                proj.AddFile(path, path);

            proj.WriteToFile(projPath);
        }
    }
}
#endif