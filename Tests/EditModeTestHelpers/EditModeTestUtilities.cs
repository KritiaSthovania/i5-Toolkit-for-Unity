﻿using i5.Toolkit.Core.Utilities;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace i5.Toolkit.Core.Editor.TestHelpers
{
    public static class EditModeTestUtilities
    {
        public static void ResetScene()
        {
            Assert.IsTrue(Application.isEditor, "This scene reset only works in edit mode tests");
            EditorSceneManager.OpenScene(PackagePathUtils.GetPackagePath() + "Tests/TestResources/SetupTestScene.unity");
        }
    }
}