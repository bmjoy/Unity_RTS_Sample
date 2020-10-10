﻿using System;
using System.Collections.Generic;
using Game.Services.Base;
using Game.Services.Interfaces;
using UnityEngine;

namespace Game.Services
{
    public class GameServices : MonoBehaviour
    {

        [SerializeField] private GameServiceMonoBehaviour[] _servicePrefabs;
        [SerializeField] private GameServiceScriptableObject[] _serviceScriptableObjects;
        
        public static GameServices Instance { get; private set; }


        private Dictionary<string, IGameService> _servicesDict = new Dictionary<string, IGameService>(20);


        public T GetService<T>() where T : IGameService
        {
            string key = nameof(T);
            if (!_servicesDict.ContainsKey(key))
            {
                Debug.LogError($"{key} not found in services!");
                throw new InvalidOperationException();
            }

            return (T) _servicesDict[key];
        }

        private void RegisterService<T>(T service) where T : IGameService
        {
            string key = nameof(T);
            if (_servicesDict.ContainsKey(key))
            {
                Debug.LogError($"{key} already registred!");
                return;
            }

            _servicesDict.Add(key, service);
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError($"Some instance of {nameof(GameServices)} loaded!");
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(this.gameObject);

            InitialRegister();
        }

        private void InitialRegister()
        {
            InitialRawServices();
            InitialMonoBehServices();
            InitialScriptableObjectsServices();
        }

        private void InitialRawServices()
        {
            // Some raw services
        }

        private void InitialMonoBehServices()
        {
            for (int i = 0; i < _servicePrefabs.Length; i++)
            {
                var prefab = _servicePrefabs[i];
                if (prefab != null)
                {
                    var instance = Instantiate(prefab, transform);
                    RegisterService(instance);
                }
            }
        }

        private void InitialScriptableObjectsServices()
        {
            foreach (var soService in _serviceScriptableObjects)
            {
                if (soService != null)
                {
                    RegisterService(soService);
                }
            }
        }
    }  
}
