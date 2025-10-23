namespace poker.api.Models;

public static class Constants
{
	public static class HubExceptionMessage
	{
		public const string NotInTableYou = "You are not in the table";
		public const string NotInTableOther = "Player not found in table";
		public const string NoTableDeal = "Table not available to deal";
		public const string NoTableJoin = "Table not available to join";

	}
	
    public static class HubMethods
    {
        public const string ReceiveTable = "ReceiveTable";
        public const string ReceivePlayerChange = "ReceviePlayerChange";
        public const string ReceiveKicked = "ReceivedKicked";
		public const string ReceiveDeal = "ReceiveDeal";
		public const string ReceiveShow = "ReceiveShow";
		public const string ReceiveNudge = "ReceiveNudge";
    }
    
}