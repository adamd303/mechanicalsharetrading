// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    /// <summary>
    /// A class containing enumerations relevant to trading
    /// </summary>
    public class TradingEnumerations
    {
        /// <summary>
        /// The Trend Enumeration
        /// </summary>
        public enum Trend
        {
            Up,
            Down,
            Unknown
        }

        /// <summary>
        /// An enumeration which stores the status of the trading system
        /// </summary>
        public enum TradeStatus
        {
            Long,
            Short,
            NoTrade
        }

        /// <summary>
        /// An enumeration which stores the type of Top/Bottom
        /// </summary>
        public enum TopBottom
        {
            Top,
            Bottom,
            Unknown,
        }

        /// <summary>
        /// An enumeration which stores the reason for Entering/Exiting a trade for Gann 2 Day System
        /// </summary>
        public enum GannTradeReason
        {
            NoTrade,
            DefinateMainTrendChangeToUp,
            DefinateMainTrendChangeToDown,
            MainTrendUpPyramid,
            MainTrendDownPyramid,
            ExitStopHit,
            NotExitedYet
        }

        /// <summary>
        /// An enumeration which stores the additional reason for Entering a trade for Gann 2 Day System
        /// </summary>
        public enum GannTradeAdditionalEntryReason
        {
            None,
            PossibleMinorChangeToUp,
            DefinateMinorChangeToUp,
            ExceedHi,
            PossibleMinorChangeToDown,
            DefinateMinorChangeToDown,
            ExceedLo
        }

        /// <summary>
        /// An enumeration which stores the trades types in the list for Gann 2 Day System
        /// </summary>
        public enum GannTradeTypesInList
        {
            None,
            ShortOnly,
            LongOnly,
            Mixture
        }

        /// <summary>
        /// An enumeration which stores the reason for Entering/Exiting a trade for Krause System
        /// </summary>
        public enum KrauseTradeReason
        {
            NoTrade,
            DefinateMainTrendChangeToUp,
            DefinateMainTrendChangeToDown,
            MainTrendUpPyramid,
            MainTrendDownPyramid,
            ProfitProtection1,
            ProfitProtection2,
            NotExitedYet
        }

        /// <summary>
        /// An enumeration which stores the additional reason for Entering a trade for Krause System
        /// </summary>
        public enum KrauseTradeAdditionalEntryReason
        {
            None,
            PossibleMinorChangeToUp,
            ExceedHi,
            PossibleMinorChangeToDown,
            ExceedLo
        }

        /// <summary>
        /// An enumeration which stores the trades types in the list for Krause System
        /// </summary>
        public enum KrauseTradeType
        {
            None,
            Short,
            Long,
        }
    }
}
