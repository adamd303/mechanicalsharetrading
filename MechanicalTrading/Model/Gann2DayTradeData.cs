// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    /// <summary>
    /// The data for one trade
    /// </summary>
    public class Gann2DayTradeData
    {
        private uint _identifier;
        private int _topBottomIdentifier;
        private DateTime _entryDate;
        private decimal _entryPrice;
        private TradingEnumerations.GannTradeReason _entryReason;
        private TradingEnumerations.GannTradeAdditionalEntryReason _entryAdditionalReason;
        private uint _contracts;
        private TradingEnumerations.TradeStatus _entryType;
        private decimal _exitStop;
        private DateTime? _exitDate;
        private decimal? _exitPrice;
        private TradingEnumerations.GannTradeReason _exitReason;

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

        public TradingEnumerations.GannTradeReason EntryReason
        {
            get { return _entryReason; }
        }

        public TradingEnumerations.GannTradeAdditionalEntryReason EntryAdditionalReason
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

        public decimal ExitStop
        {
            get { return _exitStop; }
            set { _exitStop = value; }
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

        public TradingEnumerations.GannTradeReason ExitReason
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

        public Gann2DayTradeData(uint identifier,
                                 int topBottomIdentifier,
                                 DateTime entryDate, 
                                 decimal entryPrice, 
                                 TradingEnumerations.GannTradeReason entryReason, 
                                 TradingEnumerations.GannTradeAdditionalEntryReason entryAdditionalReason,
                                 uint contracts, 
                                 TradingEnumerations.TradeStatus entryType,
                                 decimal exitStop, 
                                 decimal? exitPrice, 
                                 DateTime? exitDate,
                                 TradingEnumerations.GannTradeReason exitReason)
        {
            _identifier = identifier;
            _topBottomIdentifier = topBottomIdentifier;
            _entryDate = entryDate;
            _entryPrice = entryPrice;
            _entryReason = entryReason;
            _entryAdditionalReason = entryAdditionalReason;
            _contracts = contracts;
            _entryType = entryType;
            _exitStop = exitStop;
            _exitPrice = exitPrice;
            _exitDate = exitDate;
            _exitReason = exitReason;
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
