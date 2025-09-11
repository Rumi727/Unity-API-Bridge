using RuniOS.APIBridge;
using UnityEngine;

[assembly: GenerateAPIBridgeForAssembly("UnityEngine.CoreModule")]
[assembly: GenerateAPIBridgeForType(typeof(DrivenPropertyManager))]
[assembly: GenerateAPIBridgeForType(typeof(NumericFieldDraggerUtility), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(UINumericFieldsUtils))]