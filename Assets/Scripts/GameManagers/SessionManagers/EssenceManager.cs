using System;
using Sirenix.OdinInspector;
using TDB.ShopSystem.Framework;
using TDB.Utils.DataPersistence;
using UnityEngine;

namespace TDB.GameManagers.SessionManagers
{
    public class EssenceManager : MonoBehaviour, IResourceDataHolder, IGameDataWriter
    {
        [ShowInInspector] private int _essence = 0;
        private SessionManager _sessionManager;

        private void Awake()
        {
            _sessionManager = FindObjectOfType<SessionManager>();
        }

        private void OnEnable()
        {
            _sessionManager.RegisterDataWriter(this);
        }

        private void OnDisable()
        {
            _sessionManager.UnregisterDataWriter(this);
        }

        public int GetResource() => _essence;

        public void SetResource(int essence)
        {
            _essence = essence;
            OnResourceUpdate?.Invoke();
        }

        public Action OnResourceUpdate { get; set; }
        
        public void WriteToData(GameData data)
        {
            data.Essence = _essence;
        }

        public void AddEssence(int newEssence) => SetResource(GetResource() + newEssence);

        public static string EssenceToString(int amount) => $"{amount}";
    }
}