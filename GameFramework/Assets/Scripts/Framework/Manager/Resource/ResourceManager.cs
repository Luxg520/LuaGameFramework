using UnityEngine;
using System.Collections;
/// <summary>
/// 资源管理器
/// </summary>
#if UNITY_5
public class ResourceManager : ResourceBundle
#else
public class ResourceManager : ResourceLocal
#endif
{

}