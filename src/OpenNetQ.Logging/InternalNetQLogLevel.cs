// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenNetQ.Logging;

namespace KhaosLog.LoggingProvider
{
    /// <summary>
    /// The log level that <see cref="IInternalNetQLogger"/> can log at.
    /// </summary>
    public enum InternalNetQLogLevel
    {
        /// <summary>
        ///     'TRACE' log level.
        /// </summary>
        TRACE,

        /// <summary>
        ///     'DEBUG' log level.
        /// </summary>
        DEBUG,

        /// <summary>
        ///     'INFO' log level.
        /// </summary>
        INFO,

        /// <summary>
        ///     'WARN' log level.
        /// </summary>
        WARN,

        /// <summary>
        ///     'ERROR' log level.
        /// </summary>
        ERROR
    }
}