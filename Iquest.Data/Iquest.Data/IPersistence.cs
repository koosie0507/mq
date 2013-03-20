using System;
using System.Collections.Generic;
using System.Text;

namespace Iquest.Messaging.Data
{
	public interface IPersistence<T>
	{
		IAdd<T> NewAdd();
	}
}
