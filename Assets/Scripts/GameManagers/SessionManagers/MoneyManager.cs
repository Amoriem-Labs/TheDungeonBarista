using System;
using Sirenix.OdinInspector;
using TDB.ShopSystem.Framework;
using TDB.Utils.DataPersistence;
using UnityEngine;

namespace TDB.GameManagers.SessionManagers
{
    public class MoneyManager : MonoBehaviour, IMoneyDataHolder, IGameDataWriter
    {
        [ShowInInspector] private int _money = 0;
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

        public int GetMoney() => _money;

        public void SetMoney(int money)
        {
            _money = money;
            OnMoneyUpdate?.Invoke();
        }

        public Action OnMoneyUpdate { get; set; }
        public Action<ReceiveMoneyInfo> OnReceiveMoney { get; set; }
            
        public void WriteToData(GameData data)
        {
            data.Money = _money;
        }

        public void ReceiveMoneyFrom(int amount, Vector3 moneySourcePosition)
        {
            SetMoney(_money + amount);
            OnReceiveMoney?.Invoke(new ReceiveMoneyInfo()
            {
                MoneySourcePosition = moneySourcePosition,
            });
        }
    }

    public struct ReceiveMoneyInfo
    {
        public Vector3 MoneySourcePosition;
    }
}