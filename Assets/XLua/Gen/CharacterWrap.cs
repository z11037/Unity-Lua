#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class CharacterWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Character);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 5, 5);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddAttack", _m_AddAttack);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddBuff", _m_AddBuff);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TakeDamage", _m_TakeDamage);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "health", _g_get_health);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "attack", _g_get_attack);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "charactr_name", _g_get_charactr_name);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "buffs", _g_get_buffs);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "debuffs", _g_get_debuffs);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "health", _s_set_health);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "attack", _s_set_attack);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "charactr_name", _s_set_charactr_name);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "buffs", _s_set_buffs);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "debuffs", _s_set_debuffs);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Character();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Character constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddAttack(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _val = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.AddAttack( _val );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBuff(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _buffName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.AddBuff( _buffName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TakeDamage(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _damage = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.TakeDamage( _damage );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_health(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.health);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_attack(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.attack);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_charactr_name(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.charactr_name);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_buffs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.buffs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_debuffs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.debuffs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_health(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.health = (FinalState)translator.GetObject(L, 2, typeof(FinalState));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_attack(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.attack = (FinalState)translator.GetObject(L, 2, typeof(FinalState));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_charactr_name(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.charactr_name = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_buffs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.buffs = (System.Collections.Generic.List<Buff>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<Buff>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_debuffs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Character gen_to_be_invoked = (Character)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.debuffs = (System.Collections.Generic.List<DeBuff>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<DeBuff>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
