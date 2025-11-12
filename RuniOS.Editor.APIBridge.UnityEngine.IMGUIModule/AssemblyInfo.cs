using RuniOS.APIBridge;
using UnityEngine;

[assembly: APIBridgeNamespace("RuniOS.Editor.APIBridge")]
[assembly: GenerateAPIBridgeForAssembly("UnityEngine.IMGUIModule")]
[assembly: GenerateAPIBridgeForType(typeof(GUIUtility), excludeMember = ["imeCompositionMode"], forceStatic = true)]//
[assembly: GenerateAPIBridgeForType(typeof(EventInterests))]