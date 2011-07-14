namespace EPiRobots
{
    using EPiServer.Data.Dynamic;

    [EPiServerDataStore(AutomaticallyCreateStore = true)]
    public class RobotsTxtData
    {
        [EPiServerDataIndex]
        public string Key { get; set; }

        public string RobotsTxtContent { get; set; }
    }
}
