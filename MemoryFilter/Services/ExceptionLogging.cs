using System;
using System.Diagnostics;
using Sentry;
using Sentry.Protocol;

namespace MemoryFilter.Services {

    public static class ExceptionLogging {
        private const string SentryId = "https://f0c389cfed134e42b791dab600f0707c@sentry.io/1382570";

        [DebuggerStepThrough]
        public static SentryId CaptureEvent(SentryEvent evt, Scope scope) {
            using (CreateSdk()) {
                return SentrySdk.CaptureEvent(evt, scope);
            }
        }

        private static IDisposable CreateSdk() {
            return SentrySdk.Init(SentryId);
        }

        /// <summary>
        ///     Captures the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The Id of the event</returns>
        [DebuggerStepThrough]
        public static SentryId CaptureException(this Exception exception) {
            using (CreateSdk()) {
                return SentrySdk.CaptureException(exception);
            }
        }

        /// <summary>
        ///     Captures the message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="level">The message level.</param>
        /// <returns>The Id of the event</returns>
        [DebuggerStepThrough]
        public static SentryId CaptureMessage(string message, SentryLevel level = SentryLevel.Info) {
            using (CreateSdk()) {
                return SentrySdk.CaptureMessage(message, level);
            }
        }
    }

}