using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Services
{
    public class ServiceLocator : MonoBehaviour
    {
        private static Dictionary<Type, object> _services = new();

        public void RegisterService<T>(T service)
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
                throw new Exception("Service already registered.");

            _services[type] = service;
        }

        public static T GetService<T>()
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
                return (T)_services[type];

            throw new Exception($"Service of type {type} not found.");
        }
    }
}
