// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    public class KrauseTradeData
    {
        private uint _identifier;
        private int _topBottomIdentifier;
        private DateTime _entryDate;
        private decimal _entryPrice;
        private TradingEnumerations.KrauseTradeReason _entryReason;
        private TradingEnumerations.KrauseTradeAdditionalEntryReason _entryAdditionalReason;
        private uint _contracts;
        private TradingEnumerations.TradeStatus _entryType;
        private DateTime? _exitDate;
        private decimal? _exitPrice;
        private TradingEnumerations.KrauseTradeReason _exitReason;

        public uint Identifier
        {
            get { return _identifier; }
        }

        public int TopBottomIdentifier
        {
            get { return _topBottomIdentifier; }
        }

        public DateTime EntryDate
        {
            get { return _entryDate; }
        }

        public decimal EntryPrice
        {
            get { return _entryPrice; }
        }

        public TradingEnumerations.KrauseTradeReason EntryReason
        {
            get { return _entryReason; }
        }

        public TradingEnumerations.KrauseTradeAdditionalEntryReason EntryAdditionalReason
        {
            get { return _entryAdditionalReason; }
        }

        public uint Contracts
        {
            get { return _contracts; }
        }

        public TradingEnumerations.TradeStatus EntryType
        {
            get { return _entryType; }
        }

        public DateTime? ExitDate
        {
            get { return _exitDate; }
            set
            {
                if (_exitDate != value)
                {
                    _exitDate = value;
                }
            }
        }

        public decimal? ExitPrice
        {
            get { return _exitPrice; }
            set
            {
                if (_exitPrice != value)
                {
                    _exitPrice = value;
                }
            }
        }

        public TradingEnumerations.KrauseTradeReason ExitReason
        {
            get { return _exitReason; }
            set
            {
                if (_exitReason != value)
                {
                    _exitReason = value;
                }
            }
        }

        // The usual constructor
        public KrauseTradeData(uint identifier,
                               int topBottomIdentifier,
                               DateTime entryDate, 
                               decimal entryPrice, 
                               TradingEnumerations.KrauseTradeReason entryReason, 
                               TradingEnumerations.KrauseTradeAdditionalEntryReason entryAdditionalReason,
                               uint contracts, 
                               TradingEnumerations.TradeStatus entryType,
                               decimal? exitPrice, 
                               DateTime? exitDate,
                               TradingEnumerations.KrauseTradeReason exitReason)
        {
            _identifier = identifier;
            _topBottomIdentifier = topBottomIdentifier;
            _entryDate = entryDate;
            _entryPrice = entryPrice;
            _entryReason = entryReason;
            _entryAdditionalReason = entryAdditionalReason;
            _contracts = contracts;
            _entryType = entryType;
            _exitPrice = exitPrice;
            _exitDate = exitDate;
            _exitReason = exitReason;
        }

        // The constructor to clear the trade
        public KrauseTradeData()
        {
            _identifier = uint.MaxValue;
            _topBottomIdentifier = -1;
            _entryDate = DateTime.MinValue;
            _entryPrice = 0.0M;
            _entryReason = TradingEnumerations.KrauseTradeReason.NoTrade;
            _entryAdditionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.None;
            _contracts = 0;
            _entryType = TradingEnumerations.TradeStatus.NoTrade;
            _exitPrice = 0.0M;
            _exitDate = DateTime.MinValue;
            _exitReason = TradingEnumerations.KrauseTradeReason.NoTrade;
        }

        public decimal? CalculateProfitPoints()
        {
            decimal? profitPoints = null;
            if (_exitPrice.HasValue)
            {
                if (_entryType == TradingEnumerations.TradeStatus.Long)
                {
                    profitPoints = _contracts * (_exitPrice - _entryPrice);
                }
                else if (_entryType == TradingEnumerations.TradeStatus.Short)
                {
                    profitPoints = _contracts * (_entryPrice - _exitPrice);
                }
            }
            return profitPoints;
        }
    }
}

