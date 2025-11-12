using RuniOS.APIBridge;
using UnityEngine;

[assembly: GenerateAPIBridgeForAssembly("UnityEngine.CoreModule")]
[assembly: GenerateAPIBridgeForType(typeof(DrivenPropertyManager), forceStatic = true)]