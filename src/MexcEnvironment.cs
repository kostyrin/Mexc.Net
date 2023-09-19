using Mexc.Net.Objects;

namespace Mexc.Net
{
    /// <summary>
    /// OKX environments
    /// </summary>
    public class MexcEnvironment : TradeEnvironment
    {
        /// <summary>
        /// Rest API base address
        /// </summary>
        public string RestAddress { get; }
        /// <summary>
        /// Socket API base address
        /// </summary>
        public string SocketAddress { get; }

        internal MexcEnvironment(string name,
            string restAddress, string socketAddress) : base(name)
        {
            RestAddress = restAddress;
            SocketAddress = socketAddress;
        }

        /// <summary>
        /// Live environment
        /// </summary>
        public static MexcEnvironment Live { get; }
            = new MexcEnvironment(TradeEnvironmentNames.Live,
                                   MexcApiAddresses.Default.UnifiedRestAddress,
                                   MexcApiAddresses.Default.UnifiedSocketAddress);

        /// <summary>
        /// Live environment
        /// </summary>
        public static MexcEnvironment Demo { get; }
            = new MexcEnvironment(TradeEnvironmentNames.Testnet,
                                   MexcApiAddresses.Demo.UnifiedRestAddress,
                                   MexcApiAddresses.Demo.UnifiedSocketAddress);

        /// <summary>
        /// Create a custom environment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restAddress"></param>
        /// <param name="socketAddress"></param>
        /// <returns></returns>
        public static MexcEnvironment CreateCustom(string name, string restAddress, string socketAddress)
            => new MexcEnvironment(name, restAddress, socketAddress);
    }
}
