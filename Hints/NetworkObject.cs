using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000808 RID: 2056
	public abstract class NetworkObject<TData> : NetworkMessage
	{
		// Token: 0x06002F98 RID: 12184
		public abstract void Deserialize(NetworkReader reader);

		// Token: 0x06002F99 RID: 12185
		public abstract void Serialize(NetworkWriter writer);
	}
}
