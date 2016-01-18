using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandLibs.Manager
{    
    public class DynamicCallbackObject
    {
        public string CallbackKey { get; set; }
        public object CallbackPayload { get; set; }
    }

    public interface ICallbackReceiverInterface
    {
        void Callback(CallbackSubscriptionType callbackSubscriptionType, object callbackObject);
    }

    public interface ICallbackDynamicSenderReceiverInterface : ICallbackReceiverInterface
    {
        string ListenerId { get; set; }
    }

    public enum CallbackSubscriptionType
    {
        DYNAMIC_CALLBACK,
    }

    /// <summary>
    /// @Author - Talha Naqvi
    /// </summary>
    public static class CallbackManager
    {
        private static Dictionary<int, Dictionary<int, WeakReference>> CallbackList = new Dictionary<int, Dictionary<int, WeakReference>>();        

        public static void AssignCallback(string callbackSubscriptionType, ICallbackReceiverInterface callbackInterface)
        {
            var obj = new WeakReference(callbackInterface);
            int enumIndex = callbackSubscriptionType.GetHashCode();
            Dictionary<int, WeakReference> typeCallbackDict = null;
            if (CallbackList.ContainsKey(enumIndex))
            {
                typeCallbackDict = CallbackList[enumIndex];
            }
            else
            {
                typeCallbackDict = new Dictionary<int, WeakReference>();
                CallbackList.Add(enumIndex, typeCallbackDict);
            }
            typeCallbackDict[callbackInterface.GetHashCode()] = obj;
        }

        public static void RemoveCallback(CallbackSubscriptionType callbackSubscriptionType, ICallbackReceiverInterface callbackInterface)
        {
            string enumKey = callbackSubscriptionType.ToString();
            RemoveCallback(enumKey, callbackInterface);
        }

        public static void RemoveCallback(string callbackSubscriptionType, ICallbackReceiverInterface callbackInterface)
        {
            if (!string.IsNullOrEmpty(callbackSubscriptionType))
            {
                int hashCode = callbackInterface.GetHashCode();
                int enumIndex = callbackSubscriptionType.GetHashCode();
                Dictionary<int, WeakReference> typeCallbackDict = null;
                if (CallbackList.ContainsKey(enumIndex))
                {
                    typeCallbackDict = CallbackList[enumIndex];
                }
                if (typeCallbackDict != null && typeCallbackDict.ContainsKey(hashCode))
                {
                    typeCallbackDict.Remove(hashCode);
                }
            }
        }

        public static void GiveCallbacks(string callbackSubscriptionType, object callbackObject, params string[] SuppressedListeners)
        {
            GiveCallbacks(callbackSubscriptionType.GetHashCode(), CallbackSubscriptionType.DYNAMIC_CALLBACK,
                new DynamicCallbackObject()
                {
                    CallbackKey = callbackSubscriptionType,
                    CallbackPayload = callbackObject
                }, SuppressedListeners.Any() ? new HashSet<string>(SuppressedListeners) : null);
        }       

        private static void GiveCallbacks(int key, CallbackSubscriptionType callbackSubscriptionType, object callbackObject, HashSet<string> SuppressedListeners)
        {
            int enumIndex = key;
            Dictionary<int, WeakReference> typeCallbackDict = null;
            if (CallbackList.ContainsKey(enumIndex))
            {
                typeCallbackDict = CallbackList[enumIndex];
            }
            if (typeCallbackDict != null)
            {
                List<int> KeyList = new List<int>(typeCallbackDict.Keys);
                if (SuppressedListeners != null)
                {
                    IterateAndSendDynamicCallbacks(typeCallbackDict, KeyList, callbackSubscriptionType, callbackObject, SuppressedListeners);
                }
                else
                {
                    IterateAndSendCallbacks(typeCallbackDict, KeyList, callbackSubscriptionType, callbackObject);
                }
            }
        }

        private static void IterateAndSendDynamicCallbacks(Dictionary<int, WeakReference> typeCallbackDict, List<int> KeyList, CallbackSubscriptionType callbackSubscriptionType, object callbackObject, HashSet<string> SuppressedListeners)
        {
            foreach (var Key in KeyList)
            {
                if (typeCallbackDict.ContainsKey(Key))
                {
                    var weakObj = typeCallbackDict[Key];
                    if (weakObj.IsAlive)
                    {
                        try
                        {
                            var dynamicCallbackReceiver = weakObj.Target as ICallbackDynamicSenderReceiverInterface;
                            if (dynamicCallbackReceiver != null)
                            {
                                if (!SuppressedListeners.Contains(dynamicCallbackReceiver.ListenerId))
                                    dynamicCallbackReceiver.Callback(callbackSubscriptionType, callbackObject);
                            }
                            else
                                (weakObj.Target as ICallbackReceiverInterface).Callback(callbackSubscriptionType, callbackObject);
                        }
                        catch { }
                    }
                    else
                        typeCallbackDict.Remove(Key);
                }
            }
        }

        private static void IterateAndSendCallbacks(Dictionary<int, WeakReference> typeCallbackDict, List<int> KeyList, CallbackSubscriptionType callbackSubscriptionType, object callbackObject)
        {
            foreach (var Key in KeyList)
            {
                var weakObj = typeCallbackDict[Key];
                if (weakObj.IsAlive)
                {
                    try
                    {
                        (weakObj.Target as ICallbackReceiverInterface).Callback(callbackSubscriptionType, callbackObject);
                    }
                    catch { }
                }
                else
                    typeCallbackDict.Remove(Key);
            }
        }

    }
}
