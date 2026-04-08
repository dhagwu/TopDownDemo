using UnityEngine;
using XLua;

public class XLuaTest : MonoBehaviour
{
    private LuaEnv luaEnv;

    void Start()
    {
        luaEnv = new LuaEnv();
        luaEnv.DoString("CS.UnityEngine.Debug.Log('hello from xLua')");
    }

    void OnDestroy()
    {
        luaEnv?.Dispose();
        luaEnv = null;
    }
}