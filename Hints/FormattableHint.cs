using System;

namespace Hints
{
	// Token: 0x020007FE RID: 2046
	public abstract class FormattableHint<THint> : Hint where THint : FormattableHint<THint>
	{
		// Token: 0x06002F78 RID: 12152 RVA: 0x000A00F4 File Offset: 0x0009E2F4
		protected FormattableHint(HintParameter[] parameters, HintEffect[] effects, float durationScalar = 1f) : base(parameters, effects, durationScalar)
		{
		}
	}
}
