/*
using System;

namespace RosSharp.RosBridgeClient
{
    public static class MessageReflector
    {
        public static Type Type(string typeString)
        {
            Type type = System.Type.GetType(fullName(typeString));
            return (typeof(Message).IsAssignableFrom(type) ? type : null);
        }

        public static string RosMessageType(Type type)
        {
            return (typeof(Message).IsAssignableFrom(type) ?
                type.GetField("type").GetRawConstantValue().ToString() : null);

        }

        public static string RosMessageType(string typeString)
        {
            return RosMessageType(Type(typeString));
        }

        private static string assembly
        {
            get
            {
                string assembly = typeof(MessageReflector).FullName.ToString();
                int pos = assembly.IndexOf("MessageReflector");
                return assembly.Substring(0, pos);
            }
        }

        private static string fullName(string typeString)
        {
            return (typeString.StartsWith(assembly) ? typeString : assembly + typeString);
        }
    }
}
*/