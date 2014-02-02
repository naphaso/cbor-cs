using System;

namespace Naphaso.Cbor
{
    /// <summary>
    /// The date time extensions.
    /// </summary>
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// The unix epoch.
        /// </summary>
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

        /// <summary>
        /// The get current unix timestamp in millis.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// The get unix timestamp in seconds.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long GetUnixTimestampSeconds(this DateTime time)
        {
            return (long)(time - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// The DateTime from unix timestamp in millis.
        /// </summary>
        /// <param name="millis">
        /// The millis.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        /// <summary>
        /// The get current unix timestamp in seconds.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// The DateTime from unix timestamp in seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}
