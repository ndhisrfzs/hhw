namespace GN
{
	public static class ErrorCode
	{
		public const short ERR_Success = 0;
		public const short ERR_NotFoundActor = 2;

		public const short ERR_AccountOrPasswordError = 102;
		public const short ERR_SessionActorError = 103;
		public const short ERR_NotFoundUnit = 104;
		public const short ERR_ConnectGateKeyError = 105;

		// 大于这个错误抛异常
		public const short ERR_Exception = 1000;

		public const short ERR_RpcFail = 2001;
		public const short ERR_SocketDisconnected = 2002;
		public const short ERR_ReloadFail = 2003;
		public const short ERR_ActorLocationNotFound = 2004;
	}
}