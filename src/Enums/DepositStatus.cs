﻿using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums;

/// <summary>
/// Status of a deposit
/// </summary>
public enum DepositStatus
{
    /// <summary>
    /// In progress
    /// </summary>
    [Map("PROCESSING")]
    Processing,
    /// <summary>
    /// Successful
    /// </summary>
    [Map("SUCCESS")]
    Success,
    /// <summary>
    /// Failed
    /// </summary>
    [Map("FAILURE")]
    Failure
}
